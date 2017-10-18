using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    class VisualТочка : VisualЭлемент
    {
        private const double радиус = 6;
        public VisualТочка(Point позиция)
        {
            Ширина = 2 * радиус;
            Высота = 2 * радиус;
            Позиция = позиция;
            кистьНормальная = Brushes.White;
            Draw();
        }

        public bool Отображена
        {
            get { return отображена; }
            set
            {
                if (отображена == value) return;
                отображена = value; Draw();
            }
        }
        private bool отображена;
        public override VisualЭлемент ДатьЭлемент(Point позиция)
        {
            if (Отображена)
                return base.ДатьЭлемент(позиция);
            else return null;
        }
        public override bool Содержит(Point точка)
        {
            if (Отображена)
                return base.Содержит(точка);
            else
                return false;
        }
        public override void Draw()
        {
            using (DrawingContext dc = RenderOpen())
            {
                if (Отображена)
                    dc.DrawRectangle(кистьНормальная, пероНормальное, Габариты);
            }
        }
    }
}
