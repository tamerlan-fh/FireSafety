using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    abstract class Node : Entity
    {
        /// <summary>
        /// Этаж, породивший объект ParentFloor
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
            set { position = value; OnPropertyChanged("Позиция"); }
        }
        private Point position;
        public override BitmapImage Icon { get { return Настройки.Instance.УзелIco; } }
        public virtual List<Section> IncomingSections { get; protected set; }
        public virtual List<Section> OutgoingSections { get; protected set; }
        public bool IncomingSectionsAllowed { get; set; }
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
