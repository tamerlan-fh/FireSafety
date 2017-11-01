using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    class BuildingComposition : Entity
    {
        public BuildingComposition(string title, Entity parent, ObservableCollection<Floor> floors) : base(title, parent)
        {
            Floors = floors;
        }
        public ObservableCollection<Floor> Floors { get; private set; }
        public override BitmapImage Icon { get { return SettingsManager.Instance.BuildingIco; } }
    }
}
