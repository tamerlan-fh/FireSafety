using FireSafety.Models;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    abstract class VisualОбъект : VisualЭлемент
    {
        public Entity Модель { get; protected set; }
        public VisualЭтаж РодительскийЭтаж { get; protected set; }

        protected FormattedText formattedText;
        public VisualОбъект(Entity объект, VisualЭтаж родитель)
        {
            this.Модель = объект;
            this.РодительскийЭтаж = родитель;
            formattedText = new FormattedText(Модель.Title, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Verdana"), 18, Brushes.Black);
            объект.PropertyChanged += ОбъектPropertyChanged;
        }
        public VisualОбъект(Entity объект, Point позиция, VisualЭтаж родитель) : this(объект, родитель)
        {
            Позиция = позиция;
            Состояние = EntityStatus.Normal;
        }

        protected virtual void ОбъектPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Позиция": break;
                case "Состояние":
                    {
                        Draw(); break;
                    }
                case "Название":
                    {
                        formattedText = new FormattedText(Модель.Title, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Verdana"), 18, Brushes.Black);
                        Draw(); break;
                    }
                default: Draw(); break;
            }
        }

        public virtual EntityStatus Состояние
        {
            get { return Модель.Status; }
            set { Модель.Status = value; }
        }
        protected Drawing НарисоватьНадпись(Point точка)
        {
            var надпись = new DrawingVisual();
            using (DrawingContext dc = надпись.RenderOpen())
            {
                var радиус = 5;
                var кисть = Brushes.AliceBlue;
                if (Состояние == EntityStatus.Selected)
                    кисть = Brushes.Orange;
                dc.PushOpacity(0.5);
                dc.DrawRoundedRectangle(кисть, null, new Rect(точка - new Vector(радиус, радиус), точка + new Vector(formattedText.Width + радиус, formattedText.Height + радиус)), радиус, радиус);
                dc.Pop();
                dc.DrawText(formattedText, точка);
            }
            return надпись.Drawing;
        }
    }
}
