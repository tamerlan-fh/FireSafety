using FireSafety.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    class VisualСтарт : VisualУзел
    {
        public VisualСтарт(StartNode объект, Point позиция, VisualЭтаж родитель) : base(объект, позиция, родитель)
        {
            Ширина = 60;
            Высота = 60;

            кистьНормальная = Brushes.LightBlue;
            кистьВыделенная = кистьНормальная;
            пероНормальное.Brush = Brushes.Blue;
            Draw();
        }
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

                dc.DrawDrawing(НарисоватьНадпись(Позиция));
                //var радиус = 5;
                //dc.PushOpacity(0.5);
                //dc.DrawRoundedRectangle(Brushes.AliceBlue, null, new Rect(Позиция - new Vector(радиус, радиус), Позиция + new Vector(formattedText.Width + радиус, formattedText.Height + радиус)), радиус, радиус);
                //dc.Pop();
                //dc.DrawText(formattedText, Позиция);

            }
        }
    }
}
