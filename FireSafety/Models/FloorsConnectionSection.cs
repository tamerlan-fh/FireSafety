﻿using System.ComponentModel;
using System.Windows.Media.Imaging;

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
            get { return string.Format("Связь {0} - {1}", First.ParentFloor.Title, Last.Parent.Title); }
            set { base.Title = value; }
        }

        private void NodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Title")
                OnPropertyChanged("Title");
        }

        public override SectionTypes SectionType { get { return SectionTypes.Other; } }

        public override BitmapImage Icon { get { return SettingsManager.Instance.FloorsConnectionIco; } }
    }
}
