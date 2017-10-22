using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    /// <summary>
    /// лустничный пролет
    /// </summary>
    class StairsSection : RoadSection
    {
        private static int index = 1;
        public StairsSection(Node first, Node last, Floor parent) : base(first, last, parent, string.Format("Лестничный пролет {0}", index++)) { }
        public override BitmapImage Icon { get { return Settings.Instance.ЛестницаIco; } }
    }
}
