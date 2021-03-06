﻿using System;

namespace FireSafety.Models
{
    /// <summary>
    /// Сведения о маштабе 
    /// </summary>
    
    public class ZoomTool
    {
        /// <summary>
        /// Значение фактической длины 
        /// </summary>
        public double ActualLength { get; private set; }

        /// <summary>
        /// Значение длины графического представления
        /// </summary>
        public double GraphicLength { get; private set; }

        public static double E = 0.001;

        /// <summary>
        /// Масштаб
        /// </summary>
        /// <param name="actualLength">фактическое значение длины (метры)</param>
        /// <param name="graphicLength">графическое представление (пиксели)
        public ZoomTool(double actualLength, double graphicLength)
        {
            if (actualLength < E || graphicLength < E)
                throw new Exception(string.Format("Необходимо задать значения, превышающие {e}", E));

            ActualLength = actualLength;
            GraphicLength = graphicLength;
        }

        /// <summary>
        /// число метров в одном пикселе
        /// </summary>
        public double Ratio
        {
            get { return ActualLength / GraphicLength; }
        }

        /// <summary>
        /// Мастабирование из длины графического представления в фактическое значение в соответствии с масштабом
        /// </summary>
        /// <param name="graphicLength">значение длины графического представления в пикселях</param>
        /// <returns>фактическое значение</returns>
        public double TransformToActualLength(double graphicLength)
        {
            return ActualLength * graphicLength / GraphicLength;
        }

        /// <summary>
        /// Мастабирование из фактического значения длины в его графическое представление в соответствии с масштабом
        /// </summary>
        /// <param name="actualLength">значение фактической длины в метрах</param>
        /// <returns></returns>

        public double TransformToGraphicLength(double actualLength)
        {
            return GraphicLength * actualLength / ActualLength;
        }

        public static ZoomTool Empty
        {
            get { return new ZoomTool(15, 1920); }
        }

        public static bool operator ==(ZoomTool left, ZoomTool right)
        {
            if (object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null))
                return true;
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
                return false;

            return Math.Abs(right.Ratio - left.Ratio) < E;
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
