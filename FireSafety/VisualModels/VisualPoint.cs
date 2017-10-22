using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    class VisualPoint : VisualUnit
    {
        private const double radius = 6;
        public VisualPoint(Point position)
        {
            Width = 2 * radius;
            Height = 2 * radius;
            Position = position;
            normalBrush = Brushes.White;
            Draw();
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (isVisible == value) return;
                isVisible = value; Draw();
            }
        }
        private bool isVisible;
        public override VisualUnit GetUnit(Point позиция)
        {
            if (IsVisible)
                return base.GetUnit(позиция);
            else return null;
        }
        public override bool Contains(Point point)
        {
            if (IsVisible)
                return base.Contains(point);
            else
                return false;
        }
        public override void Draw()
        {
            using (DrawingContext dc = RenderOpen())
            {
                if (IsVisible)
                    dc.DrawRectangle(normalBrush, normalPen, Dimensions);
            }
        }
    }
}
