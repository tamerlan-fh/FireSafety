using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    abstract class VisualЭлемент : DrawingVisual
    {
        public VisualЭлемент()
        {
            var радиус = 30;
            Ширина = 2 * радиус;
            Высота = 2 * радиус;
        }

        public double Ширина
        {
            get { return ширина; }
            protected set { ширина = value; ВекторДиагонали = new Vector(Ширина, Высота); }
        }
        private double ширина;

        public double Высота
        {
            get { return высота; }
            protected set { высота = value; ВекторДиагонали = new Vector(Ширина, Высота); }
        }
        private double высота;
        public virtual Point Позиция
        {
            get { return позиция; }
            set
            {
                позиция = value;
                Габариты = new Rect(Позиция - 0.5 * ВекторДиагонали, Позиция + 0.5 * ВекторДиагонали);
                Draw();
            }
        }
        private Point позиция;
        public Vector ВекторДиагонали { get; protected set; }
        public virtual Rect Габариты { get; protected set; }
        public abstract void Draw();
        //{
        //    using (DrawingContext dc = RenderOpen())
        //    {
        //        dc.DrawRectangle(кистьНормальная, пероНормальное, Габариты);
        //    }
        //}

        protected Brush кистьНормальная = Brushes.Gray;
        protected Pen пероНормальное = new Pen(Brushes.Black, 2);

        protected Brush кистьВыделенная = Brushes.LightGray;
        protected Pen пероВыделенное = new Pen(Brushes.Black, 2);

        public virtual VisualЭлемент ДатьЭлемент(Point позиция)
        {
            return this;
        }

        public virtual void Перемещение(Vector сдвиг)
        {
            OnПозицияИзменена(new NodePositionEventArgs(Позиция, сдвиг));
            Позиция += сдвиг;
        }

        public virtual bool Содержит(Point точка)
        {
            return Габариты.Contains(точка);
        }

        public event EventHandler<NodePositionEventArgs> ПозицияИзменена;
        protected virtual void OnПозицияИзменена(NodePositionEventArgs e)
        {
            ПозицияИзменена?.Invoke(this, e);
        }
    }
}
