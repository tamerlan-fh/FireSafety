using FireSafety.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    class VisualEntryNode : VisualNode
    {
        private List<VisualPoint> points;
        private VisualPoint topLeft;
        private VisualPoint topRight;
        private VisualPoint bottomLeft;
        private VisualPoint bottomRight;
        public VisualEntryNode(EntryNode node, Point position, VisualFloor floor) : base(node, position, floor)
        {
            points = new List<VisualPoint>();
            var dx = 60;
            var dy = 30;

            topLeft = new VisualPoint(position + new Vector(-dx, -dy)); AddPoint(topLeft);
            topRight = new VisualPoint(position + new Vector(dx, -dy)); AddPoint(topRight);
            bottomRight = new VisualPoint(position + new Vector(dx, dy)); AddPoint(bottomRight);
            bottomLeft = new VisualPoint(position + new Vector(-dx, dy)); AddPoint(bottomLeft);
            Dimensions = new Rect(bottomLeft.Position, topRight.Position);
            Draw();
        }

        public override Point Position
        {
            get
            {
                if (topLeft == null || bottomRight == null) return new Point();
                return NodeModel.Position;
            }
        }
        public override VisualUnit GetUnit(Point position)
        {
            foreach (var point in points)
                if (point.Contains(position)) return point;
            return this;
        }
        public override void Move(Vector shift)
        {
            OnPositionChanged(new NodePositionEventArgs(Position, shift));

            bottomLeft.Position += shift;
            topRight.Position += shift;
            bottomRight.Position += shift;
            topLeft.Position += shift;

            NodeModel.Position = topLeft.Position + (bottomRight.Position - topLeft.Position) / 2;
            Dimensions = new Rect(bottomLeft.Position, topRight.Position);
            Draw();
        }
        public override bool Contains(Point position)
        {
            foreach (var point in points)
                if (point.Contains(position)) return true;

            return Dimensions.Contains(position);
        }
        private void AddPoint(VisualPoint point)
        {
            points.Add(point);

            this.Children.Add(point);
            point.PositionChanged += NodePositionChanged;
        }
        private void NodePositionChanged(object sender, NodePositionEventArgs e)
        {
            var point = sender as VisualPoint;
            var shift = e.Shift;

            if (point == topLeft)
            {
                bottomLeft.Position += new Vector(shift.X, 0);
                topRight.Position += new Vector(0, shift.Y);
            }
            else if (point == topRight)
            {
                bottomRight.Position += new Vector(shift.X, 0);
                topLeft.Position += new Vector(0, shift.Y);
            }
            else if (point == bottomLeft)
            {
                topLeft.Position += new Vector(shift.X, 0);
                bottomRight.Position += new Vector(0, shift.Y);
            }
            else if (point == bottomRight)
            {
                topRight.Position += new Vector(shift.X, 0);
                bottomLeft.Position += new Vector(0, shift.Y);
            }
            OnPositionChanged(new NodePositionEventArgs(Position, new Vector(0, 0)));

            NodeModel.Position = topLeft.Position + (bottomRight.Position - topLeft.Position) / 2;
            Dimensions = new Rect(bottomLeft.Position, topRight.Position);
            Draw();
        }

        protected override void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.EntityPropertyChanged(sender, e);
            if (Model.IsSelected)
                foreach (var point in points) point.IsVisible = true;
            else
                foreach (var point in points) point.IsVisible = false;
        }
        public override void Draw()
        {
            if (topLeft == null || topRight == null || bottomLeft == null || bottomRight == null) return;

            using (DrawingContext dc = RenderOpen())
            {
                if (Status == EntityStatus.Selected)
                {
                    var d = 10;
                    var vector = new Vector(d, d);
                    dc.PushOpacity(0.5);
                    dc.DrawRectangle(selectBrush, null, new Rect(Dimensions.TopLeft - vector, Dimensions.BottomRight + vector));
                    dc.Pop();
                }

                dc.DrawRectangle(normalBrush, normalPen, Dimensions);
                dc.DrawDrawing(DrawTitle(Position));
            }
        }
    }
}
