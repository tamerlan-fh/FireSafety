using System.Linq;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    /// <summary>
    /// Абстрактный класс единица участка пути
    /// </summary>

    abstract class Section : Entity, ISection, IScalable
    {
        public Section(string title, Entity parent) : base(title, parent) { }

        public virtual double Length
        {
            get { return length; }
            set { length = value; OnPropertyChanged("Length"); }
        }
        private double length;
        public virtual double Width
        {
            get { return width; }
            set { width = value; OnPropertyChanged("Width"); }
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
            set { densityHumanFlow = value; OnPropertyChanged("DensityHumanFlow"); }

        }
        private double densityHumanFlow;

        /// <summary>
        /// Интенсивность людского потока
        /// </summary>
        public virtual double IntensityHumanFlow
        {
            get { return intensityHumanFlow; }
            set { intensityHumanFlow = value; OnPropertyChanged("IntensityHumanFlow"); }
        }
        private double intensityHumanFlow;

        /// <summary>
        /// скорость людского потока
        /// </summary>
        public virtual double MovementSpeed
        {
            get { return movementSpeed; }
            set { movementSpeed = value; OnPropertyChanged("MovementSpeed"); }
        }
        private double movementSpeed;

        /// <summary>
        /// Время движения
        /// </summary>
        public virtual double MovementTime
        {
            get { return movementTime; }
            set { movementTime = value; OnPropertyChanged("MovementTime"); }
        }
        private double movementTime;

        public bool AutoSize
        {
            get { return autoSize; }
            set { autoSize = value; OnPropertyChanged("AutoSize"); }
        }
        private bool autoSize = true;

        public override BitmapImage Icon { get { return Settings.Instance.УчастокПутиIco; } }
    }
}
