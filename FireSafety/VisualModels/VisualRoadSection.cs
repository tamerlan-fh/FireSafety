using FireSafety.Models;
using System;
using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    class VisualRoadSection : VisualEntity
    {
        public VisualRoadSection(VisualNode first, VisualNode last, RoadSection section, VisualFloor floor) : base(section, floor)
        {
            this.First = first;
            this.Last = last;

            First.OutgoingSections.Add(this);
            Last.IncomingSections.Add(this);

            First.PositionChanged += NodePositionChanged;
            Last.PositionChanged += NodePositionChanged;

            normalPen.Brush = Brushes.Green;
            normalPen.Thickness = Thickness;

            selectPen.Brush = Brushes.Yellow;
            selectPen.Thickness = Thickness + 20;

            ApplyScale(ParentFloor.Model.Scale);
            Draw();
        }

        public const double Thickness = 20;

        private bool isMoving = false;

        private void NodePositionChanged(object sender, PositionChangedEventArgs e)
        {
            Draw();
            if (!isMoving)
                ApplyScale(ParentFloor.Model.Scale);
        }

        public override void Move(Vector shift)
        {
            isMoving = true;

            First.Move(shift);
            Last.Move(shift);

            isMoving = false;
        }

        public double Length { get { return (Last.Position - First.Position).Length; } }

        public VisualNode First { get; private set; }
        public VisualNode Last { get; private set; }
        public override EntityStatus Status
        {
            get { return base.Status; }

            set
            {
                First.Status = value;
                Last.Status = value;
                base.Status = value;
            }
        }
        public override bool Contains(Point point)
        {
            return GetUnit(point) != null;
        }

        public override VisualUnit GetUnit(Point position)
        {
            if (First.Contains(position)) return First;
            if (Last.Contains(position)) return Last;

            double angleA = Math.Abs(Vector.AngleBetween(position - First.Position, Last.Position - First.Position));
            if (angleA > 90) return null;

            double angleB = Math.Abs(Vector.AngleBetween(position - Last.Position, First.Position - Last.Position));
            if (angleB > 90) return null;

            double h = (First.Position - position).Length * Math.Sin(angleA * Math.PI / 180);
            if (h <= Thickness)
                return this;
            else
                return null;
        }

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
                dc.DrawDrawing(DrawTitle(First.Position + (Last.Position - First.Position) / 2));
            }
        }

        public override void ApplyScale(ZoomTool scale)
        {
            if (!Model.AutoSize) return;

            (Model as Section).Length = scale.TransformToActualLength(Length);
        }
    }
}
