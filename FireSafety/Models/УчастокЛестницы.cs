using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    class УчастокЛестницы : УчастокПути
    {
        private static int индекс = 1;
        public УчастокЛестницы(Node начало, Node конец, Floor parent) : base(начало, конец, parent, string.Format("Лестничный пролет {0}", индекс++)) { }
        public override BitmapImage Icon { get { return Настройки.Instance.ЛестницаIco; } }
    }
}
