using System.Windows;

namespace FireSafety.Models
{
    class УзелПути : Node
    {
        private static int индекс = 1;
        public УзелПути(Floor parent, Point позиция, string название) : base(parent, позиция, название) { }
        public УзелПути(Floor parent, Point позиция) : this(parent, позиция, string.Format("Узел {0}", индекс++)) { }
    }
}
