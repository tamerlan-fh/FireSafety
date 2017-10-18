namespace FireSafety.Models
{
    /// <summary>
    /// Интерфейс, характеризующий, что объект зависит от масштаба
    /// </summary>
    interface IScalable
    {
        /// <summary>
        /// Автоматически подстраивать размеры в соответствии с масштабом
        /// </summary>
        bool AutoSize { get; }
    }

    /// <summary>
    /// Сведения о маштабе
    /// </summary>
    class Scale
    {
        /// <summary>
        /// Значение фактической длины 
        /// </summary>
        public double ActualLength { get; private set; }

        /// <summary>
        /// Значение длины графического представления
        /// </summary>
        public double GraphicLength { get; private set; }
        public Scale(double actualLength, double graphicLength)
        {
            this.ActualLength = actualLength;
            this.GraphicLength = graphicLength;
        }

        /// <summary>
        /// Мастабирование из длины графического представления в фактическое значение в соответствии с масштабом
        /// </summary>
        /// <param name="length">значение длины графического представления</param>
        /// <returns>фактическое значение</returns>
        public double GetActualLength(double length)
        {
            return ActualLength * length / GraphicLength;
        }
    }
}
