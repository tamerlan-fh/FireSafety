using System.Windows;

namespace FireSafety.Models
{
    /// <summary>
    /// узел пути
    /// </summary>
    class RoadNode : Node
    {
        private static int index = 1;
        public RoadNode(Floor parent, Point position, string title) : base(parent, position, title) { }
        public RoadNode(Floor parent, Point позиция) : this(parent, позиция, string.Format("Узел {0}", index++)) { }
    }
}
