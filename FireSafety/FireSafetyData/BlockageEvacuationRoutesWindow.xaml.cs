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
        private BlockageEvacuationRoutesViewModel БлокированиеПутейЭвакуации;
        public BlockageEvacuationRoutesWindow()
        {
            InitializeComponent();
            БлокированиеПутейЭвакуации = new BlockageEvacuationRoutesViewModel();
            this.DataContext = БлокированиеПутейЭвакуации;
        }

        public TimeSpan ВремяБлокирования { get { return БлокированиеПутейЭвакуации.ВремяБлокирования; } }
        protected override void OnClosing(CancelEventArgs e)
        {
            DialogResult = true;
            base.OnClosing(e);
        }
    }
}
