using FireSafety.Controls;
using System;
using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    /// <summary>
    /// Оболочка, позволяющая проводить манипуляции приближения / отдалния визуального представления
    /// </summary>
    class VisualShell : DrawingVisual, IMovable, IDisposable
    {
        public VisualFloor Floor { get; private set; }

        private EvacuationControl control;
        private Point AxisPoint { get { return new Point(control.ActualWidth / 2, control.ActualHeight / 2); } }
        public VisualShell(VisualFloor floor, EvacuationControl control)
        {
            this.Floor = floor;
            this.control = control;
            control.SizeChanged += ControlSizeChanged;
            this.Children.Add(floor);

            group = new TransformGroup();
            this.Transform = group;
            Inscribe();
        }

        private void ControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplyTranslateTransform(new Vector(e.NewSize.Width - e.PreviousSize.Width, e.NewSize.Height - e.PreviousSize.Height) / (2 * Ratio));
        }

        public void AddVisual(DrawingVisual visual)
        {
            this.Children.Add(visual);
        }

        public void RemoveVisual(DrawingVisual visual)
        {
            this.Children.Remove(visual);
        }

        private TransformGroup group;
        private TranslateTransform translateTransform;

        public double Ratio
        {
            get { return ratio; }
            private set { ratio = value; control.Ratio = Ratio; }
        }
        private double ratio;

        private double step = 0.1;

        private double minRatio = 0.3;
        private double maxRatio = 4;

        public void Inscribe()
        {
            group.Children.Clear();

            translateTransform = new TranslateTransform(0, 0);
            group.Children.Add(translateTransform);

            Ratio = Math.Min(
                 control.ActualWidth / Floor.ActualWidth,
                 control.ActualHeight / Floor.ActualHeight);

            ApplyTranslateTransform(control.AxisPoint - Floor.AxisPoint);
            ApplyScaleTransform(Ratio, control.AxisPoint);
        }

        private void ApplyScaleTransform(double ratio, Point axis)
        {
            var scaleTransform = new ScaleTransform();

            scaleTransform.CenterX = axis.X;
            scaleTransform.CenterY = axis.Y;
            scaleTransform.ScaleX = ratio;
            scaleTransform.ScaleY = ratio;
            group.Children.Add(scaleTransform);
        }

        private void ApplyTranslateTransform(Vector vector)
        {
            translateTransform.X += vector.X;
            translateTransform.Y += vector.Y;
        }

        /// <summary>
        /// Сместить на вектор
        /// </summary>
        /// <param name="vector"></param>
        public void Move(Vector vector)
        {
            ApplyTranslateTransform(vector);
        }

        /// <summary>
        /// Приблизить
        /// </summary>
        public void ToNearly(Point axis)
        {
            double newRatio = Ratio + step;
            if (newRatio >= maxRatio)
                newRatio = maxRatio;

            double value = newRatio / Ratio;
            if (value == 0) return;

            Ratio = newRatio;
            ApplyScaleTransform(value, axis);
        }

        /// <summary>
        /// Отдалить
        /// </summary>
        public void ToFar(Point axis)
        {
            double newRatio = Ratio - step;
            if (newRatio <= minRatio)
                newRatio = minRatio;

            double value = newRatio / Ratio;
            if (value == 0) return;

            Ratio = newRatio;
            ApplyScaleTransform(value, axis);
        }

        /// <summary>
        /// Преобразует в точку с абсолютными значениями
        /// </summary>
        public Point TransformPoint(Point point)
        {
            return Transform.Inverse.Transform(point);
        }

        /// <summary>
        /// Преобразует в вектор с абсолютными значениями
        /// </summary>
        public Vector TransformVector(Vector vector)
        {
            return vector / Ratio;
        }

        public double TransformValue(double value)
        {
            return value / Ratio;
        }

        public void Dispose()
        {
            Children.Clear();
        }
    }
}
