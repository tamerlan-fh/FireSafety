using System.Windows;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    class ExitNode : Node
    {
        private static int index = 1;
        public ExitNode(Floor parent, Point position) : this(parent, position, string.Format("Выход {0}", index++)) { }

        public ExitNode(Floor parent, Point position, string title) : base(parent, position, title)
        {
            OutgoingSectionsAllowed = false;
        }

        public override BitmapImage Icon { get { return Настройки.Instance.ВыходIco; } }
    }
}
