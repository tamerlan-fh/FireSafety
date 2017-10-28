using System;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    /// <summary>
    /// лустничный пролет
    /// </summary>

    class StairsSection : RoadSection
    {
        private static int index = 1;
        public StairsSection(Node first, Node last, Floor parent) : this(first, last, parent, string.Format("Лестничный пролет {0}", index++)) { }
        public StairsSection(Node first, Node last, Floor parent, string title) : base(first, last, parent, title) { }
        public override BitmapImage Icon { get { return Settings.Instance.StairsIco; } }
    }
}
