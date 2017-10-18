using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireSafety.FireSafetyData
{
    class ТиповаяНагрузка
    {
        public ТиповаяНагрузка(string название)
        {
            this.Название = название;
        }

        public ТиповаяНагрузка(string название,
            double низшаяТеплотаСгорания, double линейнаяСкоростьПламени, double удельнаяСкоростьВыгорания, double дымообразующаяСпособность,
            double потреблениеКислорода, double максимальныйВыходCO2, double максимальныйВыходCO, double максимальныйВыходHCL)
        {
            this.Название = название;
            this.НизшаяТеплотаСгорания = низшаяТеплотаСгорания;
            this.ЛинейнаяСкоростьПламени = линейнаяСкоростьПламени;
            this.УдельнаяСкоростьВыгорания = удельнаяСкоростьВыгорания;
            this.ДымообразующаяСпособность = дымообразующаяСпособность;
            this.ПотреблениеКислорода = потреблениеКислорода;
            this.МаксимальныйВыходCO2 = максимальныйВыходCO2;
            this.МаксимальныйВыходCO = максимальныйВыходCO;
            this.МаксимальныйВыходHCL = максимальныйВыходHCL;
        }
        public string Название { get; set; }
        public double НизшаяТеплотаСгорания { get; set; }
        public double ЛинейнаяСкоростьПламени { get; set; }
        public double УдельнаяСкоростьВыгорания { get; set; }
        public double ДымообразующаяСпособность { get; set; }
        public double ПотреблениеКислорода { get; set; }
        public double МаксимальныйВыходCO2 { get; set; }
        public double МаксимальныйВыходCO { get; set; }
        public double МаксимальныйВыходHCL { get; set; }

        public override string ToString()
        {
            return Название;
        }
    }
}
