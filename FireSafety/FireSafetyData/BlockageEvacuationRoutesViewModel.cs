using FireSafety.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace FireSafety.FireSafetyData
{
    class BlockageEvacuationRoutesViewModel : BasePropertyChanged
    {
        #region Переменные
        public List<ТиповаяНагрузка> ТиповыеНагрузки { get { return ТаблицаТиповаяНагрузка.Instance.ТиповыеНагрузки; } }
        public ТиповаяНагрузка ВыбраннаяТиповаяНагрузка
        {
            get { return выбраннаяТиповаяНагрузка; }
            set { выбраннаяТиповаяНагрузка = value; OnPropertyChanged("ВыбраннаяТиповаяНагрузка"); }
        }
        private ТиповаяНагрузка выбраннаяТиповаяНагрузка;
        public double Длина
        {
            get { return длина; }
            set { длина = value; OnPropertyChanged("Длина"); OnPropertyChanged("ОбъемПомещения"); OnPropertyChanged("СвободныйОбъемПомещения"); }
        }
        private double длина = 1;

        public double Ширина
        {
            get { return ширина; }
            set { ширина = value; OnPropertyChanged("Ширина"); OnPropertyChanged("ОбъемПомещения"); OnPropertyChanged("СвободныйОбъемПомещения"); }
        }
        private double ширина = 1;

        public double Высота
        {
            get { return высота; }
            set { высота = value; OnPropertyChanged("Высота"); OnPropertyChanged("ОбъемПомещения"); OnPropertyChanged("СвободныйОбъемПомещения"); }
        }
        private double высота = 1;

        public double ОбъемПомещения { get { return Ширина * Высота * Длина; } }

        public double СвободныйОбъемПомещения { get { return 0.8 * ОбъемПомещения; } }

        public int НачальнаяТемператураВоздуха
        {
            get { return начальнаяТемператураВоздуха; }
            set { начальнаяТемператураВоздуха = value; OnPropertyChanged("НачальнаяТемператураВоздуха"); OnPropertyChanged("УдельнаяИзобарнаяТеплоемкостьГаза"); }
        }
        private int начальнаяТемператураВоздуха;

        public double ВысотаРабочейЗоны
        {
            get { return высотаРабочейЗоны; }
            set { высотаРабочейЗоны = value; OnPropertyChanged("ВысотаРабочейЗоны"); }
        }
        private double высотаРабочейЗоны;

        public double РазностьВысотПола
        {
            get { return разностьВысотПола; }
            set { разностьВысотПола = value; OnPropertyChanged("РазностьВысотПола"); }
        }
        private double разностьВысотПола;

        public double ПредельнаяДальностьВидимости
        {
            get { return предельнаяДальностьВидимости; }
            set { предельнаяДальностьВидимости = value; OnPropertyChanged("ПредельнаяДальностьВидимости"); }
        }
        private double предельнаяДальностьВидимости;

        public double НачальнаяОсвещенность
        {
            get { return начальнаяОсвещенность; }
            set { начальнаяОсвещенность = value; OnPropertyChanged("НачальнаяОсвещенность"); }
        }
        private double начальнаяОсвещенность;

        public double КоэффициентОтраженияПредметов
        {
            get { return коэффициентОтраженияПредметов; }
            set { коэффициентОтраженияПредметов = value; OnPropertyChanged("КоэффициентОтраженияПредметов"); }
        }
        private double коэффициентОтраженияПредметов;

        public double КоэффициентТеплопотер
        {
            get { return коэффициентТеплопотер; }
            set { коэффициентТеплопотер = value; OnPropertyChanged("КоэффициентТеплопотер"); }
        }
        private double коэффициентТеплопотер;

        public double НачальнаяКонцентрацияКислорода
        {
            get { return начальнаяКонцентрацияКислорода; }
            set { начальнаяКонцентрацияКислорода = value; OnPropertyChanged("НачальнаяКонцентрацияКислорода"); }
        }
        private double начальнаяКонцентрацияКислорода;

        public double ТекущаяКонцентрацияКислорода
        {
            get { return текущаяКонцентрацияКислорода; }
            set { текущаяКонцентрацияКислорода = value; OnPropertyChanged("ТекущаяКонцентрацияКислорода"); }
        }
        private double текущаяКонцентрацияКислорода;

        //public double УдельнаяИзобарнаяТеплоемкостьГаза
        //{
        //    get { return удельнаяИзобарнаяТеплоемкостьГаза; }
        //    set { удельнаяИзобарнаяТеплоемкостьГаза = value; OnPropertyChanged("УдельнаяИзобарнаяТеплоемкостьГаза"); }
        //}
        //private double удельнаяИзобарнаяТеплоемкостьГаза;

        public double УдельнаяИзобарнаяТеплоемкостьГаза
        {
            get { return ТаблицаФизическиеСвойстваДымовыхГазов.Instance.УдельнаяТеплоемкость2(НачальнаяТемператураВоздуха); }
        }

        public double КоэффииентПолнотыГорения
        {
            get { return коэффииентПолнотыГорения; }
            set { коэффииентПолнотыГорения = value; OnPropertyChanged("КоэффииентПолнотыГорения"); }
        }
        private double коэффииентПолнотыГорения;

        public string ВремяПоПовышеннойТемпературе
        {
            get { return времяПоПовышеннойТемпературе; }
            set { времяПоПовышеннойТемпературе = value; OnPropertyChanged("ВремяПоПовышеннойТемпературе"); }
        }
        private string времяПоПовышеннойТемпературе;

        public string ВремяПоПотереВидимости
        {
            get { return времяПоПотереВидимости; }
            set { времяПоПотереВидимости = value; OnPropertyChanged("ВремяПоПотереВидимости"); }
        }
        private string времяПоПотереВидимости;

        public string ВремяПоПониженномуСодержаниюКислорода
        {
            get { return времяПоПониженномуСодержаниюКислорода; }
            set { времяПоПониженномуСодержаниюКислорода = value; OnPropertyChanged("ВремяПоПониженномуСодержаниюКислорода"); }
        }
        private string времяПоПониженномуСодержаниюКислорода;

        public string СодержаниеCО
        {
            get { return содержаниеCО; }
            set { содержаниеCО = value; OnPropertyChanged("СодержаниеCО"); }
        }
        private string содержаниеCО;

        public string СодержаниеCО2
        {
            get { return содержаниеCО2; }
            set { содержаниеCО2 = value; OnPropertyChanged("СодержаниеCО2"); }
        }
        private string содержаниеCО2;

        public string СодержаниеHCL
        {
            get { return содержаниеHCL; }
            set { содержаниеHCL = value; OnPropertyChanged("СодержаниеHCL"); }
        }
        private string содержаниеHCL;

        public bool КруговоеРаспространенияПожара { get; set; }
        public bool ПрямоугольноеРаспространенияПожара { get; set; }
        public bool ГорениеЖидкости { get; set; }

        public TimeSpan ВремяБлокирования
        {
            get { return времяБлокирования; }
            set { времяБлокирования = value; OnPropertyChanged(""); }
        }
        private TimeSpan времяБлокирования;

        public IEnumerable<int> ДиапазонТемператур { get; private set; }

        #endregion

        public BlockageEvacuationRoutesViewModel()
        {
            ДиапазонТемператур = Enumerable.Range(0, 1201);
            НачальнаяТемператураВоздуха = 24;

            ВыбраннаяТиповаяНагрузка = ТиповыеНагрузки.FirstOrDefault();
            Длина = 10;
            Ширина = 10;
            Высота = 3;


            ВысотаРабочейЗоны = 0;
            РазностьВысотПола = 0;
            ПредельнаяДальностьВидимости = 20;
            НачальнаяОсвещенность = 50;
            КоэффициентОтраженияПредметов = 0.3;
            КоэффициентТеплопотер = 0.55;
            НачальнаяКонцентрацияКислорода = 0.23;
            ТекущаяКонцентрацияКислорода = 0.23;

            КруговоеРаспространенияПожара = true; OnPropertyChanged("КруговоеРаспространенияПожара");

            РассчетCommand = new RelayCommand(param => this.Рассчет());
            старт = true;
            Рассчет();
        }
        private bool старт = false;
        private void ЗагрузитьДанные()
        {

            var имяфайла = "ТиповыеНагрузки.txt";

            if (!File.Exists(имяфайла))
            {
                MessageBox.Show(string.Format("Отсутствует файл \"{0}\" с данными о типовых нагрузках", имяфайла), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var данные = File.ReadAllText(имяфайла);
            var pattern = new Regex(@"\""(?<название>.*?)\""(?<значения>.*?)\n");

            var matches = pattern.Matches(данные + "\r\n");
            foreach (Match match in matches)
                if (match.Success)
                {
                    string название = match.Groups["название"].Value;
                    if (string.IsNullOrEmpty(название)) continue;
                    string значения = match.Groups["значения"].Value;
                    if (string.IsNullOrEmpty(значения)) continue;

                    //if (NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator == ".")
                    //    значения.Replace(",", NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator);
                    //else
                    //    значения.Replace(".", NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator);

                    значения.Replace(",", ".");

                    var список = значения.Split(new string[] { "\t", " ", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    if (список == null || список.Length < 8) continue;
                    var нагрузка = new ТиповаяНагрузка(название);

                    double число;
                    if (!double.TryParse(список[0], NumberStyles.Float, CultureInfo.InvariantCulture, out число)) continue;
                    нагрузка.НизшаяТеплотаСгорания = число;

                    if (!double.TryParse(список[1], NumberStyles.Float, CultureInfo.InvariantCulture, out число)) continue;
                    нагрузка.ЛинейнаяСкоростьПламени = число;

                    if (!double.TryParse(список[2], NumberStyles.Float, CultureInfo.InvariantCulture, out число)) continue;
                    нагрузка.УдельнаяСкоростьВыгорания = число;

                    if (!double.TryParse(список[3], NumberStyles.Float, CultureInfo.InvariantCulture, out число)) continue;
                    нагрузка.ДымообразующаяСпособность = число;

                    if (!double.TryParse(список[4], NumberStyles.Float, CultureInfo.InvariantCulture, out число)) continue;
                    нагрузка.ПотреблениеКислорода = число;

                    if (!double.TryParse(список[5], NumberStyles.Float, CultureInfo.InvariantCulture, out число)) continue;
                    нагрузка.МаксимальныйВыходCO2 = число;

                    if (!double.TryParse(список[6], NumberStyles.Float, CultureInfo.InvariantCulture, out число)) continue;
                    нагрузка.МаксимальныйВыходCO = число;

                    if (!double.TryParse(список[7], NumberStyles.Float, CultureInfo.InvariantCulture, out число)) continue;
                    нагрузка.МаксимальныйВыходHCL = число;

                    ТиповыеНагрузки.Add(нагрузка);
                }
        }
        private void Рассчет()
        {
            старт = false;
            var h = ВысотаРабочейЗоны + 1.7 - 0.5 * РазностьВысотПола;
            var z = h / Высота * Math.Exp(1.4 * h / Высота);

            var N = 0.63 + 0.2 * НачальнаяКонцентрацияКислорода + 1500 * Math.Pow(ТекущаяКонцентрацияКислорода, 6);
            var b = 353 * УдельнаяИзобарнаяТеплоемкостьГаза * СвободныйОбъемПомещения / ((1 - КоэффициентТеплопотер) * ВыбраннаяТиповаяНагрузка.НизшаяТеплотаСгорания * N);

            double a = 0;
            int n = 1;
            if (КруговоеРаспространенияПожара)
            {
                a = 1.05 * ВыбраннаяТиповаяНагрузка.УдельнаяСкоростьВыгорания * Math.Pow(ВыбраннаяТиповаяНагрузка.ЛинейнаяСкоростьПламени, 2);
                n = 3;
            }
            else if (ПрямоугольноеРаспространенияПожара)
            {
                a = ВыбраннаяТиповаяНагрузка.УдельнаяСкоростьВыгорания * ВыбраннаяТиповаяНагрузка.ЛинейнаяСкоростьПламени * Ширина;
                n = 2;
            }


            try
            {
                double t = Math.Pow(b / a * Math.Log(1 + (70 - НачальнаяТемператураВоздуха) / ((273 + НачальнаяТемператураВоздуха) * z)), 1.0 / n);
                var время = TimeSpan.FromSeconds(t);
                ВремяПоПовышеннойТемпературе = время.ToString(@"hh\:mm\:ss\.fff");
                if (ВремяБлокирования.Ticks == 0 || ВремяБлокирования.Ticks > время.Ticks)
                    ВремяБлокирования = время;

            }
            catch (Exception)
            {
                ВремяПоПовышеннойТемпературе = "НЕ ОПАСНО";
            }

            try
            {
                double число = 1 - (СвободныйОбъемПомещения * Math.Log(1.05 * НачальнаяОсвещенность * КоэффициентОтраженияПредметов)) / (ПредельнаяДальностьВидимости * ВыбраннаяТиповаяНагрузка.ДымообразующаяСпособность * b * z);
                var t = Math.Pow(b / a * Math.Log(Math.Pow(число, -1)), 1f / n);
                var время = TimeSpan.FromSeconds(t);
                ВремяПоПотереВидимости = время.ToString(@"hh\:mm\:ss\.fff");
                if (ВремяБлокирования.Ticks == 0 || ВремяБлокирования.Ticks > время.Ticks)
                    ВремяБлокирования = время;

            }
            catch (Exception)
            {
                ВремяПоПотереВидимости = "НЕ ОПАСНО";
            }

            try
            {
                double число = 1 - 0.044 / ((b * ВыбраннаяТиповаяНагрузка.ПотреблениеКислорода / СвободныйОбъемПомещения + 0.27) * z);
                var t = Math.Pow(b / a * Math.Log(Math.Pow(число, -1)), 1f / n);

                var время = TimeSpan.FromSeconds(t);
                ВремяПоПониженномуСодержаниюКислорода = время.ToString(@"hh\:mm\:ss\.fff");
                if (ВремяБлокирования.Ticks == 0 || ВремяБлокирования.Ticks > время.Ticks)
                    ВремяБлокирования = время;
            }
            catch (Exception)
            {
                ВремяПоПониженномуСодержаниюКислорода = "НЕ ОПАСНО";
            }

            try
            {
                double x = 0.11;
                double число = 1 - СвободныйОбъемПомещения * x / (b * z * ВыбраннаяТиповаяНагрузка.МаксимальныйВыходCO2);
                var t = Math.Pow(b / a * Math.Log(Math.Pow(число, -1)), 1f / n);

                var время = TimeSpan.FromSeconds(t);
                СодержаниеCО2 = время.ToString(@"hh\:mm\:ss\.fff");
                if (ВремяБлокирования.Ticks == 0 || ВремяБлокирования.Ticks > время.Ticks)
                    ВремяБлокирования = время;
            }
            catch (Exception)
            {
                СодержаниеCО2 = "НЕ ОПАСНО";
            }

            try
            {
                double x = 0.00116;
                double число = 1 - СвободныйОбъемПомещения * x / (b * z * ВыбраннаяТиповаяНагрузка.МаксимальныйВыходCO);
                var t = Math.Pow(b / a * Math.Log(Math.Pow(число, -1)), 1f / n);

                var время = TimeSpan.FromSeconds(t);
                СодержаниеCО = время.ToString(@"hh\:mm\:ss\.fff");
                if (ВремяБлокирования.Ticks == 0 || ВремяБлокирования.Ticks > время.Ticks)
                    ВремяБлокирования = время;
            }
            catch (Exception)
            {
                СодержаниеCО = "НЕ ОПАСНО";
            }

            try
            {
                double x = 0.000023;
                double число = 1 - СвободныйОбъемПомещения * x / (b * z * ВыбраннаяТиповаяНагрузка.МаксимальныйВыходHCL);
                var t = Math.Pow(b / a * Math.Log(Math.Pow(число, -1)), 1f / n);

                var время = TimeSpan.FromSeconds(t);
                СодержаниеHCL = время.ToString(@"hh\:mm\:ss\.fff");
                if (ВремяБлокирования.Ticks == 0 || ВремяБлокирования.Ticks > время.Ticks)
                    ВремяБлокирования = время;
            }
            catch (Exception)
            {
                СодержаниеHCL = "НЕ ОПАСНО";
            }

            старт = true;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            if (старт) Рассчет();
            base.OnPropertyChanged(propertyName);
        }
        public ICommand РассчетCommand { get; protected set; }
    }
}
