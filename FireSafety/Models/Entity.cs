using FireSafety.ViewModels;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    /// <summary>
    /// Сущность, прародитель всех остальных
    /// </summary>
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
                if (value) Status = EntityStatus.Selected;
                else Status = EntityStatus.Normal;
            }
        }
        private bool isSelected;
        public virtual EntityStatus Status
        {
            get { return status; }
            set
            {
                if (status == value) return;
                status = value; OnPropertyChanged("Status");
            }
        }
        private EntityStatus status;
        public abstract BitmapImage Icon { get; }
        public override string ToString()
        {
            return Title;
        }
    }
}
