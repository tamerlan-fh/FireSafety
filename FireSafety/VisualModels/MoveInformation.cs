using System.Windows;

namespace FireSafety.VisualModels
{
    class MoveInformation
    {
        public MoveInformation(VisualUnit unit, Point position)
        {
            this.Unit = unit;
            this.StartPosition = position;
        }

        public VisualUnit Unit { get; private set; }
        public Point StartPosition { get; set; }
    }
}
