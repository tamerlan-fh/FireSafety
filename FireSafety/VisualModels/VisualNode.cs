using FireSafety.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    class VisualNode : VisualEntity
    {
        public Node NodeModel { get { return Model as Node; } }
        public VisualNode(Node node, Point position, VisualFloor parent) : base(node, position, parent)
        {
            Width = VisualRoadSection.Thickness + 5;
            Height = VisualRoadSection.Thickness + 5;

            IncomingSections = new List<VisualRoadSection>();
            OutgoingSections = new List<VisualRoadSection>();

            normalBrush = Brushes.LightGreen;
            selectBrush = normalBrush;
            normalPen.Brush = Brushes.Green;
            selectPen = normalPen;
            Draw();
        }
        public List<VisualRoadSection> IncomingSections { get; private set; }
        public List<VisualRoadSection> OutgoingSections { get; private set; }
        public void AddSection(VisualRoadSection section)
        {
            if (section.First == this)
                if (!OutgoingSections.Contains(section))
                    OutgoingSections.Add(section);
            if (section.Last == this)
                if (!IncomingSections.Contains(section))
                    IncomingSections.Add(section);
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
