using System.Windows;

namespace FireSafety.FireSafetyData
{
    /// <summary>
    /// Логика взаимодействия для FireRiskWindow.xaml
    /// </summary>
    public partial class FireRiskWindow : Window
    {
        public FireRiskWindow(double evacuationTime, double blockingTime, double square)
        {
            InitializeComponent();
            viewmodel = new FireRiskViewModel(evacuationTime, blockingTime, square);
            this.DataContext = viewmodel;
        }
        private FireRiskViewModel viewmodel;

        public double FireRiskValue { get { return viewmodel.ПожарныйРиск; } }
    }
}
