using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    /// <summary>
    /// лестничный узел
    /// </summary>
    class StairsNode : RoadNode
    {
        private static int index = 1;
        public StairsNode(Floor parent, Point position) : this(parent, position, string.Format("Лестничный узел {0}", index++)) { }
        public StairsNode(Floor parent, Point position, string title) : base(parent, position, title)
        {

            Значения = new БулевТип[] { new БулевТип(false), new БулевТип(true) };
            СвязьЭтажомНиже = Значения.FirstOrDefault(x => !x.Значение);
        }
        public override BitmapImage Icon { get { return Settings.Instance.УзелIco; } }
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
            get { return ParentFloor.FloorIndex != 1 && (СвязьЭтажомНиже.Значение || !OutgoingSections.Any()); }
        }
        public override void AddSection(Section section)
        {
            base.AddSection(section);
            OnPropertyChanged("СвязьЭтажомНижеАктивность");
        }
        public override void RemoveSection(Section section)
        {
            base.RemoveSection(section);
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
