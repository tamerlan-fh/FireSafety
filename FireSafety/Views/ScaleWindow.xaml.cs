using FireSafety.Models;
using FireSafety.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;

namespace FireSafety.Views
{
    /// <summary>
    /// Логика взаимодействия для ScaleWindow.xaml
    /// </summary>
    public partial class ScaleWindow : Window
    {
        private ScaleWindowViewModel viewModel;
        public ScaleWindow(double pixelLength, ZoomTool oldScale)
        {
            InitializeComponent();
            viewModel = new ScaleWindowViewModel(pixelLength, oldScale);
            DataContext = viewModel;
            if (viewModel.CloseAction == null)
                viewModel.CloseAction = new Action(() => this.Close());
        }

        public ZoomTool Scale { get { return viewModel.Scale; } }
        protected override void OnClosing(CancelEventArgs e)
        {
            DialogResult = viewModel.DialogResult;
            base.OnClosing(e);
        }
    }
}
