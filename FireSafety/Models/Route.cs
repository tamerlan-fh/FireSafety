using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{

    class Route : Entity, IDisposable
    {
        private static int index = 1;
        public List<Section> Sections { get; private set; }
        public Route(Building parent) : this(string.Format("Путь {0}", index++), parent) { }
        public Route(string title, Building parent) : base(title, parent)
        {
            Sections = new List<Section>();
        }

        /// <summary>
        /// Начало пути
        /// </summary>
        public Node FirstNode
        {
            get { return Sections.FirstOrDefault().First; }
        }

        /// <summary>
        /// Конец пути
        /// </summary>
        public Node LastNode
        {
            get { return Sections.LastOrDefault().Last; }
        }

        /// <summary>
        /// Стартовый узел пути
        /// </summary>
        public StartNode Start { get { return FirstNode as StartNode; } }

        public void AddSections(IEnumerable<Section> sections)
        {
            foreach (var section in sections)
                AddSection(section);
        }
        public void AddSection(Section section)
        {
            if (Sections.Contains(section)) return;
            if (Sections.Any())
                if (Sections.LastOrDefault().Last != section.First) throw new Exception("Не верно составлен путь!");

            Sections.Add(section);
            section.PropertyChanged += SectionPropertyChanged;

            if (section.First is StartNode) section.First.PropertyChanged += SectionPropertyChanged;
        }
        private void SectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Width": OnRouteChanged(e.PropertyName, (sender as Section).Width); break;
                case "PeopleCount": OnRouteChanged(e.PropertyName, (sender as StartNode).PeopleCount); break;
                case "Length": OnRouteChanged(e.PropertyName, (sender as Section).Length); break;
            }
        }
        public double MovementTime
        {
            get { return movementTime; }
            set { movementTime = value; OnPropertyChanged("MovementTime"); }
        }
        private double movementTime;
        public double Length
        {
            get { return length; }
            set { length = value; OnPropertyChanged("Length"); }
        }
        private double length;

        /// <summary>
        /// Является сформированным - соединяет старт с выходом
        /// </summary>
        public bool IsFormed
        {
            get { return (FirstNode is StartNode && LastNode is ExitNode); }
        }
        public override EntityStatus Status
        {
            get { return base.Status; }
            set
            {
                if (Status == value) return;
                base.Status = value;
                foreach (var section in Sections)
                    (section as Entity).Status = value;
            }
        }
        public override BitmapImage Icon { get { return SettingsManager.Instance.RouteIco; } }

        public event EventHandler<RouteChangedEventArgs> RouteChanged;
        protected virtual void OnRouteChanged(string name, object value)
        {
            RouteChanged?.Invoke(this, new RouteChangedEventArgs(name, value));
        }
        public void Dispose()
        {
            foreach (var section in Sections)
                section.PropertyChanged += SectionPropertyChanged;
            if (Sections.FirstOrDefault().First is StartNode)
                (Sections.FirstOrDefault().First as StartNode).PropertyChanged -= SectionPropertyChanged;

            Sections.Clear();
        }
    }
    public class RouteChangedEventArgs : EventArgs
    {
        public RouteChangedEventArgs(string name, object value)
        {
            this.ParameterName = name;
            this.ParameterValue = value;
        }
        public string ParameterName { get; private set; }
        public object ParameterValue { get; private set; }
    }
}
