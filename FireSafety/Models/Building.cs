using FireSafety.FireSafetyData;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    class Building : Entity
    {
        private static int index = 1;
        public Building(string title) : base(title, null)
        {
            Floors = new ObservableCollection<Floor>();
            evacuationPlan = new EvacuationPlan("Пути эвакуации", this);
            buildingComposition = new BuildingComposition("Этажи", this, Floors);
            Objects = new ObservableCollection<Entity>()
            {
                evacuationPlan,
                buildingComposition
            };
        }
        public Building() : this(string.Format("Здание {0}", index++)) { }

        private EvacuationPlan evacuationPlan;
        private BuildingComposition buildingComposition;

        public ObservableCollection<Floor> Floors { get; private set; }
        public ObservableCollection<Entity> Objects { get; private set; }

        public bool IsEmpty
        {
            get { return !Floors.Any(); }
        }
        public Floor CurrentFloor
        {
            get { return currentFloor; }
            set { currentFloor = value; OnPropertyChanged("CurrentFloor"); }
        }
        private Floor currentFloor;

        /// <summary>
        /// время эвакуации
        /// </summary>
        public double EvacuationTime
        {
            get { return evacuationTime; }
            set { evacuationTime = value; OnPropertyChanged("EvacuationTime"); }
        }
        private double evacuationTime;
        public void AddFloor(Floor floor)
        {
            if (Floors.Contains(floor)) return;

            Floors.Add(floor);
            floor.RefreshTitle();
            floor.Objects.CollectionChanged += FloorObjectsCollectionChanged;

            if (CurrentFloor == null) CurrentFloor = floor;
        }
        public void RemoveFloor(Floor floor)
        {
            if (!Floors.Contains(floor)) return;

            var index = Floors.IndexOf(floor);

            Floors.Remove(floor);
            floor.Objects.CollectionChanged -= FloorObjectsCollectionChanged;
            floor.RemoveObjects();

            if (index >= Floors.Count - 1)
                CurrentFloor = Floors.LastOrDefault();
            else
                CurrentFloor = Floors[index];

            foreach (var f in Floors)
                f.RefreshTitle();

            evacuationPlan.ComposeRoutes();
        }

        public bool CanAddSection(Section section)
        {
            if (section == null) return false;

            foreach (var route in evacuationPlan.Routes)
                if (route.Sections.Any(x => section.First == x.First || section.First == x.Last)
                    && route.Sections.Any(x => section.Last == x.First || section.Last == x.Last))
                    return false;
            return true;
        }

        public Floor GetFloorBelow(Floor floor)
        {
            var index = Floors.IndexOf(floor);
            if (index < 1) return null;
            else return Floors[index - 1];
        }

        private void FloorObjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var obj in e.NewItems)
                    if (obj is Section) evacuationPlan.ComposeRoutes();

                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                evacuationPlan.ComposeRoutes();
                return;
            }
        }

        /// <summary>
        /// Рассчет блокировании путей эвакуации
        /// </summary>
        public void CalculateBlockageEvacuationRoutes()
        {
            var window = new BlockageEvacuationRoutesWindow();

            if (window.ShowDialog() == true)
            {
                blockageEvacuationRoutesTime = window.BlockageEvacuationRoutesTime.TotalMinutes;
                square = window.Square;
            }
        }

        private double blockageEvacuationRoutesTime = 0;
        private double square = 0;

        public void CalculateFireRisk()
        {

            if (!evacuationPlan.ContainsFormedRoutes)
            {
                MessageBox.Show("Для рассчета пожарного риска необходимо построить хотя бы один путь эвакуации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (blockageEvacuationRoutesTime == 0)
            {
                var result = MessageBox.Show("Для рассчета пожарного риска необходимо предварительно рассчитать время блокирования путей.\r\nХотите рассчитать?", "Предупреждение", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Cancel) return;
                if (result == MessageBoxResult.Yes) CalculateBlockageEvacuationRoutes();
            }

            var window = new FireRiskWindow(EvacuationTime, blockageEvacuationRoutesTime, square);
            window.ShowDialog();
            evacuationPlan.FireRiskValue = window.FireRiskValue;
        }

        public async void ComposeReport()
        {
            if (!evacuationPlan.ContainsFormedRoutes)
            {
                MessageBox.Show("Для составления отчета здание должно содержать хотя бы один путь эвакуациии", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "docx files (*.docx)|*.docx";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = string.Format("{0}_report_{1}.docx", Title.Replace(" ", "_"), DateTime.Now.ToString(@"yyyy\.MM\.dd_HH\.mm\.ss"));

            if (saveFileDialog.ShowDialog() == true)
            {
                var filename = saveFileDialog.FileName;

                var doc = await DocumentManager.Instance.CreateDocument(GetEvacuationPlan());
                doc.SaveAs(filename);
            }
        }

        public override bool AutoSize
        {
            get { return base.AutoSize; }

            set
            {
                foreach (var floor in Floors)
                    floor.AutoSize = value;
                base.AutoSize = value;
            }
        }

        public EvacuationPlan GetEvacuationPlan()
        {
            return evacuationPlan;
        }
        public override BitmapImage Icon { get { return SettingsManager.Instance.BuildingIco; } }
    }
}