using System;

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
    class ZoomTool
    {
        /// <summary>
        /// Значение фактической длины 
        /// </summary>
        public double ActualLength { get; private set; }

        /// <summary>
        /// Значение длины графического представления
        /// </summary>
        public double GraphicLength { get; private set; }
        public ZoomTool(double actualLength, double graphicLength)
        {
            double e = 0.001;
            if (actualLength < e || graphicLength < e)
                throw new Exception(string.Format("Необходимо задать значения, превышающие {e}", e));

            this.ActualLength = actualLength;
            this.GraphicLength = graphicLength;
        }

        public double Ratio
        {
            get { return ActualLength / GraphicLength; }
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

        public static ZoomTool Empty
        {
            get { return new ZoomTool(1, 1); }
        }

        public static bool operator ==(ZoomTool left, ZoomTool right)
        {
            if (object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null))
                return true;
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
                return false;

            return Math.Abs(right.Ratio - left.Ratio) < 0.05;
        }
        public static bool operator !=(ZoomTool left, ZoomTool right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ZoomTool))
                return false;
            return this == obj as ZoomTool;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
