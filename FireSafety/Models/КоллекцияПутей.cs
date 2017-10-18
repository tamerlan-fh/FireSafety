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
    class КоллекцияПутей : Entity
    {
        public ObservableCollection<Путь> Пути { get; private set; }
        public КоллекцияПутей(string название, Entity parent) : base(название, parent)
        {
            Пути = new ObservableCollection<Путь>();
        }

        public void Очистить()
        {
            Пути.Clear();
        }
        public void ДобавитьПуть(Путь путь)
        {
            Пути.Add(путь);
            путь.PropertyChanged += ПутьPropertyChanged;
        }

        private void ПутьPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is Путь))
                РассчитатьПараметрыПутей();
        }

        public void ДобавитьПути(IEnumerable<Путь> пути)
        {
            foreach (var путь in пути)
                ДобавитьПуть(путь);
            РассчитатьПараметрыПутей();
        }

        public bool ИмеютсяНесформированныеПути
        {
            get { return Пути.FirstOrDefault(x => !(x.Начало is StartNode && x.Конец is ExitNode)) != null; }
        }

        public bool ИмеютсяСформированныеПути
        {
            get { return Пути.FirstOrDefault(x => (x.Начало is StartNode && x.Конец is ExitNode)) != null; }
        }

        public double ВремяЭвакуации
        {
            get { return времяЭвакуации; }
            set { времяЭвакуации = value; OnPropertyChanged("ВремяЭвакуации"); }
        }
        private double времяЭвакуации;

    
        public void РассчитатьПараметрыПутей()
        {
            foreach (var путь in Пути)
                путь.Длина = путь.Участки.Sum(x => x.Length);

            var пути = Пути.Where(x => x.Сформированный);
            if (!пути.Any()) return;

            var рассматриваемыеУчастки = new List<ISection>();
            foreach (var путь in пути)
                рассматриваемыеУчастки.AddRange(путь.Участки);
            рассматриваемыеУчастки = рассматриваемыеУчастки.Distinct().ToList();

            var рассмотренныеУчастки = new List<ISection>();


            foreach (var путь in пути)
            {
                путь.ВремяДвижения = 0;

                var первыйУчасток = путь.Участки.First();
                var старт = путь.Начало as StartNode;
                первыйУчасток.DensityHumanFlow = старт.PeopleCount * старт.ProjectionArea / (первыйУчасток.Length * первыйУчасток.Width);

                if (первыйУчасток.GetType() == typeof(УчастокПути))
                {
                    первыйУчасток.IntensityHumanFlow = ТаблицаЗависимостиГоризонтальныйПуть.Instance.ИнтенсивностьЧерезПлотность(первыйУчасток.DensityHumanFlow);
                    первыйУчасток.Speed = ТаблицаЗависимостиГоризонтальныйПуть.Instance.СкоростьЧерезПлотность(первыйУчасток.DensityHumanFlow);
                }

                if (первыйУчасток.GetType() == typeof(УчастокЛестницы))
                {
                    первыйУчасток.Speed = ТаблицаЗависимостиЛестницаВниз.Instance.СкоростьЧерезПлотность(первыйУчасток.DensityHumanFlow);
                    первыйУчасток.IntensityHumanFlow = ТаблицаЗависимостиЛестницаВниз.Instance.Интенсивность(первыйУчасток.DensityHumanFlow);
                }

                первыйУчасток.MovementTime = первыйУчасток.Length / первыйУчасток.Speed;
                путь.ВремяДвижения += первыйУчасток.MovementTime;

                Section предыдущийУчасток = первыйУчасток;

                foreach (var участок in путь.Участки.Skip(1))
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
                        участок.MovementTime = старт.PeopleCount * 0.1 / (участок.IntensityHumanFlow * участок.Width);
                    }

                    путь.ВремяДвижения += участок.MovementTime;

                    предыдущийУчасток = участок;
                }
            }
            ВремяЭвакуации = Пути.Max(x => x.ВремяДвижения);
        }
        public override BitmapImage Icon { get { return Настройки.Instance.ПутьIco; } }
    }
}
