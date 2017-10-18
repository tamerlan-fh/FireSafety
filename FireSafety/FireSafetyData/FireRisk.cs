namespace FireSafety.FireSafetyData
{
    class ПожарныйРиск
    {
        public ПожарныйРиск(string название, double частота)
        {
            Название = название;
            Частота = частота;
        }
        public string Название { get; private set; }
        public double Частота { get; private set; }

        public override string ToString()
        {
            return Название;
        }
    }
}
