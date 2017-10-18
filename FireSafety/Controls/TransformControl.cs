using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FireSafety.Controls
{
    public class TransformControl : ContentControl
    {
        static TransformControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransformControl), new FrameworkPropertyMetadata(typeof(TransformControl)));

        }

        #region Dependency properties

        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor",
            typeof(double), typeof(TransformControl),
            new FrameworkPropertyMetadata(1.4d, ZoomFactorChanged));
        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }
        private static void ZoomFactorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if ((double)e.NewValue == 0.0d)
                o.SetValue(ZoomFactorProperty, 1.0d);
        }

        public static readonly DependencyProperty IsPanEnabledProperty =
            DependencyProperty.Register("IsPanEnabled",
            typeof(bool), typeof(TransformControl),
            new FrameworkPropertyMetadata(true));
        public bool IsPanEnabled
        {
            get { return (bool)GetValue(IsPanEnabledProperty); }
            set { SetValue(IsPanEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsCropEnabledProperty =
            DependencyProperty.Register("IsCropEnabled",
            typeof(bool), typeof(TransformControl),
            new FrameworkPropertyMetadata(true));
        public bool IsCropEnabled
        {
            get { return (bool)GetValue(IsCropEnabledProperty); }
            set { SetValue(IsCropEnabledProperty, value); }
        }

        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle",
            typeof(double), typeof(TransformControl),
            new FrameworkPropertyMetadata(0.0d, RotationAngleChanged));
        public double RotationAngle
        {
            get { return (double)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }
        private static void RotationAngleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((TransformControl)o).DoRotate((double)e.NewValue);
        }

        public static readonly DependencyProperty RotationCenterProperty =
            DependencyProperty.Register("RotationCenter",
            typeof(Point), typeof(TransformControl),
            new FrameworkPropertyMetadata(new Point(0, 0)));
        public Point RotationCenter
        {
            get { return (Point)GetValue(RotationCenterProperty); }
            set { SetValue(RotationCenterProperty, value); }
        }

        public static readonly DependencyProperty ZoomScaleProperty =
            DependencyProperty.Register("ZoomScale",
            typeof(double), typeof(TransformControl),
            new FrameworkPropertyMetadata(1.0d, ZoomScaleChanged));
        public double ZoomScale
        {
            get { return (double)GetValue(ZoomScaleProperty); }
            set { SetValue(ZoomScaleProperty, value); }
        }
        private static void ZoomScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = ((TransformControl)o);

            var newValue = Math.Min(Math.Max(control.MinZoomScale, (double)e.NewValue), control.MaxZoomScale);
            if (newValue != (double)e.NewValue)
            {
                control.ZoomScale = newValue;
            }
            if (newValue != (double)e.OldValue)
            {
                control.DoZoom(newValue);
            }
        }

        public static readonly DependencyProperty MaxZoomScaleProperty =
            DependencyProperty.Register("MaxZoomScale",
            typeof(double), typeof(TransformControl),
            new FrameworkPropertyMetadata(1.0d));
        public double MaxZoomScale
        {
            get { return (double)GetValue(MaxZoomScaleProperty); }
            set { SetValue(MaxZoomScaleProperty, value); }
        }

        public static readonly DependencyProperty MinZoomScaleProperty =
            DependencyProperty.Register("MinZoomScale",
            typeof(double), typeof(TransformControl),
            new FrameworkPropertyMetadata(1.0d));
        public double MinZoomScale
        {
            get { return (double)GetValue(MinZoomScaleProperty); }
            set { SetValue(MinZoomScaleProperty, value); }
        }

        public static readonly DependencyProperty ZoomCenterProperty =
            DependencyProperty.Register("ZoomCenter",
            typeof(Point), typeof(TransformControl),
            new FrameworkPropertyMetadata(new Point(0.0d, 0.0d)));
        public Point ZoomCenter
        {
            get { return (Point)GetValue(ZoomCenterProperty); }
            set { SetValue(ZoomCenterProperty, value); }
        }

        private static readonly DependencyPropertyKey ControlCenterPropertyKey =
            DependencyProperty.RegisterReadOnly("ControlCenter",
            typeof(Point), typeof(TransformControl),
            new FrameworkPropertyMetadata(new Point(0, 0)));
        public static readonly DependencyProperty ControlCenterProperty =
            ControlCenterPropertyKey.DependencyProperty;
        public Point ControlCenter
        {
            get { return (Point)GetValue(ControlCenterProperty); }
            private set { SetValue(ControlCenterPropertyKey, value); }
        }

        #endregion

        #region protected overrides

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.Handled) return;

            _startMousePosition = Mouse.GetPosition(this);
            Mouse.Capture(this, CaptureMode.Element);
            e.Handled = true;
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Mouse.Captured != null && Mouse.Captured.Equals(this))
            {
                _currMousePosition = e.GetPosition(this);
                if (IsPanEnabled) DoMove(_currMousePosition - _startMousePosition);
                _startMousePosition = _currMousePosition;
                e.Handled = true;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            Mouse.Capture(this, CaptureMode.None);
            e.Handled = true;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            double delta = Math.Sign(e.Delta) > 0 ? ZoomFactor : 1 / ZoomFactor;

            ZoomCenter = Mouse.GetPosition(this);
            ZoomScale *= delta;

            e.Handled = true;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            ControlCenter = new Point(ActualWidth / 2, ActualHeight / 2);
        }

        #endregion

        public override void OnApplyTemplate()
        {
            _rotateControl = (FrameworkElement)GetTemplateChild("PART_RotateControl");
            _rotateTransform = new RotateTransform(0.0d);
            _rotateControl.RenderTransform = _rotateTransform;

            _zoomControl = (FrameworkElement)GetTemplateChild("PART_ZoomControl");
            TransformGroup group = new TransformGroup();
            _zoomControl.RenderTransform = group;
            _zoomTransform = new ScaleTransform(1.0d, 1.0d, 0.0d, 0.0d);
            _moveTransform = new TranslateTransform(0.0d, 0.0d);
            group.Children.Add(_zoomTransform);
            group.Children.Add(_moveTransform);
        }

        private void DoRotate(double newAngle)
        {
            Point center = TranslatePoint(RotationCenter, _rotateControl);
            _rotateTransform.Angle = newAngle;
            DoMove(RotationCenter - _rotateControl.TranslatePoint(center, this));
        }

        private void DoZoom(double newZoom)
        {
            //var диагональ = new Vector(_rotateControl.Width, _rotateControl.Height) / 2;
            //ZoomCenter = _rotateControl.
            Point center = TranslatePoint(ZoomCenter, _zoomControl);
            _zoomTransform.ScaleX = _zoomTransform.ScaleY = newZoom;
            DoMove(ZoomCenter - _zoomControl.TranslatePoint(center, this));
        }

        private void DoMove(Vector vector)
        {
            _moveTransform.X += vector.X;
            _moveTransform.Y += vector.Y;
        }

        private Point _startMousePosition;
        private Point _currMousePosition;

        private TranslateTransform _moveTransform;
        private RotateTransform _rotateTransform;
        private ScaleTransform _zoomTransform;

        private FrameworkElement _rotateControl;
        private FrameworkElement _zoomControl;
    }
}
