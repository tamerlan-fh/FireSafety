﻿using FireSafety.IO;
using FireSafety.Models;
using FireSafety.VisualModels;
using Microsoft.Win32;
using Novacode;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FireSafety.ViewModels
{
    class MainWindowViewModel : BasePropertyChanged
    {
        #region Команды
        public ICommand ExitCommand { get; protected set; }
        public ICommand AddBuildingCommand { get; protected set; }
        public ICommand AddFloorCommand { get; protected set; }
        public ICommand RemoveSelectedEntityCommand { get; protected set; }
        public ICommand LoadBuildingCommand { get; protected set; }
        public ICommand SaveBuldingCommand { get; protected set; }
        public ICommand CalculateBlockageEvacuationRoutesCommand { get; protected set; }
        public ICommand CalculateFireRiskCommand { get; protected set; }
        public ICommand ComposeReportCommand { get; protected set; }
        public ICommand AddFloorsConnectionSectionCommand { get; protected set; }

        #endregion

        public ActionMode Mode
        {
            get { return mode; }
            set
            {
                if (mode == value) return;
                mode = value; OnPropertyChanged("Mode");
            }
        }
        private ActionMode mode;
        public ObservableCollection<Building> Buildings { get; private set; }
        public Entity SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                if (SelectedEntity == value) return;
                selectedEntity = value;
                OnPropertyChanged("SelectedEntity");
                if (value == null) return;

                SelectedEntity.IsSelected = true;
                if (SelectedEntity is Floor)
                    CurrentFloor = value as Floor;
                else if (SelectedEntity is Building)
                    CurrentBuilding = value as Building;
                else if (SelectedEntity is Route)
                    return;
                else if (SelectedEntity.Parent is Floor)
                    CurrentFloor = SelectedEntity.Parent as Floor;
                else if (SelectedEntity.Parent is Building)
                    CurrentBuilding = SelectedEntity.Parent as Building;
            }
        }
        private Entity selectedEntity;
        public Floor CurrentFloor
        {
            get { return currentFloor; }
            set
            {
                if (CurrentFloor == value) return;
                currentFloor = value;
                OnPropertyChanged("CurrentFloor");
                if (CurrentFloor != null)
                    CurrentBuilding = CurrentFloor.ParentBuilding;

                CurrentFloorNotNull = CurrentFloor != null;
            }
        }
        private Floor currentFloor;
        public Building CurrentBuilding
        {
            get { return currentBuilding; }
            set
            {
                if (CurrentBuilding != value)
                {
                    currentBuilding = value;
                    OnPropertyChanged("CurrentBuilding");
                }

                if (CurrentBuilding == null)
                { CurrentFloor = null; return; }

                if ((CurrentFloor == null || CurrentFloor.Parent != CurrentBuilding)
                    && CurrentBuilding != null)
                    CurrentFloor = CurrentBuilding.Floors.FirstOrDefault();
            }
        }
        private Building currentBuilding;

        public bool CurrentFloorNotNull
        {
            get { return currentFloorNotNull; }
            set { currentFloorNotNull = value; OnPropertyChanged("CurrentFloorNotNull"); }
        }
        private bool currentFloorNotNull = false;
        public MainWindowViewModel()
        {
            ExitCommand = new RelayCommand(param => App.Current.Shutdown());
            AddBuildingCommand = new RelayCommand(param => this.AddBuilding());
            AddFloorCommand = new RelayCommand(param => this.AddFloor(), param => CurrentBuilding != null);
            RemoveSelectedEntityCommand = new RelayCommand(param => this.RemoveSelectedEntity(), param => CanRemoveSelectedEntity());
            CalculateBlockageEvacuationRoutesCommand = new RelayCommand(param => CurrentBuilding.CalculateBlockageEvacuationRoutes(), param => CurrentBuilding != null);
            CalculateFireRiskCommand = new RelayCommand(param => CurrentBuilding.CalculateFireRisk(), param => CurrentBuilding != null);
            ComposeReportCommand = new RelayCommand(param => CurrentBuilding.ComposeReport(), param => CurrentBuilding != null);
            SaveBuldingCommand = new RelayCommand(param => SaveBuilding(), param => CurrentBuilding != null);
            LoadBuildingCommand = new RelayCommand(param => LoadBuilding());
            AddFloorsConnectionSectionCommand = new RelayCommand(p => AddFloorsConnectionSection(), p => CanAddFloorsConnectionSection());

            Buildings = new ObservableCollection<Building>();

            Mode = ActionMode.Move;
            AddBuilding();
        }

        private void AddBuilding()
        {
            AddBuilding(new Building());
            AddFloor();
        }

        private void AddBuilding(Building building)
        {
            Buildings.Add(building);
            SelectedEntity = building;
        }
        private void AddFloor()
        {
            if (CurrentBuilding == null) return;
            var floor = new Floor(CurrentBuilding);

            CurrentBuilding.AddFloor(floor);
            CurrentBuilding.CurrentFloor = floor;
            SelectedEntity = floor;
        }

        private void RemoveSelectedEntity()
        {
            Mode = ActionMode.Remove;
            if (SelectedEntity == null) return;

            if (SelectedEntity is Building)
            {
                var index = Buildings.IndexOf(SelectedEntity as Building);
                Buildings.Remove(SelectedEntity as Building);

                if (index >= Buildings.Count - 1)
                    CurrentBuilding = Buildings.LastOrDefault();
                else
                    CurrentBuilding = Buildings[index];

                SelectedEntity = CurrentBuilding;
                return;
            }

            if (SelectedEntity is Floor)
            {
                CurrentBuilding.RemoveFloor(SelectedEntity as Floor);
                CurrentFloor = CurrentBuilding.CurrentFloor;
                SelectedEntity = CurrentFloor;
                return;
            }

            CurrentFloor.RemoveObject(SelectedEntity);
            SelectedEntity = CurrentFloor.Objects.FirstOrDefault();
            if (SelectedEntity == null) SelectedEntity = CurrentFloor;
        }

        private bool CanRemoveSelectedEntity()
        {
            return SelectedEntity != null &&
                 (SelectedEntity is Node
                || SelectedEntity is Models.Section
                || SelectedEntity is Building
                || SelectedEntity is Floor);
        }
        private void LoadBuilding()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "dat files (*.dat)|*.dat|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != true) return;

            var b = SaveBuildingManager.Instance.Load(openFileDialog.FileName);
            AddBuilding(b);
        }

        private void SaveBuilding()
        {
            var saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "dat files (*.dat)|*.dat|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = string.Format("{0}.dat", CurrentBuilding.Title);

            if (saveFileDialog.ShowDialog() == true)
                SaveBuildingManager.Instance.Save(CurrentBuilding, saveFileDialog.FileName);
        }


        public Point ContextMenuPosition { get; set; }
        private void AddFloorsConnectionSection()
        {
            var node = new StairsNode(CurrentFloor, ContextMenuPosition);
            CurrentFloor.AddObject(node);
            node.IsFloorsConnected = true;
        }

        private bool CanAddFloorsConnectionSection()
        {
            return (ContextMenuPosition != null)
                && !(CurrentBuilding == null || CurrentFloor == null || CurrentBuilding.GetFloorBelow(CurrentFloor) == null);
        }
    }
}
