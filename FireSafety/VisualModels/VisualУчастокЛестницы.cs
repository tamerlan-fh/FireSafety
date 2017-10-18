using FireSafety.Models;
using System;
using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    class VisualУчастокЛестницы : VisualУчастокПути
    {
        public VisualУчастокЛестницы(VisualУзел начало, VisualУзел конец, УчастокПути объект, VisualЭтаж родитель) : base(начало, конец, объект, родитель)
        {
            пероНормальное.Brush = Brushes.RosyBrown;
            пероНормальное.Thickness = Толщина;

            пероВыделенное.Brush = Brushes.Yellow;
            пероВыделенное.Thickness = Толщина + 20;
        }

        private Pen пероСтупенек = new Pen(Brushes.Brown, 1);
        public override void Draw()
        {
            if (Начало == null || Конец == null) return;
            using (DrawingContext dc = RenderOpen())
            {
                if (Состояние == ObjectStatus.Selected)
                {
                    dc.PushOpacity(0.5);
                    dc.DrawLine(пероВыделенное, Начало.Позиция, Конец.Позиция);
                    dc.Pop();
                }
                dc.DrawLine(пероНормальное, Начало.Позиция, Конец.Позиция);

                var шаг = Толщина / 2;
                var вектор = Конец.Позиция - Начало.Позиция;
                вектор.Normalize();
                вектор *= шаг;
                var перпендикуляр = new Vector(вектор.Y, -вектор.X);

                int количество = Convert.ToInt32((Конец.Позиция - Начало.Позиция).Length / шаг);
                var точка = Начало.Позиция;
                for (int i = 0; i < количество; i++)
                {
                    точка += вектор;
                    dc.DrawLine(пероСтупенек, точка + перпендикуляр, точка - перпендикуляр);
                }


                dc.DrawDrawing(НарисоватьНадпись(Начало.Позиция + (Конец.Позиция - Начало.Позиция) / 2));

                //var радиус = 5;
                //var позиция = Начало.Позиция + (Конец.Позиция - Начало.Позиция) / 2;
                //dc.PushOpacity(0.5);
                //dc.DrawRoundedRectangle(Brushes.AliceBlue, null, new Rect(позиция - new Vector(радиус, радиус), позиция + new Vector(formattedText.Width + радиус, formattedText.Height + радиус)), радиус, радиус);
                //dc.Pop();
                //dc.DrawText(formattedText, позиция);
            }
        }
    }
}
