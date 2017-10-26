using System.ComponentModel;

namespace FireSafety.Models
{
    /// <summary>
    /// связь между этажами
    /// </summary>

    class FloorsConnectionSection : Section
    {
        public FloorsConnectionSection(Node first, Node last, Floor parent) : base("Связь этажей", parent)
        {
            this.First = first;
            this.Last = last;
            First.AddSection(this);
            Last.AddSection(this);
            Width = -1;
            Length = 0;
            AutoSize = false;

            first.ParentFloor.PropertyChanged += NodePropertyChanged;
            last.ParentFloor.PropertyChanged += NodePropertyChanged;
        }

        public override string Title
        {
            get { return string.Format("Связь этажей {0} - {1}", First.ParentFloor.Title, Last.Parent.Title); }
        }

        private void NodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Title")
                OnPropertyChanged("Title");
        }
    }
}
