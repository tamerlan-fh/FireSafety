using FireSafety.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    class VisualДверь : VisualУзел
    {
        private List<VisualТочка> точки;
        private VisualТочка верхняяЛевая;
        private VisualТочка верхняяПравая;
        private VisualТочка нижняяЛевая;
        private VisualТочка нижняяПравая;
        //   private VisualМанипулятор манипулятор;
        public VisualДверь(EntryNode объект, Point позиция, VisualЭтаж родитель) : base(объект, позиция, родитель)
        {
            точки = new List<VisualТочка>();
            var dx = 60;
            var dy = 30;

            верхняяЛевая = new VisualТочка(позиция + new Vector(-dx, -dy)); ДобавитьТочку(верхняяЛевая);
            верхняяПравая = new VisualТочка(позиция + new Vector(dx, -dy)); ДобавитьТочку(верхняяПравая);
            нижняяПравая = new VisualТочка(позиция + new Vector(dx, dy)); ДобавитьТочку(нижняяПравая);
            нижняяЛевая = new VisualТочка(позиция + new Vector(-dx, dy)); ДобавитьТочку(нижняяЛевая);
            Габариты = new Rect(нижняяЛевая.Позиция, верхняяПравая.Позиция);

            Draw();
        }

        public override Point Позиция
        {
            get
            {
                if (верхняяЛевая == null || нижняяПравая == null) return new Point();
                // УзелМодель.Позиция = верхняяЛевая.Позиция + (нижняяПравая.Позиция - верхняяЛевая.Позиция) / 2;
                return УзелМодель.Position;
                // return верхняяЛевая.Позиция + (нижняяПравая.Позиция - верхняяЛевая.Позиция) / 2;
            }
        }
        public override VisualЭлемент ДатьЭлемент(Point позиция)
        {
            foreach (var точка in точки)
                if (точка.Содержит(позиция)) return точка;
            return this;
        }
        public override void Перемещение(Vector сдвиг)
        {
            OnПозицияИзменена(new NodePositionEventArgs(Позиция, сдвиг));

            нижняяЛевая.Позиция += сдвиг;
            верхняяПравая.Позиция += сдвиг;
            нижняяПравая.Позиция += сдвиг;
            верхняяЛевая.Позиция += сдвиг;
            //  манипулятор.Позиция += сдвиг;

            УзелМодель.Position = верхняяЛевая.Позиция + (нижняяПравая.Позиция - верхняяЛевая.Позиция) / 2;
            Габариты = new Rect(нижняяЛевая.Позиция, верхняяПравая.Позиция);
            Draw();
        }
        public override bool Содержит(Point позиция)
        {
            foreach (var точка in точки)
                if (точка.Содержит(позиция)) return true;

            return Габариты.Contains(позиция);
        }
        public void ДобавитьТочку(VisualТочка точка)
        {
            точки.Add(точка);

            this.Children.Add(точка);
            точка.ПозицияИзменена += ПозицияТочкиИзменена;
        }
        private void ПозицияТочкиИзменена(object sender, NodePositionEventArgs e)
        {
            var точка = sender as VisualТочка;
            var сдвиг = e.Сдвиг;

            //if (Math.Abs(сдвиг.X) > Ширина)
            //{
            //    var поправка = new Vector(Ширина - Math.Abs(сдвиг.X), 0);
            //    if (Math.Sign(сдвиг.X) == -1) поправка = -1 * поправка;
            //    сдвиг += поправка;
            //    точка.Позиция += поправка;
            //}


            if (точка == верхняяЛевая)
            {
                нижняяЛевая.Позиция += new Vector(сдвиг.X, 0);
                верхняяПравая.Позиция += new Vector(0, сдвиг.Y);
            }
            else if (точка == верхняяПравая)
            {
                нижняяПравая.Позиция += new Vector(сдвиг.X, 0);
                верхняяЛевая.Позиция += new Vector(0, сдвиг.Y);
            }
            else if (точка == нижняяЛевая)
            {
                верхняяЛевая.Позиция += new Vector(сдвиг.X, 0);
                нижняяПравая.Позиция += new Vector(0, сдвиг.Y);
            }
            else if (точка == нижняяПравая)
            {
                верхняяПравая.Позиция += new Vector(сдвиг.X, 0);
                нижняяЛевая.Позиция += new Vector(0, сдвиг.Y);
            }
            OnПозицияИзменена(new NodePositionEventArgs(Позиция, new Vector(0, 0)));

            УзелМодель.Position = верхняяЛевая.Позиция + (нижняяПравая.Позиция - верхняяЛевая.Позиция) / 2;
            Габариты = new Rect(нижняяЛевая.Позиция, верхняяПравая.Позиция);
            Draw();
        }

        protected override void ОбъектPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.ОбъектPropertyChanged(sender, e);
            if (Модель.IsSelected)
                foreach (var точка in точки) точка.Отображена = true;
            else
                foreach (var точка in точки) точка.Отображена = false;
        }
        public override void Draw()
        {
            if (верхняяЛевая == null || верхняяПравая == null || нижняяЛевая == null || нижняяПравая == null) return;

            using (DrawingContext dc = RenderOpen())
            {
                if (Состояние == EntityStatus.Selected)
                {
                    var d = 10;
                    var вектор = new Vector(d, d);
                    dc.PushOpacity(0.5);
                    dc.DrawRectangle(кистьВыделенная, null, new Rect(Габариты.TopLeft - вектор, Габариты.BottomRight + вектор));
                    dc.Pop();
                }

                dc.DrawRectangle(кистьНормальная, пероНормальное, Габариты);
                dc.DrawDrawing(НарисоватьНадпись(Позиция));
            }
        }
    }
}
