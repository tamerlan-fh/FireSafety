namespace FireSafety.VisualModels
{
    class СхемаЭвакуации
    {
        public СхемаЭвакуации(double ширина, double высота, byte[] bytes)
        {
            Высота = высота;
            Ширина = ширина;
            Bytes = bytes;
        }
        public double Высота { get; private set; }
        public double Ширина { get; private set; }
        public byte[] Bytes { get; private set; }
    }
}
