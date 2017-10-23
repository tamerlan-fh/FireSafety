using FireSafety.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FireSafety.FireSafetyData
{
    class FireRiskViewModel : BasePropertyChanged
    {
        public List<ПожарныйРиск> ВариантыПожарногоРиска { get { return ТаблицаПожарныйРиск.Instance.Значения; } }

        public ПожарныйРиск ВыбранныйПожарныйРиск
        {
            get { return выбранныйПожарныйРиск; }
            set { выбранныйПожарныйРиск = value; OnPropertyChanged("ВыбранныйПожарныйРиск"); }
        }
        private ПожарныйРиск выбранныйПожарныйРиск;

        public double ВремяНахождения
        {
            get { return времяНахождения; }
            set { времяНахождения = value; OnPropertyChanged("ВремяНахождения"); }
        }
        private double времяНахождения;

        public bool УстановкиПожарутшения
        {
            get { return установкиПожарутшения; }
            set { установкиПожарутшения = value; OnPropertyChanged("УстановкиПожарутшения"); }
        }
        private bool установкиПожарутшения;

        public bool СистемыПожарнойСигнализации
        {
            get { return системыПожарнойСигнализации; }
            set { системыПожарнойСигнализации = value; OnPropertyChanged("СистемыПожарнойСигнализации"); }
        }
        private bool системыПожарнойСигнализации;

        public bool СистемыОповещения
        {
            get { return системыОповещения; }
            set { системыОповещения = value; OnPropertyChanged("СистемыОповещения"); }
        }
        private bool системыОповещения;

        public bool СистемыПротиводымнойЗащиты
        {
            get { return системыПротиводымнойЗащиты; }
            set { системыПротиводымнойЗащиты = value; OnPropertyChanged("СистемыПротиводымнойЗащиты"); }
        }
        private bool системыПротиводымнойЗащиты;

        public double РасчетноеВремяЭвакуации
        {
            get { return расчетноеВремяЭвакуации; }
            set { расчетноеВремяЭвакуации = value; OnPropertyChanged("РасчетноеВремяЭвакуации"); }
        }
        private double расчетноеВремяЭвакуации;

        public double ВремяНачалаЭвакуации
        {
            get { return времяНачалаЭвакуации; }
            set { времяНачалаЭвакуации = value; OnPropertyChanged("ВремяНачалаЭвакуации"); }
        }
        private double времяНачалаЭвакуации;

        public double ВремяБлокирования
        {
            get { return времяБлокировки; }
            set { времяБлокировки = value; OnPropertyChanged("ВремяБлокирования"); }
        }
        private double времяБлокировки;

        public double ВремяСкопления
        {
            get { return времяСкопления; }
            set { времяСкопления = value; OnPropertyChanged("ВремяСкопления"); }
        }
        private double времяСкопления;

        public double ПожарныйРиск
        {
            get { return пожарныйРиск; }
            set { пожарныйРиск = value; OnPropertyChanged("ПожарныйРиск"); }
        }
        private double пожарныйРиск;

        public string Вывод
        {
            get { return вывод; }
            set { вывод = value; OnPropertyChanged("Вывод"); }
        }
        private string вывод;

        public FireRiskViewModel(double времяЭвакуации, double времяБлокирования)
        {
            ВыбранныйПожарныйРиск = ВариантыПожарногоРиска.FirstOrDefault();
            РассчетCommand = new RelayCommand(param => this.Рассчет());

            ВремяНахождения = 4.8;
            УстановкиПожарутшения = false;
            СистемыПожарнойСигнализации = true;
            СистемыОповещения = true;
            СистемыПротиводымнойЗащиты = false;
            РасчетноеВремяЭвакуации = времяЭвакуации;
            ВремяНачалаЭвакуации = 1.5;
            ВремяБлокирования = времяБлокирования;
            ВремяСкопления = 2.03;
            старт = true;
            Рассчет();
        }

        private bool старт = false;

        protected override void OnPropertyChanged(string propertyName)
        {
            if (старт)
                Рассчет();
            base.OnPropertyChanged(propertyName);
        }
        private void Рассчет()
        {
            старт = false;
            double Qn = ВыбранныйПожарныйРиск.Частота;
            double Кап = 0;
            if (УстановкиПожарутшения) Кап = 0.9;
            double Рпр = ВремяНахождения / 24;
            double Кобн = 0;
            if (СистемыПожарнойСигнализации) Кобн = 0.8;
            double Ксоуэ = 0;
            if (СистемыОповещения) Ксоуэ = 0.8;
            double Кпдз = 0;
            if (СистемыПротиводымнойЗащиты) Кпдз = 0.8;

            double Кпз = 1 - (1 - Кобн * Ксоуэ) * (1 - Кобн * Кпдз);

            double Рэ = 0;
            if (РасчетноеВремяЭвакуации < 0.8 * ВремяБлокирования &&
                0.8 * ВремяБлокирования < РасчетноеВремяЭвакуации + ВремяНачалаЭвакуации && ВремяСкопления <= 6)
                Рэ = 0.999 * (0.8 * ВремяБлокирования - РасчетноеВремяЭвакуации) / ВремяНачалаЭвакуации;
            else if (РасчетноеВремяЭвакуации + ВремяНачалаЭвакуации <= 0.8 * ВремяБлокирования && ВремяСкопления <= 6)
                Рэ = 0.999;

            ПожарныйРиск = ВыбранныйПожарныйРиск.Частота * (1 - Кап) * Рпр * (1 - Рэ) * (1 - Кпз);

            if (ПожарныйРиск <= Math.Pow(10, -6))
                Вывод = string.Format("Все ОК");
            else
                Вывод = string.Format("Несоответствие");
            старт = true;
        }

        public ICommand РассчетCommand { get; protected set; }
    }
}
