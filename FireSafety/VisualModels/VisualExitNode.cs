using FireSafety.Models;
using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    class VisualExitNode : VisualNode
    {
        public VisualExitNode(ExitNode node,  VisualFloor floor) : base(node, floor)
        {
            Width = 60;
            Height = 60;

            normalBrush = Brushes.Orange;
            selectBrush = normalBrush;
            normalPen.Brush = Brushes.Red;
            selectPen = normalPen;
            Draw();
        }

        public override void Draw()
        {
            using (DrawingContext dc = RenderOpen())
            {
                var radiusX = Width / 2;
                var radiusY = Height / 2;
                var d = 10;

                if (Status == EntityStatus.Selected)
                {
                    dc.PushOpacity(0.5);
                    dc.DrawEllipse(selectBrush, null, Position, d + radiusX, d + radiusY);
                    dc.Pop();
                }
                dc.DrawEllipse(normalBrush, normalPen, Position, radiusX, radiusY);

                dc.DrawDrawing(DrawTitle(Position));
            }
        }
    }
}
