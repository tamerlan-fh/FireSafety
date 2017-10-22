using System.Windows;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    class StartNode : Node
    {
        private static int index = 1;
        public StartNode(Floor parent, Point position) : this(parent, position, string.Format("Группа {0}", index++)) { }

        public StartNode(Floor parent, Point position, string title) : base(parent, position, title)
        {
            IncomingSectionsAllowed = false;
            ProjectionArea = 0.125;
            PeopleCount = 1;
        }
        public override BitmapImage Icon { get { return Settings.Instance.СтартIco; } }

        /// <summary>
        /// Число людей 
        /// </summary>
        public int PeopleCount
        {
            get { return peopleCount; }
            set { peopleCount = value; OnPropertyChanged("PeopleCount"); }
        }
        private int peopleCount;

        /// <summary>
        /// Площадь проекции человека 
        /// </summary>
        public double ProjectionArea
        {
            get { return projectionArea; }
            set { projectionArea = value; OnPropertyChanged("ProjectionArea"); }
        }
        private double projectionArea;
    }
}
