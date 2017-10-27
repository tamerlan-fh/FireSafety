﻿using FireSafety.FireSafetyData;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
            floor.ОбновитьНазвание();
            floor.Objects.CollectionChanged += FloorObjectsCollectionChanged;

            if (CurrentFloor == null) CurrentFloor = floor;
        }
        public void RemoveFloor(Floor floor)
        {
            if (!Floors.Contains(floor)) return;

            var index = Floors.IndexOf(floor);

            Floors.Remove(floor);
            floor.Objects.CollectionChanged -= FloorObjectsCollectionChanged;

            if (index >= Floors.Count - 1)
                CurrentFloor = Floors.LastOrDefault();
            else
                CurrentFloor = Floors[index];

            foreach (var f in Floors)
                f.ОбновитьНазвание();

            evacuationPlan.ComposeRoutes();
        }
        public Floor ВыдатьЭтажНиже(Floor этаж)
        {
            var индекс = Floors.IndexOf(этаж);
            if (индекс < 1) return null;
            else return Floors[индекс - 1];
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
            }
        }

        private double blockageEvacuationRoutesTime = 0;

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

            var window = new FireRiskWindow(EvacuationTime, blockageEvacuationRoutesTime);
            window.ShowDialog();
        }

        public void ComposeReport()
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
                var document = evacuationPlan.CreateDocument(saveFileDialog.FileName);
                document.Save();
            }
        }
        public override BitmapImage Icon { get { return Settings.Instance.ЗданиеIco; } }
    }
}