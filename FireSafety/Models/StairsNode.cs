using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    /// <summary>
    /// лестничный узел
    /// </summary>
    class StairsNode : RoadNode
    {
        private static int index = 1;
        public StairsNode(Floor parent, Point position) : this(parent, position, string.Format("Лестничный узел {0}", index++)) { }
        public StairsNode(Floor parent, Point position, string title) : base(parent, position, title)
        {
            IsFloorsConnected = false;
        }
        public override BitmapImage Icon { get { return Settings.Instance.NodeIco; } }


        public bool IsFloorsConnected
        {
            get { return isFloorsConnected; }
            set
            {
                isFloorsConnected = value; OnPropertyChanged("IsFloorsConnected");
                if (IsFloorsConnected)
                    ParentFloor.AddFloorsConnectionSection(this);
                else
                    RemoveFloorsConnection();
            }
        }
        private bool isFloorsConnected;

        private void RemoveFloorsConnection()
        {
            foreach (var connection in OutgoingSections.Where(x => x is FloorsConnectionSection).ToArray())
            {
                (connection.Parent as Floor).RemoveObject(connection);
                //ParentFloor.RemoveObject(connection);
            }
        }

        public bool CanAddFloorsConnection
        {
            get { return ParentFloor.FloorIndex != 1 && (IsFloorsConnected || !OutgoingSections.Any()); }
        }

        public override void RemoveSection(Section section)
        {
            base.RemoveSection(section);
            if (section is FloorsConnectionSection)
                IsFloorsConnected = false;
        }
    }
}
