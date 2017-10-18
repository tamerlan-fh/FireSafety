using System;
using System.Windows;

namespace FireSafety.VisualModels
{
    class NodePositionEventArgs : EventArgs
    {
        private Point стараяПозиция;
        private Vector сдвиг;
        public NodePositionEventArgs(Point стараяПозиция, Vector сдвиг)
        {
            this.стараяПозиция = стараяПозиция;
            this.сдвиг = сдвиг;
        }

        public Point СтараяПозиция { get { return стараяПозиция; } }
        public Point НоваяПозиция { get { return стараяПозиция + сдвиг; } }
        public Vector Сдвиг { get { return сдвиг; } }
    }
}
