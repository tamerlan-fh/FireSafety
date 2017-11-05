using System.Windows;

namespace FireSafety.VisualModels
{
    /// <summary>
    /// Информация о перемещаемом обьекте
    /// </summary>
    class MoveInformation
    {
        public MoveInformation(IMovable unit, Point position, bool isUsesAbsoluteValues = false)
        {
            this.Unit = unit;
            this.StartPosition = position;
            this.IsUsedAbsoluteValues = isUsesAbsoluteValues;
        }

        /// <summary>
        /// перемещаемый объект
        /// </summary>
        public IMovable Unit { get; private set; }

        /// <summary>
        /// Информация о позиции объекта
        /// </summary>
        public Point StartPosition { get; set; }

        //
        public bool IsUsedAbsoluteValues { get; private set; }
    }

    interface IMovable
    {
        void Move(Vector vector);
    }
}
