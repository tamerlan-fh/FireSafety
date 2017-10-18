using FireSafety.Models;
using System;
using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    class VisualУчастокПути : VisualОбъект
    {
        public VisualУчастокПути(VisualУзел начало, VisualУзел конец, УчастокПути объект, VisualЭтаж родитель) : base(объект, родитель)
        {
            this.Начало = начало;
            this.Конец = конец;

            Начало.ИсходящиеПути.Add(this);
            Конец.ВходящиеПути.Add(this);

            Начало.ПозицияИзменена += УзелПозицияИзменена;
            Конец.ПозицияИзменена += УзелПозицияИзменена;

            пероНормальное.Brush = Brushes.Green;
            пероНормальное.Thickness = Толщина;

            пероВыделенное.Brush = Brushes.Yellow;
            пероВыделенное.Thickness = Толщина + 20;

            Draw();
        }

        public const double Толщина = 20;

        private void УзелПозицияИзменена(object sender, NodePositionEventArgs e)
        {
            Draw();
        }

        public override void Перемещение(Vector сдвиг)
        {
            Начало.Перемещение(сдвиг);
            Конец.Перемещение(сдвиг);
        }

        public VisualУзел Начало { get; private set; }
        public VisualУзел Конец { get; private set; }
        public override ObjectStatus Состояние
        {
            get { return base.Состояние; }

            set
            {
                Начало.Состояние = value;
                Конец.Состояние = value;
                base.Состояние = value;
            }
        }
        public override bool Содержит(Point точка)
        {
            //if (Начало.Содержит(точка)) return true;
            //if (Конец.Содержит(точка)) return true;


            //double уголА = Math.Abs(Vector.AngleBetween(точка - Начало.Позиция, Конец.Позиция - Начало.Позиция));
            //if (уголА > 90) return false;

            //double уголБ = Math.Abs(Vector.AngleBetween(точка - Конец.Позиция, Начало.Позиция - Конец.Позиция));
            //if (уголБ > 90) return false;

            //double h = (Начало.Позиция - точка).Length * Math.Sin(уголА * Math.PI / 180);
            //return h <= толщина;

            return ДатьЭлемент(точка) != null;
        }

        public override VisualЭлемент ДатьЭлемент(Point позиция)
        {
            if (Начало.Содержит(позиция)) return Начало;
            if (Конец.Содержит(позиция)) return Конец;

            double уголА = Math.Abs(Vector.AngleBetween(позиция - Начало.Позиция, Конец.Позиция - Начало.Позиция));
            if (уголА > 90) return null;

            double уголБ = Math.Abs(Vector.AngleBetween(позиция - Конец.Позиция, Начало.Позиция - Конец.Позиция));
            if (уголБ > 90) return null;

            double h = (Начало.Позиция - позиция).Length * Math.Sin(уголА * Math.PI / 180);
            if (h <= Толщина)
                return this;
            else
                return null;
        }

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
