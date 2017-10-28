using System.Windows;

namespace FireSafety.VisualModels
{
    /// <summary>
    /// Информация о перемещаемом обьекте
    /// </summary>
    class MoveInformation
    {
        public MoveInformation(VisualUnit unit, Point position)
        {
            this.Unit = unit;
            this.StartPosition = position;
        }

        /// <summary>
        /// перемещаемый объект
        /// </summary>
        public VisualUnit Unit { get; private set; }

        /// <summary>
        /// Информация о позиции объекта
        /// </summary>
        public Point StartPosition { get; set; }
    }
}
