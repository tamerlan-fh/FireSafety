using FireSafety.Models;
using FireSafety.Properties;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    class VisualУзелЛестницы : VisualУзел
    {
        public УзелЛестницы УзелЛестницы { get { return Модель as УзелЛестницы; } }
        public VisualУзелЛестницы(Node объект, Point позиция, VisualЭтаж родитель) : base(объект, позиция, родитель)
        {
            кистьНормальная = Brushes.Orange;
            кистьВыделенная = кистьНормальная;
            пероНормальное.Brush = Brushes.Brown;
            пероВыделенное = пероНормальное;
           // иконка = Настройки.GetBitmapImage(, ImageFormat.Png);
            Draw();
        }

        private ImageSource иконка;
        public override void Draw()
        {
            using (DrawingContext dc = RenderOpen())
            {
                var радиусX = Ширина / 2;
                var радиусY = Высота / 2;
                var d = 10;

                if (Состояние == ObjectStatus.Selected)
                {
                    dc.PushOpacity(0.5);
                    dc.DrawEllipse(кистьВыделенная, null, Позиция, d + радиусX, d + радиусY);
                    dc.Pop();
                }

                dc.DrawEllipse(кистьНормальная, пероНормальное, Позиция, радиусX, радиусY);
                if (УзелЛестницы.СвязьЭтажомНиже.Значение)
                {
                    var v = new Vector(Ширина, Высота);
                    dc.DrawImage(иконка, new Rect(Позиция + v, Позиция - v));
                }
                dc.DrawDrawing(НарисоватьНадпись(Позиция));
            }
        }
    }
}
