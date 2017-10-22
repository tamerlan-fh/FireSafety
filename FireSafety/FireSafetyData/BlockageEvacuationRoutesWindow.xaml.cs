using System;
using System.ComponentModel;
using System.Windows;

namespace FireSafety.FireSafetyData
{
    /// <summary>
    /// Логика взаимодействия для BlockageEvacuationRoutesWindow.xaml
    /// </summary>
    public partial class BlockageEvacuationRoutesWindow : Window
    {
        private BlockageEvacuationRoutesViewModel BlockageEvacuationRoutes;
        public BlockageEvacuationRoutesWindow()
        {
            InitializeComponent();
            BlockageEvacuationRoutes = new BlockageEvacuationRoutesViewModel();
            this.DataContext = BlockageEvacuationRoutes;
        }

        public TimeSpan BlockageEvacuationRoutesTime { get { return BlockageEvacuationRoutes.ВремяБлокирования; } }
        protected override void OnClosing(CancelEventArgs e)
        {
            DialogResult = true;
            base.OnClosing(e);
        }
    }
}
