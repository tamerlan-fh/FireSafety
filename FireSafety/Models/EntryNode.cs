using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    /// <summary>
    /// Дверной проем
    /// </summary>  
    class EntryNode : Node, ISection
    {
        
        private static int index = 1;
        public EntryNode(Floor parent, Point position) : this(parent, position, string.Format("Дверь {0}", index++)) { }
        public EntryNode(Floor parent, Point position, string title) : base(parent, position, title)
        {
            Width = 1;
            Length = 0;
        }

        public virtual double Length { get; private set; } 
        public virtual double Width
        {
            get { return width; }
            set { width = value; OnPropertyChanged("Width"); }
        }
        private double width;
        public double Area { get { return Length * Width; } }
        public override BitmapImage Icon { get { return Settings.Instance.EntryIco; } }
        public Node First { get; protected set; }
        public Node Last { get; protected set; }
        public bool NoIncoming { get { return !First.IncomingSections.Any(); } }
        public bool NoOutgoing { get { return !Last.OutgoingSections.Any(); } }
        public virtual double DensityHumanFlow
        {
            get { return densityHumanFlow; }
            set { densityHumanFlow = value; OnPropertyChanged("DensityHumanFlow"); }
        }
        private double densityHumanFlow;
        public virtual double IntensityHumanFlow
        {
            get { return intensityHumanFlow; }
            set { intensityHumanFlow = value; OnPropertyChanged("IntensityHumanFlow"); }
        }
        private double intensityHumanFlow;
        public virtual double MovementSpeed
        {
            get { return speed; }
            set { speed = value; OnPropertyChanged("MovementSpeed"); }
        }
        private double speed;
        public virtual double MovementTime
        {
            get { return movementTime; }
            set { movementTime = value; OnPropertyChanged("MovementTime"); }
        }
        private double movementTime;

        public void ApplyScale()
        {
            throw new NotImplementedException();
        }
    }
}
