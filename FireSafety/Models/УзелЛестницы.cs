using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    class УзелЛестницы : УзелПути
    {
        private static int индекс = 1;
        public УзелЛестницы(Floor parent, Point позиция) : this(parent, позиция, string.Format("Лестничный узел {0}", индекс++)) { }
        public УзелЛестницы(Floor parent, Point позиция, string название) : base(parent, позиция, название)
        {

            Значения = new БулевТип[] { new БулевТип(false), new БулевТип(true) };
            СвязьЭтажомНиже = Значения.FirstOrDefault(x => !x.Значение);
        }
        public override BitmapImage Icon { get { return Настройки.Instance.УзелIco; } }
        public БулевТип СвязьЭтажомНиже
        {
            get { return связьЭтажомНиже; }
            set
            {
                //   if (!СвязьЭтажомНижеАктивность) { OnPropertyChanged("СвязьЭтажомНижеАктивность"); return; }

                связьЭтажомНиже = value; OnPropertyChanged("СвязьЭтажомНиже");
                if (связьЭтажомНиже.Значение)
                    ParentFloor.ДобавитьСпуск(this);
            }
        }
        private БулевТип связьЭтажомНиже;

        public БулевТип[] Значения { get; private set; }
        public bool СвязьЭтажомНижеАктивность
        {
            get { return ParentFloor.НомерЭтажа != 1 && (СвязьЭтажомНиже.Значение || !OutgoingSections.Any()); }
        }
        public override void AddSection(Section участок)
        {
            base.AddSection(участок);
            OnPropertyChanged("СвязьЭтажомНижеАктивность");
        }
        public override void RemoveSection(Section участок)
        {
            base.RemoveSection(участок);
            OnPropertyChanged("СвязьЭтажомНижеАктивность");
        }
    }

    class БулевТип
    {
        public БулевТип(bool значение)
        {
            Значение = значение;
        }
        public bool Значение;
        public override string ToString()
        {
            return Значение ? "Да" : "Нет";
        }
    }
}
