using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    class CreatingLineInformation
    {
        public CreatingLineInformation(VisualUnit first) : this(first, new VisualThumb(first.Position)) { }
        public CreatingLineInformation(VisualUnit first, VisualUnit last)
        {
            FirstUnit = first;
            LastUnit = last;
            Drawing = new DrawingVisual();

            FirstUnit.PositionChanged += UnitPositionChanged;
            LastUnit.PositionChanged += UnitPositionChanged;
        }

        private void UnitPositionChanged(object sender, PositionChangedEventArgs e)
        {
            Draw();
        }

        public VisualNode FirstNode { get { return FirstUnit as VisualNode; } }
        public VisualUnit FirstUnit { get; private set; }
        public VisualUnit LastUnit { get; private set; }
        public Point FirstPosition
        {
            get { return FirstUnit.Position; }
            set { FirstUnit.Move(value - FirstPosition); }
        }
        public Point LastPosition
        {
            get { return LastUnit.Position; }
            set { LastUnit.Move(value - LastPosition); }
        }
        public double Length { get { return (LastPosition - FirstPosition).Length; } }
        public DrawingVisual Drawing { get; private set; }
        private void Draw()
        {
            using (var dc = Drawing.RenderOpen())
            {
                var radius = 10;
                var pen = new Pen(Brushes.Black, radius / 5);

                if (FirstUnit is VisualThumb)
                {
                    dc.DrawEllipse(null, pen, FirstPosition, radius, radius);
                    dc.DrawLine(pen, FirstPosition + new Vector(-radius, 0), FirstPosition + new Vector(radius, 0));
                    dc.DrawLine(pen, FirstPosition + new Vector(0, -radius), FirstPosition + new Vector(0, radius));
                }

                dc.DrawEllipse(null, pen, LastPosition, radius, radius);
                dc.DrawLine(pen, LastPosition + new Vector(-radius, 0), LastPosition + new Vector(radius, 0));
                dc.DrawLine(pen, LastPosition + new Vector(0, -radius), LastPosition + new Vector(0, radius));

                dc.DrawLine(new Pen(Brushes.Black, radius / 5) { DashStyle = new DashStyle(new double[] { radius / 2, radius / 2 }, radius / 2) }, FirstPosition, LastPosition);
            }
        }
    }
}
