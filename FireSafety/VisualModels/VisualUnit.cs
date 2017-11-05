using System;
using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    abstract class VisualUnit : DrawingVisual, IMovable
    {
        public VisualUnit()
        {
            var radius = 30;
            Width = 2 * radius;
            Height = 2 * radius;
        }

        public double Width
        {
            get { return width; }
            protected set { width = value; Diagonal = new Vector(Width, Height); }
        }
        private double width;

        public double Height
        {
            get { return height; }
            protected set { height = value; Diagonal = new Vector(Width, Height); }
        }
        private double height;
        public virtual Point Position
        {
            get { return position; }
            set
            {
                position = value;
                Dimensions = new Rect(Position - 0.5 * Diagonal, Position + 0.5 * Diagonal);
                Draw();
            }
        }
        private Point position;
        public Vector Diagonal { get; protected set; }
        public virtual Rect Dimensions { get; protected set; }
        public abstract void Draw();

        protected Brush normalBrush = Brushes.Gray;
        protected Pen normalPen = new Pen(Brushes.Black, 2);

        protected Brush selectBrush = Brushes.LightGray;
        protected Pen selectPen = new Pen(Brushes.Black, 2);

        public virtual VisualUnit GetUnit(Point position)
        {
            return this;
        }

        public virtual void Move(Vector shift)
        {
            OnPositionChanged(new PositionChangedEventArgs(Position, shift));
            Position += shift;
        }

        public virtual bool Contains(Point point)
        {
            return Dimensions.Contains(point);
        }

        public event EventHandler<PositionChangedEventArgs> PositionChanged;
        protected virtual void OnPositionChanged(PositionChangedEventArgs e)
        {
            PositionChanged?.Invoke(this, e);
        }
    }
}
