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
        private Building ParentBuilding { get { return Parent as Building; } }
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

        public bool ContainsUnformedRoutes
        {
            get { return Routes.FirstOrDefault(x => !(x.FirstNode is StartNode && x.LastNode is ExitNode)) != null; }
        }
        public bool ContainsFormedRoutes
        {
            get { return Routes.FirstOrDefault(x => (x.FirstNode is StartNode && x.LastNode is ExitNode)) != null; }
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
        private TimeSpan CalculateInterval = TimeSpan.FromSeconds(1);
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

            var рассматриваемыеУчастки = new List<ISection>();
            foreach (var route in routes)
                рассматриваемыеУчастки.AddRange(route.Sections);
            рассматриваемыеУчастки = рассматриваемыеУчастки.Distinct().ToList();

            var рассмотренныеУчастки = new List<ISection>();

            foreach (var route in routes)
            {
                route.MovementTime = 0;

                var первыйУчасток = route.Sections.First();
                var старт = route.FirstNode as StartNode;
                первыйУчасток.DensityHumanFlow = старт.PeopleCount * старт.ProjectionArea / (первыйУчасток.Length * первыйУчасток.Width);

                if (первыйУчасток.GetType() == typeof(RoadSection))
                {
                    первыйУчасток.IntensityHumanFlow = ТаблицаЗависимостиГоризонтальныйПуть.Instance.ИнтенсивностьЧерезПлотность(первыйУчасток.DensityHumanFlow);
                    первыйУчасток.MovementSpeed = ТаблицаЗависимостиГоризонтальныйПуть.Instance.СкоростьЧерезПлотность(первыйУчасток.DensityHumanFlow);
                }

                if (первыйУчасток.GetType() == typeof(StairsSection))
                {
                    первыйУчасток.MovementSpeed = ТаблицаЗависимостиЛестницаВниз.Instance.СкоростьЧерезПлотность(первыйУчасток.DensityHumanFlow);
                    первыйУчасток.IntensityHumanFlow = ТаблицаЗависимостиЛестницаВниз.Instance.Интенсивность(первыйУчасток.DensityHumanFlow);
                }

                первыйУчасток.MovementTime = первыйУчасток.Length / первыйУчасток.MovementSpeed;
                route.MovementTime += первыйУчасток.MovementTime;

                Section предыдущийУчасток = первыйУчасток;

                foreach (var section in route.Sections.Skip(1))
                {
                    if (section is FloorsConnectionSection) continue;

                    if (section.GetType() == typeof(RoadSection))
                    {
                        section.IntensityHumanFlow = предыдущийУчасток.IntensityHumanFlow * предыдущийУчасток.Width / section.Width;
                        section.MovementSpeed = ТаблицаЗависимостиГоризонтальныйПуть.Instance.СкоростьЧерезИнтенсивность(section.IntensityHumanFlow);
                        section.MovementTime = section.Length / section.MovementSpeed;
                    }
                    if (section.GetType() == typeof(StairsSection))
                    {
                        section.IntensityHumanFlow = предыдущийУчасток.IntensityHumanFlow * предыдущийУчасток.Width / section.Width;
                        section.MovementSpeed = ТаблицаЗависимостиГоризонтальныйПуть.Instance.СкоростьЧерезИнтенсивность(section.IntensityHumanFlow);
                        section.MovementTime = section.Length / section.MovementSpeed;
                    }
                    if (section.GetType() == typeof(EntryNode))
                    {
                        section.IntensityHumanFlow = 2.5 + 3.75 * section.Width;
                        section.MovementTime = старт.PeopleCount * 0.1 / (section.IntensityHumanFlow * section.Width);
                    }

                    route.MovementTime += section.MovementTime;

                    предыдущийУчасток = section;
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
            foreach (var этаж in ParentBuilding.Floors)
                sections.AddRange(этаж.Objects.Where(x => x is Section).Select(x => x as Section));

            var стартовыеУчастки = sections.Where(x => x.NoIncoming);

            var routes = new List<Route>();
            int индекс = 1;

            foreach (var стартовыйУчасток in стартовыеУчастки)
            {
                var текущийУчасток = стартовыйУчасток;
                var текущийПуть = new List<Section>();
                var пройденныеУчастки = new List<Section>();
                var стек = new Stack<Section>();
                стек.Push(текущийУчасток);

                while (стек.Any())
                {
                    текущийУчасток = стек.Peek();
                    if (!текущийПуть.Contains(текущийУчасток))
                        текущийПуть.Add(текущийУчасток);

                    if (текущийУчасток.NoOutgoing)
                    {
                        var путь = new Route(string.Format("Путь {0}", индекс++), ParentBuilding);
                        путь.AddSections(текущийПуть);
                        routes.Add(путь);

                        пройденныеУчастки.Add(текущийУчасток);
                    }

                    if (пройденныеУчастки.Contains(текущийУчасток))
                    {
                        стек.Pop();
                        текущийПуть.Remove(текущийУчасток);
                    }
                    else
                    {
                        foreach (var участок in текущийУчасток.Last.OutgoingSections)
                            стек.Push(участок);
                        пройденныеУчастки.Add(текущийУчасток);
                    }
                }
            }

            foreach (var route in routes)
                AddRoute(route);
        }
        public DocX CreateDocument(string title)
        {
            var document = DocX.Create(title, DocumentTypes.Document);

            var routes = Routes.Where(x => x.IsFormed);

            var заголовок1 = new Formatting() { Bold = true, Size = 16 };
            var заголовок2 = new Formatting() { Bold = true, Size = 12 };
            var нормальный = new Formatting() { Bold = false, Size = 12 };
            document.InsertParagraph(Title, false, заголовок1);
            document.InsertParagraph();

            foreach (var floor in ParentBuilding.Floors)
            {
                var схема = floor.GetEvacuationPlanImage();
                using (MemoryStream ms = new MemoryStream(Settings.GetBytesFromBitmap(схема.BitmapValue)))
                {
                    Novacode.Image img = document.AddImage(ms); // Create image.

                    Paragraph p = document.InsertParagraph(string.Format("Схема эвакуации '{0}'", floor.Title), false);
                    double ширина = 672;
                    double высота = схема.Height * ширина / схема.Width;
                    Picture pic1 = img.CreatePicture((int)высота, (int)ширина);     // Create picture.
                    p.InsertPicture(pic1, 0); // Insert picture into paragraph.
                }
            }

            foreach (var route in routes)
            {
                document.InsertParagraph(route.Title, false, заголовок1);
                var числоСтрок = route.Sections.Count - route.Sections.Count(x => x is FloorsConnectionSection) + 1;
                var таблица = document.AddTable(числоСтрок, 7);
                таблица.Rows[0].Cells[0].Paragraphs.First().InsertText("Участок", false, заголовок2);
                таблица.Rows[0].Cells[1].Paragraphs.First().InsertText("Длина, м", false, заголовок2);
                таблица.Rows[0].Cells[2].Paragraphs.First().InsertText("Ширина, м", false, заголовок2);
                таблица.Rows[0].Cells[3].Paragraphs.First().InsertText("Площадь, м2", false, заголовок2);
                таблица.Rows[0].Cells[4].Paragraphs.First().InsertText("Интенсивность движения людского потока, м/мин", false, заголовок2);
                таблица.Rows[0].Cells[5].Paragraphs.First().InsertText("Скорость движения людского потока, м/мин", false, заголовок2);
                таблица.Rows[0].Cells[6].Paragraphs.First().InsertText("Время прохождения участка, мин", false, заголовок2);

                var строка = 1;
                foreach (var участок in route.Sections)
                {
                    if (участок is FloorsConnectionSection) continue;
                    таблица.Rows[строка].Cells[0].Paragraphs.First().InsertText(участок.Title, false, нормальный);
                    таблица.Rows[строка].Cells[1].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(участок.Length, 3)), false, нормальный);
                    таблица.Rows[строка].Cells[2].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(участок.Width, 3)), false, нормальный);
                    // таблица.Rows[строка].Cells[3].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(участок.Площадь, 3)), false, нормальный);
                    таблица.Rows[строка].Cells[4].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(участок.IntensityHumanFlow, 3)), false, нормальный);
                    таблица.Rows[строка].Cells[5].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(участок.MovementSpeed, 3)), false, нормальный);
                    таблица.Rows[строка].Cells[6].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(участок.MovementTime, 3)), false, нормальный);
                    строка++;
                    //if (участок.Конец is Дверь)
                    //{
                    //    var дверь = участок.Конец as Дверь;
                    //    таблица.Rows[строка].Cells[0].Paragraphs.First().InsertText(дверь.Название, false, нормальный);
                    //    таблица.Rows[строка].Cells[2].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(дверь.Ширина, 3)), false, нормальный);
                    //    таблица.Rows[строка].Cells[4].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(дверь.ИнтенсивностьЛюдскогоПотока, 3)), false, нормальный);
                    //    таблица.Rows[строка].Cells[6].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(дверь.ВремяДвижения, 3)), false, нормальный);
                    //    строка++;
                    //}
                }

                document.InsertTable(таблица);
                document.InsertParagraph(string.Format("Общее время t: {0} мин ", Math.Round(route.MovementTime, 3)), false, заголовок2);
                document.InsertParagraph();
            }

            return document;
        }
        public override BitmapImage Icon { get { return Settings.Instance.ПутьIco; } }
    }
}
