using FireSafety.FireSafetyData;
using Novacode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
        public Building ParentBuilding { get { return Parent as Building; } }
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
            //get { return Routes.FirstOrDefault(x => !(x.FirstNode is StartNode && x.LastNode is ExitNode)) != null; }
        }

        /// <summary>
        /// Содержит хотя бы один сформированный путь
        /// </summary>
        public bool ContainsFormedRoutes
        {
            get { return Routes.Any(x => x.IsFormed); }
            //get { return Routes.FirstOrDefault(x => (x.FirstNode is StartNode && x.LastNode is ExitNode)) != null; }
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
        /// <summary>
        /// Пересчитать параметры маршрутов
        /// необходимо при изменении значений отдельных звеньев маршрутов
        /// </summary>
        public async void CalculateParameters()
        {
            IsCalculated = true;
            NeedCalculate = false;

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

                if (firstSection.GetType() == typeof(RoadSection))
                {
                    firstSection.IntensityHumanFlow = ТаблицаЗависимостиГоризонтальныйПуть.Instance.ИнтенсивностьЧерезПлотность(firstSection.DensityHumanFlow);
                    firstSection.MovementSpeed = ТаблицаЗависимостиГоризонтальныйПуть.Instance.СкоростьЧерезПлотность(firstSection.DensityHumanFlow);
                }

                if (firstSection.GetType() == typeof(StairsSection))
                {
                    firstSection.MovementSpeed = ТаблицаЗависимостиЛестницаВниз.Instance.СкоростьЧерезПлотность(firstSection.DensityHumanFlow);
                    firstSection.IntensityHumanFlow = ТаблицаЗависимостиЛестницаВниз.Instance.Интенсивность(firstSection.DensityHumanFlow);
                }

                firstSection.MovementTime = firstSection.Length / firstSection.MovementSpeed;
                route.MovementTime += firstSection.MovementTime;

                var previousSection = firstSection;

                foreach (var section in route.Sections.Skip(1))
                {
                    if (section is FloorsConnectionSection) continue;

                    if (section.GetType() == typeof(RoadSection))
                    {
                        section.IntensityHumanFlow = previousSection.IntensityHumanFlow * previousSection.Width / section.Width;
                        section.MovementSpeed = ТаблицаЗависимостиГоризонтальныйПуть.Instance.СкоростьЧерезИнтенсивность(section.IntensityHumanFlow);
                        section.MovementTime = section.Length / section.MovementSpeed;
                    }
                    if (section.GetType() == typeof(StairsSection))
                    {
                        section.IntensityHumanFlow = previousSection.IntensityHumanFlow * previousSection.Width / section.Width;
                        section.MovementSpeed = ТаблицаЗависимостиГоризонтальныйПуть.Instance.СкоростьЧерезИнтенсивность(section.IntensityHumanFlow);
                        section.MovementTime = section.Length / section.MovementSpeed;
                    }
                    if (section.GetType() == typeof(EntryNode))
                    {
                        section.IntensityHumanFlow = 2.5 + 3.75 * section.Width;
                        section.MovementTime = start.PeopleCount * 0.1 / (section.IntensityHumanFlow * section.Width);
                    }

                    route.MovementTime += section.MovementTime;

                    previousSection = section;
                }
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

        //public DocX CreateDocument(string title)
        //{
        //    var document = DocX.Create(title, DocumentTypes.Document);

        //    var routes = Routes.Where(x => x.IsFormed);

        //    var headerFormat1 = new Formatting() { Bold = true, Size = 16 };
        //    var headerFormat2 = new Formatting() { Bold = true, Size = 12 };
        //    var normalFormat = new Formatting() { Bold = false, Size = 12 };
        //    document.InsertParagraph(Title, false, headerFormat1);
        //    document.InsertParagraph();

        //    foreach (var floor in ParentBuilding.Floors)
        //    {
        //        var planImage = floor.GetEvacuationPlanImage();
        //        using (var ms = new MemoryStream(Settings.GetBytesFromBitmap(planImage.BitmapValue)))
        //        {
        //            double width = 672;
        //            double height = planImage.Height * width / planImage.Width;

        //            var image = document.AddImage(ms); // Create image.
        //            var picture = image.CreatePicture((int)height, (int)width);     // Create picture.

        //            var paragraph = document.InsertParagraph(string.Format("Схема эвакуации '{0}'", floor.Title), false);
        //            paragraph.InsertPicture(picture, 0); // Insert picture into paragraph.
        //        }
        //    }

        //    foreach (var route in routes)
        //    {
        //        document.InsertParagraph(route.Title, false, headerFormat1);
        //        var rowsCount = route.Sections.Count - route.Sections.Count(x => x is FloorsConnectionSection) + 1;
        //        var table = document.AddTable(rowsCount, 7);
        //        table.Rows[0].Cells[0].Paragraphs.First().InsertText("Участок", false, headerFormat2);
        //        table.Rows[0].Cells[1].Paragraphs.First().InsertText("Длина, м", false, headerFormat2);
        //        table.Rows[0].Cells[2].Paragraphs.First().InsertText("Ширина, м", false, headerFormat2);
        //        table.Rows[0].Cells[3].Paragraphs.First().InsertText("Площадь, м2", false, headerFormat2);
        //        table.Rows[0].Cells[4].Paragraphs.First().InsertText("Интенсивность движения людского потока, м/мин", false, headerFormat2);
        //        table.Rows[0].Cells[5].Paragraphs.First().InsertText("Скорость движения людского потока, м/мин", false, headerFormat2);
        //        table.Rows[0].Cells[6].Paragraphs.First().InsertText("Время прохождения участка, мин", false, headerFormat2);

        //        var rowIndex = 1;
        //        foreach (var section in route.Sections)
        //        {
        //            if (section is FloorsConnectionSection) continue;
        //            table.Rows[rowIndex].Cells[0].Paragraphs.First().InsertText(section.Title, false, normalFormat);
        //            table.Rows[rowIndex].Cells[1].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(section.Length, 3)), false, normalFormat);
        //            table.Rows[rowIndex].Cells[2].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(section.Width, 3)), false, normalFormat);
        //            table.Rows[rowIndex].Cells[4].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(section.IntensityHumanFlow, 3)), false, normalFormat);
        //            table.Rows[rowIndex].Cells[5].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(section.MovementSpeed, 3)), false, normalFormat);
        //            table.Rows[rowIndex].Cells[6].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(section.MovementTime, 3)), false, normalFormat);
        //            rowIndex++;
        //        }

        //        document.InsertTable(table);
        //        document.InsertParagraph(string.Format("Общее время t: {0} мин ", Math.Round(route.MovementTime, 3)), false, headerFormat2);
        //        document.InsertParagraph();
        //    }

        //    return document;
        //}
        public override BitmapImage Icon { get { return Settings.Instance.RouteIco; } }

        public bool Param1 { get; set; }
        public bool Param2 { get; set; }
    }
}
