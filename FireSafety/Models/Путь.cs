using FireSafety.FireSafetyData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    class Путь : Entity
    {
        private static int индекс = 1;
        public List<Section> Участки { get; private set; }
        public Путь(Building parent) : this(string.Format("Путь {0}", индекс++), parent) { }
        public Путь(string название, Building parent) : base(название, parent)
        {
            Участки = new List<Section>();
        }

        public Node Начало
        {
            get { return Участки.FirstOrDefault().First; }
        }
        public Node Конец
        {
            get { return Участки.LastOrDefault().Last; }
        }
        public void ДобавитьУчастки(IEnumerable<Section> участки)
        {
            foreach (var участок in участки)
                ДобавитьУчасток(участок);
        }
        public void ДобавитьУчасток(Section участок)
        {
            if (Участки.Contains(участок)) return;
            if (Участки.Any())
                if (Участки.LastOrDefault().Last != участок.First) throw new Exception("Не верно составлен путь!");
            Участки.Add(участок);
            (участок as Entity).PropertyChanged += УчастокPropertyChanged;

            //if (участок.Конец is Дверь && участок.GetType() != typeof(УчастокПутиДверь))
            //    ДобавитьУчасток(new УчастокПутиДверь(участок.Конец as Дверь));

            //if (участок.Начало is Старт || участок.Начало is Дверь) участок.Начало.PropertyChanged += УчастокPropertyChanged;
            //if (участок.Конец is Дверь) участок.Начало.PropertyChanged += УчастокPropertyChanged;
            if (участок.First is StartNode) участок.First.PropertyChanged += УчастокPropertyChanged;
            // РассчитатьПараметры();
        }

        private void УчастокPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //switch (e.PropertyName)
            //{
            //    case "Ширина":
            //    case "ЧислоЛюдей":
            //    case "Длина": РассчитатьПараметры(); break;
            //}
            switch (e.PropertyName)
            {
                case "Ширина":
                case "ЧислоЛюдей":
                case "Длина": OnPropertyChanged(sender, e.PropertyName); break;
            }
        }
        public double ВремяДвижения
        {
            get { return времяДвижения; }
            set { времяДвижения = value; OnPropertyChanged("ВремяДвижения"); }
        }
        private double времяДвижения;
        public double Длина
        {
            get { return длина; }
            set { длина = value; OnPropertyChanged("Длина"); }
        }
        private double длина;
        public bool Сформированный
        {
            get { return (Начало is StartNode && Конец is ExitNode); }
        }

        public void РассчитатьПараметры_old()
        {
            Длина = Участки.Sum(x => x.Length);
            if (!Сформированный) return;

            ВремяДвижения = 0;

            var первыйУчасток = Участки.First();
            var N = (Начало as StartNode).PeopleCount;
            первыйУчасток.DensityHumanFlow = N * 0.1 / первыйУчасток.Area;
            double предыдущаяИнтенсивность;
            double предыдущаяШирина;

            if (первыйУчасток.GetType() == typeof(УчастокЛестницы))
            {
                первыйУчасток.Speed = ТаблицаЗависимостиЛестницаВниз.Instance.СкоростьЧерезПлотность(первыйУчасток.DensityHumanFlow);
                первыйУчасток.IntensityHumanFlow = ТаблицаЗависимостиЛестницаВниз.Instance.Интенсивность(первыйУчасток.DensityHumanFlow);
            }
            if (первыйУчасток.GetType() == typeof(УчастокПути))
            {
                первыйУчасток.Speed = ТаблицаЗависимостиГоризонтальныйПуть.Instance.СкоростьЧерезПлотность(первыйУчасток.DensityHumanFlow);
                первыйУчасток.IntensityHumanFlow = ТаблицаЗависимостиГоризонтальныйПуть.Instance.ИнтенсивностьЧерезПлотность(первыйУчасток.DensityHumanFlow);
            }
            первыйУчасток.MovementTime = первыйУчасток.Length / первыйУчасток.Speed;
            ВремяДвижения += первыйУчасток.MovementTime;
            предыдущаяИнтенсивность = первыйУчасток.IntensityHumanFlow;
            предыдущаяШирина = первыйУчасток.Width;

            if (первыйУчасток.Last is EntryNode)
            {
                var дверь = первыйУчасток.Last as EntryNode;
                дверь.IntensityHumanFlow = 2.5 + 3.75 * дверь.Width;
                дверь.MovementTime = N * 0.1 / (дверь.IntensityHumanFlow * дверь.Width);
                ВремяДвижения += дверь.MovementTime;
                предыдущаяИнтенсивность = дверь.IntensityHumanFlow;
                предыдущаяШирина = дверь.Width;
            }


            foreach (var участок in Участки.Skip(1))
            {
                if (участок is Спуск) continue;

                участок.IntensityHumanFlow = предыдущаяИнтенсивность * предыдущаяШирина / участок.Width;

                if (первыйУчасток.GetType() == typeof(УчастокПути))
                    участок.Speed = ТаблицаЗависимостиГоризонтальныйПуть.Instance.СкоростьЧерезИнтенсивность(участок.IntensityHumanFlow);
                if (первыйУчасток.GetType() == typeof(УчастокЛестницы))
                    участок.Speed = ТаблицаЗависимостиГоризонтальныйПуть.Instance.СкоростьЧерезИнтенсивность(участок.IntensityHumanFlow);
                участок.MovementTime = участок.Length / участок.Speed;
                ВремяДвижения += участок.MovementTime;
                предыдущаяИнтенсивность = участок.IntensityHumanFlow;
                предыдущаяШирина = участок.Width;

                if (участок.Last is EntryNode)
                {
                    var дверь = участок.Last as EntryNode;
                    дверь.IntensityHumanFlow = 2.5 + 3.75 * дверь.Width;
                    дверь.MovementTime = N * 0.1 / (дверь.IntensityHumanFlow * дверь.Width);
                    ВремяДвижения += дверь.MovementTime;
                    предыдущаяИнтенсивность = дверь.IntensityHumanFlow;
                    предыдущаяШирина = дверь.Width;
                }
            }
        }
        public void РассчитатьПараметры()
        {
            Длина = Участки.Sum(x => x.Length);
            if (!Сформированный) return;

            ВремяДвижения = 0;

            var первыйУчасток = Участки.First();
            var N = (Начало as StartNode).PeopleCount;
            первыйУчасток.DensityHumanFlow = N * 0.1 / первыйУчасток.Area;
            Section предыдущийУчасток = первыйУчасток;

            if (первыйУчасток.GetType() == typeof(УчастокЛестницы))
            {
                первыйУчасток.Speed = ТаблицаЗависимостиЛестницаВниз.Instance.СкоростьЧерезПлотность(первыйУчасток.DensityHumanFlow);
                первыйУчасток.IntensityHumanFlow = ТаблицаЗависимостиЛестницаВниз.Instance.Интенсивность(первыйУчасток.DensityHumanFlow);
            }
            if (первыйУчасток.GetType() == typeof(УчастокПути))
            {
                первыйУчасток.Speed = ТаблицаЗависимостиГоризонтальныйПуть.Instance.СкоростьЧерезПлотность(первыйУчасток.DensityHumanFlow);
                первыйУчасток.IntensityHumanFlow = ТаблицаЗависимостиГоризонтальныйПуть.Instance.ИнтенсивностьЧерезПлотность(первыйУчасток.DensityHumanFlow);
            }
            первыйУчасток.MovementTime = первыйУчасток.Length / первыйУчасток.Speed;
            ВремяДвижения += первыйУчасток.MovementTime;

            //if (первыйУчасток.Конец is Дверь)
            //{
            //    var дверь = первыйУчасток.Конец as Дверь;
            //    дверь.ИнтенсивностьЛюдскогоПотока = 2.5 + 3.75 * дверь.Ширина;
            //    дверь.ВремяДвижения = N * 0.1 / (дверь.ИнтенсивностьЛюдскогоПотока * дверь.Ширина);
            //    ВремяДвижения += дверь.ВремяДвижения;
            //    предыдущаяИнтенсивность = дверь.ИнтенсивностьЛюдскогоПотока;
            //    предыдущаяШирина = дверь.Ширина;
            //}


            foreach (var участок in Участки.Skip(1))
            {
                if (участок is Спуск) continue;

                if (участок.GetType() == typeof(УчастокПути))
                {
                    участок.IntensityHumanFlow = предыдущийУчасток.IntensityHumanFlow * предыдущийУчасток.Width / участок.Width;
                    участок.Speed = ТаблицаЗависимостиГоризонтальныйПуть.Instance.СкоростьЧерезИнтенсивность(участок.IntensityHumanFlow);
                    участок.MovementTime = участок.Length / участок.Speed;
                }
                if (участок.GetType() == typeof(УчастокЛестницы))
                {
                    участок.IntensityHumanFlow = предыдущийУчасток.IntensityHumanFlow * предыдущийУчасток.Width / участок.Width;
                    участок.Speed = ТаблицаЗависимостиГоризонтальныйПуть.Instance.СкоростьЧерезИнтенсивность(участок.IntensityHumanFlow);
                    участок.MovementTime = участок.Length / участок.Speed;
                }
                if (участок.GetType() == typeof(EntryNode))
                {
                    участок.IntensityHumanFlow = 2.5 + 3.75 * участок.Width;
                    участок.MovementTime = N * 0.1 / (участок.IntensityHumanFlow * участок.Width);
                }

                ВремяДвижения += участок.MovementTime;

                предыдущийУчасток = участок;
            }
        }
        public override ObjectStatus Status
        {
            get { return base.Status; }

            set
            {
                if (Status == value) return;
                base.Status = value;
                foreach (var участок in Участки)
                    (участок as Entity).Status = value;
            }
        }
        public override BitmapImage Icon { get { return Настройки.Instance.ПутьIco; } }
    }
}
