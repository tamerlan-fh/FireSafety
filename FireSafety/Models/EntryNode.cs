using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    class EntryNode : Node, ISection
    {
        private static int index = 1;
        public EntryNode(Floor parent, Point position) : this(parent, position, string.Format("Дверь {0}", index++)) { }
        public EntryNode(Floor parent, Point position, string title) : base(parent, position, title)
        {
            //  ДобавитьУчасток(this);
        }

        public virtual double Length
        {
            get { return length; }
            set { length = value; OnPropertyChanged("Длина"); }
        }
        private double length;
        public virtual double Width
        {
            get { return width; }
            set { width = value; OnPropertyChanged("Ширина"); }
        }
        private double width;
        public double Area { get { return Length * Width; } }  
        public override BitmapImage Icon { get { return Настройки.Instance.ДверьIco; } }
        public Node First { get; protected set; }
        public Node Last { get; protected set; }
        public bool NoIncoming { get { return !First.IncomingSections.Any(); } }
        public bool NoOutgoing { get { return !Last.OutgoingSections.Any(); } }
        public virtual double DensityHumanFlow
        {
            get { return densityHumanFlow; }
            set { densityHumanFlow = value; OnPropertyChanged("ПлотностьЛюдскогоПотока"); }
        }
        private double densityHumanFlow;
        public virtual double IntensityHumanFlow
        {
            get { return intensityHumanFlow; }
            set { intensityHumanFlow = value; OnPropertyChanged("ИнтенсивностьЛюдскогоПотока"); }
        }
        private double intensityHumanFlow;

        public virtual double Speed
        {
            get { return speed; }
            set { speed = value; OnPropertyChanged("СкоростьДвижения"); }
        }
        private double speed;

        public virtual double MovementTime
        {
            get { return movementTime; }
            set { movementTime = value; OnPropertyChanged("ВремяДвижения"); }
        }
        private double movementTime;
    }
}
