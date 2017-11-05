using FireSafety.VisualModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    /// <summary>
    /// Этаж
    /// </summary>
    class Floor : Entity
    {
        public Func<EvacuationPlanImage> GetEvacuationPlanImage { get; set; }
        public Floor(Building parent) : this("Этаж", parent) { }
        public Floor(string title, Building parent) : base(title, parent)
        {
            Objects = new ObservableCollection<Entity>();
            LoadFloorPlanCommand = new RelayCommand(param => this.LoadFloorPlan());
            ClearFloorPlanCommand = new RelayCommand(param => ClearFloorPlan(), param => FloorPlanImage != null);
        }
        public ICommand LoadFloorPlanCommand { get; protected set; }
        public ICommand ClearFloorPlanCommand { get; protected set; }
        public int FloorIndex
        {
            get { return ParentBuilding.Floors.IndexOf(this) + 1; }
        }

        public void RefreshTitle()
        {
            Title = string.Format("Этаж {0}", FloorIndex);
        }

        private void LoadFloorPlan()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files |*.png;*.bmp;*jpg;*.jepg|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != true) return;

            var filename = openFileDialog.FileName;

            var extension = System.IO.Path.GetExtension(filename).ToLower();
            switch (extension)
            {
                case ".png":
                case ".bmp":
                case ".jpg":
                case ".jepg": LoadFloorPlanImage(filename); break;
                default: break;
            }
        }
        private void LoadFloorPlanImage(string filename)
        {
            try
            {
                FloorPlanImage = new BitmapImage(new Uri(filename));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ClearFloorPlan()
        {
            FloorPlanImage = null;
        }
        public BitmapImage FloorPlanImage
        {
            get { return floorPlanImage; }
            set { floorPlanImage = value; OnPropertyChanged("FloorPlanImage"); }
        }
        private BitmapImage floorPlanImage;
        public ObservableCollection<Entity> Objects { get; private set; }
        public void AddObject(Entity obj)
        {
            if (Objects.Contains(obj)) return;

            if (obj is Section)
            {
                var section = obj as Section;
                section.First.AddSection(section);
                section.Last.AddSection(section);
            }

            Objects.Add(obj);
        }
        public bool CanAddSection(Section section)
        {
            return ParentBuilding.CanAddSection(section);
        }
        public void RemoveObject(Entity entity)
        {
            if (entity == null) return;

            var objToRemove = new List<Entity>();

            if (entity is Node)
            {
                var node = entity as Node;
                objToRemove.Add(node);

                foreach (var section in node.IncomingSections)
                {
                    section.First.RemoveSection(section);
                    objToRemove.Add(section);

                    if (section.First is RoadNode && section.First.IsUnrelated)
                        objToRemove.Add(section.First);
                }
                foreach (var section in node.OutgoingSections)
                {
                    section.Last.RemoveSection(section);
                    objToRemove.Add(section);

                    if (section.Last is RoadNode && section.Last.IsUnrelated)
                        objToRemove.Add(section.Last);
                }
            }
            else if (entity is Section)
            {
                var section = entity as Section;

                section.First.RemoveSection(section);
                section.Last.RemoveSection(section);


                objToRemove.Add(section);

                if (section.First is RoadNode && section.First.IsUnrelated)
                    objToRemove.Add(section.First);
                if (section.Last is RoadNode && section.Last.IsUnrelated)
                    objToRemove.Add(section.Last);
            }

            foreach (var obj in objToRemove)
                if (obj != null && obj.Parent != null && obj.Parent is Floor)
                    (obj.Parent as Floor).Objects.Remove(obj);
        }
        public void AddFloorsConnectionSection(StairsNode node)
        {
            if (node.ParentFloor != this) return;
            AddObject(node);

            var floor = ParentBuilding.GetFloorBelow(this);
            if (floor == null) throw new Exception(string.Format("Ошибка при попытке добавить этаж. Под \"{0}\" нет другого этажа", Title));

            var newNode = new StairsNode(floor, node.Position, node.Title);
            floor.AddObject(newNode);
            var connection = new FloorsConnectionSection(node, newNode, this);
            AddObject(connection);
        }
        public void RemoveObjects()
        {
            var objects = new List<Entity>(Objects);
            foreach (var obj in objects)
                RemoveObject(obj);
        }
        public ZoomTool Scale
        {
            get { return scale; }
            set
            {
                if (scale == value) return;
                if (scale == null)
                    scale = ZoomTool.Empty;
                else
                    scale = value;
                OnPropertyChanged("Scale");
            }
        }
        private ZoomTool scale = ZoomTool.Empty;

        public override bool AutoSize
        {
            get { return base.AutoSize; }

            set
            {
                foreach (var obj in Objects)
                    obj.AutoSize = value;
                base.AutoSize = value;
            }
        }
        public override BitmapImage Icon { get { return SettingsManager.Instance.FloorIco; } }
    }
}
