using FireSafety.Models;
using System;
using System.Windows.Input;

namespace FireSafety.ViewModels
{
    class ScaleWindowViewModel : BasePropertyChanged
    {
        public Action CloseAction { get; set; }
        public ICommand CanselCommand { get; protected set; }
        public ICommand ApplyCommand { get; protected set; }

        public ScaleWindowViewModel(double pixelLength, ZoomTool oldScale)
        {
            this.PixelLength = pixelLength;
            this.oldScale = oldScale;
            FactLength = oldScale.GetActualLength(PixelLength);
            ApplyCommand = new RelayCommand(p => Apply(), p => CanApply());
            CanselCommand = new RelayCommand(p => Cansel());
        }

        public bool? DialogResult { get; private set; }

        private ZoomTool oldScale;
        public ZoomTool Scale { get; private set; }
        private void Apply()
        {
            Scale = new ZoomTool(FactLength, PixelLength);
            DialogResult = true;
            CloseAction();
        }
        private bool CanApply()
        {
            return FactLength > 0;
        }

        private void Cansel()
        {
            DialogResult = false;
            CloseAction();
        }

        public double PixelLength { get; private set; }

        public double FactLength
        {
            get { return factLength; }
            set { factLength = value; OnPropertyChanged("FactLength"); }
        }
        private double factLength;
    }
}
