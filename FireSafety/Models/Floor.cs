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
        public Building ParentBuilding { get { return Parent as Building; } }
        public Floor(Building parent) : this("Этаж", parent) { }
        public Floor(string title, Building parent) : base(title, parent)
        {
            Objects = new ObservableCollection<Entity>();
            LoadFloorPlanCommand = new RelayCommand(param => this.LoadFloorPlan());
        }
        public ICommand LoadFloorPlanCommand { get; protected set; }
        public int FloorIndex
        {
            get { return ParentBuilding.Floors.IndexOf(this) + 1; }
        }
        public void ОбновитьНазвание()
        {
            this.Title = string.Format("Этаж {0}", FloorIndex);
        }
        /// <summary>
        /// LoadFloorPlan
        /// </summary>
        private void LoadFloorPlan()
        {
            var openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "Pdf files (*.pdf)|*.pdf|Image files |*.png;*.bmp;*jpg;*.jepg|All files (*.*)|*.*";
            openFileDialog.Filter = "Image files |*.png;*.bmp;*jpg;*.jepg|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != true) return;

            var filename = openFileDialog.FileName;

            var extension = System.IO.Path.GetExtension(filename).ToLower();
            switch (extension)
            {
                //case ".pdf": ЗагрузитьPdf(имяФайла); break;
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

            Objects.Add(obj);
        }
        public void RemoveObject(Entity obj)
        {
            if (obj == null) return;

            if (obj is Node)
            {
                var узел = obj as Node;
                var удаляемыеОбьекты = new List<Entity>();
                удаляемыеОбьекты.Add(узел);

                foreach (var участок in узел.IncomingSections)
                {
                    участок.First.RemoveSection(участок);
                    удаляемыеОбьекты.Add(участок as Entity);
                    if (участок.First is RoadNode && участок.First.IsUnrelated)
                        удаляемыеОбьекты.Add(участок.First);
                }
                foreach (var участок in узел.OutgoingSections)
                {
                    участок.Last.RemoveSection(участок);
                    удаляемыеОбьекты.Add(участок as Entity);
                    if (участок.Last is RoadNode && участок.Last.IsUnrelated)
                        удаляемыеОбьекты.Add(участок.Last);
                }

                foreach (var удаляемыйОбъект in удаляемыеОбьекты)
                    Objects.Remove(удаляемыйОбъект);

                return;
            }


            if (obj is RoadSection)
            {
                var участок = obj as RoadSection;

                участок.First.RemoveSection(участок);
                участок.Last.RemoveSection(участок);

                var удаляемыеОбьекты = new List<Entity>();
                удаляемыеОбьекты.Add(участок);

                if (участок.First is RoadNode && участок.First.IsUnrelated)
                    удаляемыеОбьекты.Add(участок.First);
                if (участок.Last is RoadNode && участок.Last.IsUnrelated)
                    удаляемыеОбьекты.Add(участок.Last);

                foreach (var удаляемыйОбъект in удаляемыеОбьекты)
                    Objects.Remove(удаляемыйОбъект);

                return;
            }
        }
        public void ДобавитьСпуск(StairsNode узел)
        {
            //var этажНиже = РодительскоеЗдание.ВыдатьЭтажНиже(this);
            var этажНиже = this;
            if (этажНиже == null) throw new Exception(string.Format("Ошибка при попытке добавить этаж. Под \"{0}\" нет другого этажа", Title));

            var новыйУзел = new StairsNode(этажНиже, узел.Position, узел.Title);
            этажНиже.AddObject(новыйУзел);
            var спуск = new FloorsConnection(узел, новыйУзел, this);
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
        private ZoomTool scale;
        public override BitmapImage Icon { get { return Settings.Instance.ЭтажIco; } }
    }
}
