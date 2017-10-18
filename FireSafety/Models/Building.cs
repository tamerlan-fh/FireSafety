using FireSafety.FireSafetyData;
using Microsoft.Win32;
using Novacode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    class Building : Entity
    {
        public void РассчетБлокированияПутейЭвакуации()
        {
            var window = new BlockageEvacuationRoutesWindow();

            if (window.ShowDialog() == true)
            {
                ВремяБлокирования = window.ВремяБлокирования.TotalMinutes;
            }
        }

        private double ВремяБлокирования = 0;
        public void РассчетПожарногоРиска()
        {

            if (!Пути.ИмеютсяСформированныеПути)
            {
                MessageBox.Show("Для рассчета пожарного риска необходимо построить хотя бы один путь эвакуации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (ВремяБлокирования == 0)
            {
                var result = MessageBox.Show("Для рассчета пожарного риска необходимо предварительно рассчитать время блокирования путей.\r\nХотите рассчитать?", "Предупреждение", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Cancel) return;
                if (result == MessageBoxResult.Yes) РассчетБлокированияПутейЭвакуации();
            }

            var window = new FireRiskWindow(EvacuationTime, ВремяБлокирования);
            window.ShowDialog();
        }
        public void СформироватьОтчет()
        {
            if (!Пути.ИмеютсяСформированныеПути)
            {
                MessageBox.Show("Для составления отчета необходимо построить хотя бы один путь эвакуации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "docx files (*.docx)|*.docx";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = string.Format("{0}_Отчет.docx", this.Title);

            if (saveFileDialog.ShowDialog() == true)
            {
                var документ = СоздатьДокумент(saveFileDialog.FileName);
                документ.Save();
            }
        }
        private DocX СоздатьДокумент(string название)
        {
            var документ = DocX.Create(название, DocumentTypes.Document);

            var пути = Пути.Пути.Where(x => x.Сформированный);

            var заголовок1 = new Formatting() { Bold = true, Size = 16 };
            var заголовок2 = new Formatting() { Bold = true, Size = 12 };
            var нормальный = new Formatting() { Bold = false, Size = 12 };
            документ.InsertParagraph(Title, false, заголовок1);
            документ.InsertParagraph();

            foreach (var этаж in Floors)
            {
                var схема = этаж.ДатьСхемуЭвакуации();
                using (MemoryStream ms = new MemoryStream(схема.Bytes))
                {
                    Novacode.Image img = документ.AddImage(ms); // Create image.

                    Paragraph p = документ.InsertParagraph(string.Format("Схема эвакуации '{0}'", этаж.Title), false);
                    double ширина = 672;
                    double высота = схема.Высота * ширина / схема.Ширина;
                    Picture pic1 = img.CreatePicture((int)высота, (int)ширина);     // Create picture.
                    p.InsertPicture(pic1, 0); // Insert picture into paragraph.
                }
            }

            foreach (var путь in пути)
            {
                документ.InsertParagraph(путь.Title, false, заголовок1);
                var числоСтрок = путь.Участки.Count - путь.Участки.Count(x => x is Спуск) + 1;
                var таблица = документ.AddTable(числоСтрок, 7);
                таблица.Rows[0].Cells[0].Paragraphs.First().InsertText("Участок", false, заголовок2);
                таблица.Rows[0].Cells[1].Paragraphs.First().InsertText("Длина, м", false, заголовок2);
                таблица.Rows[0].Cells[2].Paragraphs.First().InsertText("Ширина, м", false, заголовок2);
                таблица.Rows[0].Cells[3].Paragraphs.First().InsertText("Площадь, м2", false, заголовок2);
                таблица.Rows[0].Cells[4].Paragraphs.First().InsertText("Интенсивность движения людского потока, м/мин", false, заголовок2);
                таблица.Rows[0].Cells[5].Paragraphs.First().InsertText("Скорость движения людского потока, м/мин", false, заголовок2);
                таблица.Rows[0].Cells[6].Paragraphs.First().InsertText("Время прохождения участка, мин", false, заголовок2);

                var строка = 1;
                foreach (var участок in путь.Участки)
                {
                    if (участок is Спуск) continue;
                    таблица.Rows[строка].Cells[0].Paragraphs.First().InsertText(участок.Title, false, нормальный);
                    таблица.Rows[строка].Cells[1].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(участок.Length, 3)), false, нормальный);
                    таблица.Rows[строка].Cells[2].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(участок.Width, 3)), false, нормальный);
                    // таблица.Rows[строка].Cells[3].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(участок.Площадь, 3)), false, нормальный);
                    таблица.Rows[строка].Cells[4].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(участок.IntensityHumanFlow, 3)), false, нормальный);
                    таблица.Rows[строка].Cells[5].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(участок.Speed, 3)), false, нормальный);
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

                документ.InsertTable(таблица);
                документ.InsertParagraph(string.Format("Общее время t: {0} мин ", Math.Round(путь.ВремяДвижения, 3)), false, заголовок2);
                документ.InsertParagraph();
            }

            return документ;

        }
        public void Сохранить()
        {
            var saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = string.Format("{0}.txt", this.Title);

            if (saveFileDialog.ShowDialog() == true)
            {
                //if ((myStream = saveFileDialog.OpenFile()) != null)
                //{
                //    // Code to write the stream goes here.
                //    myStream.Close();
                //}

            }
        }

        private static int index = 1;
        public Building(string title) : base(title, null)
        {
            Пути = new КоллекцияПутей("Пути эвакуации", this);
            Floors = new ObservableCollection<Floor>();
            Objects = new ObservableCollection<Entity>();
            Objects.Add(Пути);
        }
        public Building() : this(string.Format("Здание {0}", index++)) { }
        public КоллекцияПутей Пути { get; private set; }
        public ObservableCollection<Floor> Floors { get; private set; }
        public ObservableCollection<Entity> Objects { get; private set; }
        public ObservableCollection<Путь> Routes { get { return Пути.Пути; } }
        public Floor CurrentFloor
        {
            get { return currentFloor; }
            set { currentFloor = value; OnPropertyChanged("CurrentFloor"); }
        }
        private Floor currentFloor;

        /// <summary>
        /// время эвакуации 
        /// </summary>
        public double EvacuationTime
        {
            get { return Пути.ВремяЭвакуации; }
        }
        public void AddFloor(Floor floor)
        {
            if (Floors.Contains(floor)) return;

            Floors.Add(floor);
            Objects.Add(floor);
            floor.ОбновитьНазвание();
            floor.Objects.CollectionChanged += ОбъектыЭтажаCollectionChanged;

            if (CurrentFloor == null) CurrentFloor = floor;
        }
        public void RemoveFloor(Floor floor)
        {
            if (!Floors.Contains(floor)) return;
            var индекс = Floors.IndexOf(floor);
            if (Floors.Count == 1) индекс = -1;
            else if (floor == Floors.LastOrDefault()) индекс--;


            Floors.Remove(floor);
            Objects.Remove(floor);
            floor.Objects.CollectionChanged -= ОбъектыЭтажаCollectionChanged;

            if (индекс < 0) CurrentFloor = null;
            else CurrentFloor = Floors[индекс];

            foreach (var x in Floors)
                x.ОбновитьНазвание();

            ПересчитатьПути();
        }
        public Floor ВыдатьЭтажНиже(Floor этаж)
        {
            var индекс = Floors.IndexOf(этаж);
            if (индекс < 1) return null;
            else return Floors[индекс - 1];
        }
        private void ОбъектыЭтажаCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var обьект in e.NewItems)
                    if (обьект is УчастокПути) ПересчитатьПути();

                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                ПересчитатьПути();
            }
        }
        private void ПересчитатьПути()
        {
            var участки = new List<Section>();
            foreach (var этаж in Floors)
                участки.AddRange(этаж.Objects.Where(x => x is Section).Select(x => x as Section));
            var стартовыеУчастки = участки.Where(x => x.NoIncoming);

            var пути = new List<Путь>();
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
                        var путь = new Путь(string.Format("Путь {0}", индекс++), this);
                        путь.ДобавитьУчастки(текущийПуть);
                        пути.Add(путь);

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
            Пути.Очистить();
            Пути.ДобавитьПути(пути);
        }
        public override BitmapImage Icon { get { return Настройки.Instance.ЗданиеIco; } }
    }
}
