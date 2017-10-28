using FireSafety.Models;
using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    class VisualStairsNode : VisualNode
    {
        public StairsNode StairsNode { get { return Model as StairsNode; } }

        public VisualStairsNode(Node node, VisualFloor floor) : base(node, floor)
        {
            normalBrush = Brushes.Orange;
            selectBrush = normalBrush;
            normalPen.Brush = Brushes.Brown;
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
                if (StairsNode.IsFloorsConnected)
                {
                    var v = new Vector(Width, Height);
                }
                dc.DrawDrawing(DrawTitle(Position));
            }
        }
    }
}
