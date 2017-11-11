using FireSafety.FireSafetyData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    /// <summary>
    /// План эвакуации представляет собой коллекцию путей эвакуации
    /// </summary>
    class EvacuationPlan : Entity
    {
        public EvacuationPlan(string title, Building parent) : base(title, parent)
        {
            if (parent == null) throw new Exception("Родительский элемент не может быть нулевым");
            Routes = new ObservableCollection<Route>();
        }
        public ObservableCollection<Route> Routes { get; private set; }

        public void AddRoute(Route route)
        {
            if (Routes.Contains(route)) return;

            Routes.Add(route);
            route.RouteChanged += RoutePropertyChanged;
            CalculateParameters();
        }
        public void AddRoutes(IList routes)
        {
            if (routes == null) return;

            foreach (var route in routes)
                AddRoute(route as Route);
        }
        public void RemoveRoute(Route route)
        {
            if (!Routes.Contains(route)) return;

            Routes.Remove(route);
            route.RouteChanged -= RoutePropertyChanged;
        }
        public void Clear()
        {
            foreach (var route in Routes.ToArray())
                RemoveRoute(route);
        }

        /// <summary>
        /// Содержит хотя бы один несформированный путь
        /// </summary>
        public bool ContainsUnformedRoutes
        {
            get { return Routes.Any(x => !x.IsFormed); }
        }

        /// <summary>
        /// Содержит хотя бы один сформированный путь
        /// </summary>
        public bool ContainsFormedRoutes
        {
            get { return Routes.Any(x => x.IsFormed); }
        }

        public double EvacuationTime
        {
            get { return ParentBuilding.EvacuationTime; }
            set { ParentBuilding.EvacuationTime = value; OnPropertyChanged("EvacuationTime"); }
        }

        private void RoutePropertyChanged(object sender, RouteChangedEventArgs e)
        {
            if (IsCalculated)
                NeedCalculate = true;
            else
                CalculateParameters();
        }

        private bool IsCalculated = false;
        private bool NeedCalculate = false;
        private TimeSpan CalculateInterval = TimeSpan.FromSeconds(0.5);

        private Random rnd = new Random((int)DateTime.Now.Ticks);
        /// <summary>
        /// Пересчитать параметры маршрутов
        /// необходимо при изменении значений отдельных звеньев маршрутов
        /// </summary>
        public async void CalculateParameters()
        {
            IsCalculated = true;
            NeedCalculate = false;
            DelayTime = 0;

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            foreach (var route in Routes)
                route.Length = route.Sections.Sum(x => x.Length);

            var routes = Routes.Where(x => x.IsFormed);
            if (!routes.Any()) return;

            foreach (var route in routes)
            {
                route.MovementTime = 0;

                var firstSection = route.Sections.First();
                var start = route.Start;
                firstSection.DensityHumanFlow = start.PeopleCount * start.ProjectionArea / (firstSection.Length * firstSection.Width);

                switch (firstSection.SectionType)
                {
                    case SectionTypes.HorizontalSection:
                        {
                            firstSection.IntensityHumanFlow = ТаблицаЗависимостиГоризонтальныйПуть.Instance.ИнтенсивностьЧерезПлотность(firstSection.DensityHumanFlow);
                            firstSection.MovementSpeed = ТаблицаЗависимостиГоризонтальныйПуть.Instance.СкоростьЧерезПлотность(firstSection.DensityHumanFlow);
                            break;
                        }
                    case SectionTypes.StaircaseSection:
                        {
                            firstSection.MovementSpeed = ТаблицаЗависимостиЛестницаВниз.Instance.СкоростьЧерезПлотность(firstSection.DensityHumanFlow);
                            firstSection.IntensityHumanFlow = ТаблицаЗависимостиЛестницаВниз.Instance.Интенсивность(firstSection.DensityHumanFlow);
                            break;
                        }
                }

                firstSection.MovementTime = firstSection.Length / firstSection.MovementSpeed;

                var previousSection = firstSection;
                foreach (var section in route.Sections.Skip(1))
                {
                    if (section.SectionType == SectionTypes.Other) continue;

                    switch (section.SectionType)
                    {
                        case SectionTypes.HorizontalSection:
                            {
                                section.IntensityHumanFlow = previousSection.IntensityHumanFlow * previousSection.Width / section.Width;
                                section.MovementSpeed = ТаблицаЗависимостиГоризонтальныйПуть.Instance.СкоростьЧерезИнтенсивность(section.IntensityHumanFlow);
                                section.MovementTime = section.Length / section.MovementSpeed;
                                break;
                            }
                        case SectionTypes.StaircaseSection:
                            {
                                section.IntensityHumanFlow = previousSection.IntensityHumanFlow * previousSection.Width / section.Width;
                                section.MovementSpeed = ТаблицаЗависимостиГоризонтальныйПуть.Instance.СкоростьЧерезИнтенсивность(section.IntensityHumanFlow);
                                section.MovementTime = section.Length / section.MovementSpeed; break;
                            }
                        case SectionTypes.Doorway:
                            {
                                section.IntensityHumanFlow = 2.5 + 3.75 * section.Width;
                                section.MovementTime = start.PeopleCount * 0.1 / (section.IntensityHumanFlow * section.Width); break;
                            }
                    }

                    if (section.First.IncomingSections.Count > 1)
                    {
                        section.DelayTime = TimeSpan.FromSeconds(10 * rnd.NextDouble()).TotalMinutes;
                        section.MovementTime += section.DelayTime;
                        DelayTime += section.DelayTime;
                    }
                    else
                        section.DelayTime = 0;

                    previousSection = section;
                }
                route.MovementTime = route.Sections.Sum(x => x.MovementTime);
            }

            EvacuationTime = Routes.Max(x => x.MovementTime);
            stopWatch.Stop();

            //Делаем паузу перед следующими вычислениями
            await Task.Delay(CalculateInterval - TimeSpan.FromMilliseconds(stopWatch.ElapsedMilliseconds));

            IsCalculated = false;
            if (NeedCalculate)
                CalculateParameters();
        }
        /// <summary>
        /// формирование коллекции маршрутов
        /// </summary>
        public void ComposeRoutes()
        {
            Clear();

            var sections = new List<Section>();
            foreach (var floor in ParentBuilding.Floors)
                sections.AddRange(floor.Objects.Where(x => x is Section).Select(x => x as Section));

            var firstSections = sections.Where(x => x.NoIncoming);

            var routes = new List<Route>();
            int index = 1;

            foreach (var firstSection in firstSections)
            {
                var currentSection = firstSection;
                var currentRoute = new List<Section>();
                var goneSections = new List<Section>();
                var stack = new Stack<Section>();
                stack.Push(currentSection);

                while (stack.Any())
                {
                    currentSection = stack.Peek();
                    if (!currentRoute.Contains(currentSection))
                        currentRoute.Add(currentSection);

                    if (currentSection.NoOutgoing)
                    {
                        var route = new Route(string.Format("Путь {0}", index++), ParentBuilding);
                        route.AddSections(currentRoute);
                        routes.Add(route);

                        goneSections.Add(currentSection);
                    }

                    if (goneSections.Contains(currentSection))
                    {
                        stack.Pop();
                        currentRoute.Remove(currentSection);
                    }
                    else
                    {
                        foreach (var section in currentSection.Last.OutgoingSections)
                            stack.Push(section);
                        goneSections.Add(currentSection);
                    }
                }
            }

            foreach (var route in routes)
                AddRoute(route);
        }

        public override BitmapImage Icon { get { return SettingsManager.Instance.RouteIco; } }

        public bool Param { get { return FireRiskValue < Math.Pow(10, -6); } }
        public double FireRiskValue { get; set; }
        public double DelayTime { get; private set; }
    }
}
