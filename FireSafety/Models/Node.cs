using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    abstract class Node : Entity
    {
        /// <summary>
        /// Этаж, породивший объект 
        /// </summary>
        public Floor ParentFloor { get { return Parent as Floor; } }
        public Node(Floor parent, Point position, string title) : base(title, parent)
        {
            Position = position;
            IncomingSectionsAllowed = true;
            OutgoingSectionsAllowed = true;
            IncomingSections = new List<Section>();
            OutgoingSections = new List<Section>();
        }
        public Point Position
        {
            get { return position; }
            set { position = value; OnPropertyChanged("Position"); }
        }
        private Point position;
        public override BitmapImage Icon { get { return Settings.Instance.УзелIco; } }

        /// <summary>
        /// Входящие 
        /// </summary>
        public virtual List<Section> IncomingSections { get; protected set; }
        /// <summary>
        /// Исходящие
        /// </summary>
        public virtual List<Section> OutgoingSections { get; protected set; }

        /// <summary>
        /// Входящие разрешены
        /// </summary>
        public bool IncomingSectionsAllowed { get; set; }

        /// <summary>
        /// Исходящие разрешены
        /// </summary>
        public bool OutgoingSectionsAllowed { get; set; }

        /// <summary>
        /// Является несвязанным
        /// </summary>
        public bool IsUnrelated
        {
            get { return !(IncomingSections.Any() || OutgoingSections.Any()); }
        }
        public virtual void AddSection(Section section)
        {
            if (section.First == this)
                if (!OutgoingSections.Contains(section))
                    OutgoingSections.Add(section);
            if (section.Last == this)
                if (!IncomingSections.Contains(section))
                    IncomingSections.Add(section);
        }
        public virtual void RemoveSection(Section section)
        {
            if (section.First == this)
                OutgoingSections.Remove(section);
            if (section.Last == this)
                IncomingSections.Remove(section);
        }
    }
}
