using FireSafety.Models;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    abstract class VisualEntity : VisualUnit
    {
        public Entity Model { get; protected set; }
        public VisualFloor ParentFloor { get; protected set; }

        protected FormattedText formattedText;
        public VisualEntity(Entity entity, VisualFloor parent)
        {    
            this.Model = entity;
            this.ParentFloor = parent;
            formattedText = new FormattedText(Model.Title, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Verdana"), 18, Brushes.Black);
            entity.PropertyChanged += EntityPropertyChanged;
        }
        public VisualEntity(Entity entity, Point position, VisualFloor parent) : this(entity, parent)
        {
            Position = position;
            Status = EntityStatus.Normal;
        }

        protected virtual void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Position": break;
                case "Status":
                    {
                        Draw(); break;
                    }
                case "Title":
                    {
                        formattedText = new FormattedText(Model.Title, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Verdana"), 18, Brushes.Black);
                        Draw(); break;
                    }
                default: Draw(); break;
            }
        }

        public virtual EntityStatus Status
        {
            get { return Model.Status; }
            set { Model.Status = value; }
        }       
        protected Drawing DrawTitle(Point point)
        {
            var title = new DrawingVisual();
            using (DrawingContext dc = title.RenderOpen())
            {
                var radius = 5;
                var brush = Brushes.AliceBlue;
                if (Status == EntityStatus.Selected)
                    brush = Brushes.Orange;
                dc.PushOpacity(0.5);
                dc.DrawRoundedRectangle(brush, null, new Rect(point - new Vector(radius, radius), point + new Vector(formattedText.Width + radius, formattedText.Height + radius)), radius, radius);
                dc.Pop();
                dc.DrawText(formattedText, point);
            }
            return title.Drawing;
        }
    }
}
