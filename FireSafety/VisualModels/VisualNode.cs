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
    class VisualУзел : VisualОбъект
    {
        public Node УзелМодель { get { return Модель as Node; } }
        public VisualУзел(Node объект, Point позиция, VisualЭтаж родитель) : base(объект, позиция, родитель)
        {
            Ширина = VisualУчастокПути.Толщина + 5;
            Высота = VisualУчастокПути.Толщина + 5;

            ВходящиеПути = new List<VisualУчастокПути>();
            ИсходящиеПути = new List<VisualУчастокПути>();

            кистьНормальная = Brushes.LightGreen;
            кистьВыделенная = кистьНормальная;
            пероНормальное.Brush = Brushes.Green;
            пероВыделенное = пероНормальное;
            Draw();
        }
        public List<VisualУчастокПути> ВходящиеПути { get; private set; }
        public List<VisualУчастокПути> ИсходящиеПути { get; private set; }
        public void ДобавитьУчасток(VisualУчастокПути участок)
        {
            if (участок.Начало == this)
                if (!ИсходящиеПути.Contains(участок))
                    ИсходящиеПути.Add(участок);
            if (участок.Конец == this)
                if (!ВходящиеПути.Contains(участок))
                    ВходящиеПути.Add(участок);
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
            }
        }
    }
}
