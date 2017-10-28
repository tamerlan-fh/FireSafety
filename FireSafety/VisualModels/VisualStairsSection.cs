using FireSafety.Models;
using System;
using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    class VisualStairsSection : VisualRoadSection
    {
        public VisualStairsSection(VisualNode first, VisualNode last, RoadSection section, VisualFloor floor) : base(first, last, section, floor)
        {
            normalPen.Brush = Brushes.RosyBrown;
            normalPen.Thickness = Thickness;

            selectPen.Brush = Brushes.Yellow;
            selectPen.Thickness = Thickness + 20;
        }

        private Pen stairsPen = new Pen(Brushes.Brown, 1);

        public override void Draw()
        {
            if (First == null || Last == null) return;
            using (DrawingContext dc = RenderOpen())
            {
                if (Status == EntityStatus.Selected)
                {
                    dc.PushOpacity(0.5);
                    dc.DrawLine(selectPen, First.Position, Last.Position);
                    dc.Pop();
                }
                dc.DrawLine(normalPen, First.Position, Last.Position);

                var step = Thickness / 2;
                var vector = Last.Position - First.Position;
                vector.Normalize();
                vector *= step;
                var normal = new Vector(vector.Y, -vector.X);

                int count = Convert.ToInt32((Last.Position - First.Position).Length / step);
                var point = First.Position;
                for (int i = 0; i < count; i++)
                {
                    point += vector;
                    dc.DrawLine(stairsPen, point + normal, point - normal);
                }


                dc.DrawDrawing(DrawTitle(First.Position + (Last.Position - First.Position) / 2));
            }
        }
    }
}
