﻿using System.Linq;
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
        public override BitmapImage Icon { get { return Settings.Instance.УзелIco; } }


        public bool IsFloorsConnected
        {
            get { return isFloorsConnected; }
            set
            {
                isFloorsConnected = value; OnPropertyChanged("IsFloorsConnected");
                if (IsFloorsConnected)
                    ParentFloor.AddFloorsConnectionSection(this);
            }
        }
        private bool isFloorsConnected;


        public bool CanAddFloorsConnection
        {
            get { return ParentFloor.FloorIndex != 1 && (IsFloorsConnected || !OutgoingSections.Any()); }
        }
     
        public override void AddSection(Section section)
        {
            base.AddSection(section);
            OnPropertyChanged("СвязьЭтажомНижеАктивность");
        }
        public override void RemoveSection(Section section)
        {
            base.RemoveSection(section);
            OnPropertyChanged("СвязьЭтажомНижеАктивность");
        }
    }
}
