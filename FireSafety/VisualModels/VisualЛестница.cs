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
    class VisualЛестница : VisualУзел
    {
        public VisualЛестница(УзелЛестницы объект, Point позиция, VisualЭтаж родитель) : base(объект, позиция, родитель)
        {

        }

        public override void Draw()
        {
            using (DrawingContext dc = RenderOpen())
            {
                var радиусX = Ширина / 2;
                var радиусY = Высота / 2;
                var d = 10;

                if (Состояние == EntityStatus.Selected)
                {
                    dc.PushOpacity(0.5);
                    dc.DrawEllipse(кистьВыделенная, null, Позиция, d + радиусX, d + радиусY);
                    dc.Pop();
                }

                dc.DrawEllipse(кистьНормальная, пероНормальное, Позиция, радиусX, радиусY);
                dc.DrawDrawing(НарисоватьНадпись(Позиция));
            }
        }
    }
}
