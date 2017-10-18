namespace FireSafety.FireSafetyData
{
    class ФизическиеСвойстваДымовыхГазов
    {
        public ФизическиеСвойстваДымовыхГазов(int температура, double плотность, double теплоемкость)
        {
            this.Температура = температура;
            this.Плотность = плотность;
            this.УдельнаяТеплоемкость1 = теплоемкость;

        }

        /// <summary>
        /// t, 0C
        /// </summary>
        public int Температура { get; private set; }

        /// <summary>
        /// ρ, кг/м3
        /// </summary>
        public double Плотность { get; private set; }

        /// <summary>
        /// Ср, кДж/(кг ⋅ К)
        /// </summary>
        public double УдельнаяТеплоемкость1 { get; private set; }

        /// <summary>
        /// Ср, мДж/(кг ⋅ К)
        /// </summary>
        public double УдельнаяТеплоемкость2 { get { return УдельнаяТеплоемкость1 / 1000; } }
    }
}
