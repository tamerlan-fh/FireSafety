using System;
using System.Windows;

namespace FireSafety.VisualModels
{
    class PositionChangedEventArgs : EventArgs
    {
        private Point oldPosition;
        private Vector shift;
        public PositionChangedEventArgs(Point oldPosition, Vector shift)
        {
            this.oldPosition = oldPosition;
            this.shift = shift;
        }

        public Point OldPosition { get { return oldPosition; } }
        public Point NewPosition { get { return oldPosition + shift; } }
        public Vector Shift { get { return shift; } }
    }
}
