using System.Linq;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    /// <summary>
    /// Абстрактный класс единица участка пути
    /// </summary>
    abstract class Section : Entity, ISection
    {
        public Section(string title, Entity parent) : base(title, parent) { }

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
        public Node First { get; protected set; }
        public Node Last { get; protected set; }
        public bool NoIncoming { get { return !First.IncomingSections.Any(); } }
        public bool NoOutgoing { get { return !Last.OutgoingSections.Any(); } }

        /// <summary>
        /// Плотность людского потока
        /// </summary>
        public virtual double DensityHumanFlow
        {
            get { return densityHumanFlow; }
            set { densityHumanFlow = value; OnPropertyChanged("ПлотностьЛюдскогоПотока"); }

        }
        private double densityHumanFlow;

        /// <summary>
        /// Интенсивность людского потока
        /// </summary>
        public virtual double IntensityHumanFlow
        {
            get { return intensityHumanFlow; }
            set { intensityHumanFlow = value; OnPropertyChanged("ИнтенсивностьЛюдскогоПотока"); }
        }
        private double intensityHumanFlow;

        /// <summary>
        /// скорость людского потока
        /// </summary>
        public virtual double Speed
        {
            get { return speed; }
            set { speed = value; OnPropertyChanged("СкоростьДвижения"); }
        }
        private double speed;

        /// <summary>
        /// Время движения
        /// </summary>
        public virtual double MovementTime
        {
            get { return movementTime; }
            set { movementTime = value; OnPropertyChanged("ВремяДвижения"); }
        }
        private double movementTime;

        public override BitmapImage Icon { get { return Настройки.Instance.УчастокПутиIco; } }
    }
}
