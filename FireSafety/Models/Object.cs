using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    abstract class Entity : BasePropertyChanged
    {
        /// <summary>
        /// Родительский объект
        /// </summary>
        public Entity Parent { get; private set; }
        public Entity(string title, Entity parent)
        {
            Parent = parent;
            Title = title;
        }

        /// <summary>
        /// Название
        /// </summary>
        public virtual string Title
        {
            get { return title; }
            set { if (title == value) return; title = value; OnPropertyChanged("Название"); }
        }
        private string title;

        /// <summary>
        /// Является выбранным
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected == value) return;

                isSelected = value; OnPropertyChanged("IsSelected");
                if (value) Status = ObjectStatus.Selected;
                else Status = ObjectStatus.Normal;
            }
        }
        private bool isSelected;
        public virtual ObjectStatus Status
        {
            get { return status; }
            set
            {
                if (status == value) return;
                status = value; OnPropertyChanged("Состояние");
            }
        }
        private ObjectStatus status;
        public abstract BitmapImage Icon { get; }
        public override string ToString()
        {
            return Title;
        }
        public bool AutoSize
        {
            get { return autoSize; }
            set { autoSize = value; OnPropertyChanged("AutoSize"); }
        }
        private bool autoSize;
    }
}
