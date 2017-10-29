using FireSafety.Models;
using FireSafety.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FireSafety.VisualModels
{
    class EvacuationControl : FrameworkElement
    {

        #region Свойство Здания

        public static readonly DependencyProperty BuildingsProperty = DependencyProperty.Register("Buildings", typeof(ObservableCollection<Building>), typeof(EvacuationControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(BuildingsPropertyChanged)));
        private static void BuildingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as EvacuationControl).ApplyingNewValues(e.NewValue as ObservableCollection<Building>);
        }

        public ObservableCollection<Building> Buildings
        {
            get { return (ObservableCollection<Building>)GetValue(BuildingsProperty); }
            set { SetValue(BuildingsProperty, value); }
        }

        #endregion

        #region Свойство Режим Взаимодействия

        public static readonly DependencyProperty SelectedActionModeProperty = DependencyProperty.Register("SelectedActionMode", typeof(ActionMode), typeof(EvacuationControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(SelectedActionModePropertyChanged)));
        private static void SelectedActionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
                (d as EvacuationControl).IsCreatingLine = false;
        }


        public ActionMode SelectedActionMode
        {
            get { return (ActionMode)GetValue(SelectedActionModeProperty); }
            set { SetValue(SelectedActionModeProperty, value); }
        }

        #endregion

        #region Свойство Текущий Этаж

        public static readonly DependencyProperty SelectedFloorProperty = DependencyProperty.Register("SelectedFloor", typeof(Floor), typeof(EvacuationControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(SelectedFloorPropertyChanged)));
        private static void SelectedFloorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var floor = e.NewValue as Floor;
            var control = d as EvacuationControl;
            if (floor == null)
                control.CurrentFloor = null;
            else
                control.CurrentFloor = control.floors.FirstOrDefault(x => x.Model == floor);
        }

        /// <summary>
        /// SelectedFloor
        /// </summary>
        public Floor SelectedFloor
        {
            get { return (Floor)GetValue(SelectedFloorProperty); }
            set { SetValue(SelectedFloorProperty, value); }
        }

        #endregion

        public EvacuationControl()
        {
            floors = new List<VisualFloor>();
            buildings = new List<VisualBuilding>();

            this.visuals = new VisualCollection(this);
            this.Width = 1920;
            this.Height = 1080;
            ToDefaultValues();
        }

        #region Перемещение

        private MoveInformation moveInformation = null;
        private bool IsMoving
        {
            get { return moveInformation != null; }
            set { if (value == false) moveInformation = null; }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMoving)
            {
                e.Handled = true;
                var position = e.GetPosition(this);
                var shift = position - moveInformation.StartPosition;
                moveInformation.Unit.Move(shift);

                moveInformation.StartPosition = position;

                return;
            }
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!IsCreatingLine)
                IsMoving = false;
        }

        #endregion

        #region Построение отрезков

        private CreatingLineInformation CreatingLineInf
        {
            get { return creatingLineInf; }
            set
            {
                if (creatingLineInf != null)
                    visuals.Remove(creatingLineInf.Drawing);

                creatingLineInf = value;
                if (creatingLineInf != null)
                {
                    visuals.Add(creatingLineInf.Drawing);
                    moveInformation = new MoveInformation(creatingLineInf.LastUnit, creatingLineInf.LastPosition);
                }
            }
        }
        private CreatingLineInformation creatingLineInf = null;

        public bool IsCreatingLine
        {
            get { return CreatingLineInf != null; }
            set { if (value == false) CreatingLineInf = null; }
        }
        private void CreatingSection(Point position)
        {
            var entity = CurrentFloor.GetVisualEntity(position);
            VisualNode node = null;
            if (entity is VisualNode)
                node = entity as VisualNode;
            else
            {
                switch (SelectedActionMode)
                {
                    case ActionMode.AddStairs:
                        node = new VisualStairsNode(new StairsNode(CurrentFloor.Model, position), CurrentFloor); break;
                    case ActionMode.AddRoad:
                        node = new VisualNode(new RoadNode(CurrentFloor.Model, position), CurrentFloor); break;
                }
                AddVisualEntity(node);
            }

            if (IsCreatingLine)
            {
                if (!node.NodeModel.IncomingSectionsAllowed) return;
                if (CreatingLineInf.FirstUnit == node || !(CreatingLineInf.FirstUnit is VisualNode)) return;

                VisualRoadSection section = null;
                Section model = null;
                switch (SelectedActionMode)
                {
                    case ActionMode.AddStairs:
                        model = new StairsSection(CreatingLineInf.FirstNode.NodeModel, node.NodeModel, CurrentFloor.Model); break;
                    case ActionMode.AddRoad:
                        model = new RoadSection(CreatingLineInf.FirstNode.NodeModel, node.NodeModel, CurrentFloor.Model); break;
                }

                // по каким-то причинам (образовывается петля) нет возможности добавить данный участок
                if (!CurrentFloor.Model.CanAddSection(model))
                    return;

                switch (SelectedActionMode)
                {
                    case ActionMode.AddStairs:
                        section = new VisualStairsSection(CreatingLineInf.FirstNode, node, model as StairsSection, CurrentFloor); break;
                    case ActionMode.AddRoad:
                        section = new VisualRoadSection(CreatingLineInf.FirstNode, node, model as RoadSection, CurrentFloor); break;
                }

                AddVisualEntity(section);

            }

            CreatingLineInf = new CreatingLineInformation(node);

            if (!CreatingLineInf.FirstNode.NodeModel.OutgoingSectionsAllowed)
                IsCreatingLine = false;
        }

        #endregion

        #region Масштабирование

        private bool IsSetScale
        {
            get { return SelectedActionMode == ActionMode.SetScale; }
        }

        private void SetScale()
        {
            if (!IsSetScale || !IsCreatingLine) return;

            var length = CreatingLineInf.Length;
            if (length > 0)
            {
                var win = new ScaleWindow(length, CurrentFloor.Model.Scale);
                if (win.ShowDialog() == true)
                {
                    CurrentFloor.Model.Scale = win.Scale;
                    CurrentFloor.ApplyScale();
                }
            }

            IsCreatingLine = false;
        }

        #endregion

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (IsCreatingLine)
            {
                IsCreatingLine = false;
                e.Handled = true;
            }
            else
                e.Handled = false;

         //   base.OnMouseRightButtonDown(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (CurrentFloor == null) return;

            e.Handled = true;
            var position = e.GetPosition(this);

            try
            {
                switch (SelectedActionMode)
                {
                    case ActionMode.AddStart:
                        {
                            var start = new StartNode(CurrentFloor.Model, position);
                            AddVisualEntity(new VisualStartNode(start, CurrentFloor)); break;
                        }
                    case ActionMode.AddExit:
                        {
                            var exit = new ExitNode(CurrentFloor.Model, position);
                            AddVisualEntity(new VisualExitNode(exit, CurrentFloor)); break;
                        }
                    case ActionMode.AddEntry:
                        {
                            var entry = new EntryNode(CurrentFloor.Model, position);
                            AddVisualEntity(new VisualEntryNode(entry, CurrentFloor)); break;
                        }
                    case ActionMode.AddStairs:
                    case ActionMode.AddRoad:
                        {
                            CreatingSection(position);
                            break;
                        }
                    case ActionMode.Move:
                        {
                            var entity = CurrentFloor.GetVisualEntity(position);
                            if (entity != null)
                            {
                                e.Handled = true;
                                moveInformation = new MoveInformation(entity.GetUnit(position), position);
                                entity.Model.IsSelected = true;
                            }
                            else
                                e.Handled = false;

                            break;
                        }
                    case ActionMode.Remove:
                        {
                            var entity = CurrentFloor.GetVisualEntity(position);
                            CurrentFloor.RemoveVisualEntity(entity);
                            break;
                        }
                    case ActionMode.SetScale:
                        {
                            if (IsCreatingLine)
                            {
                                CreatingLineInf.LastPosition = position;
                                SetScale();
                            }
                            else
                                CreatingLineInf = new CreatingLineInformation(new VisualThumb(position));
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToDefaultValues()
        {
            CurrentFloor = null;
            IsMoving = false;
            IsCreatingLine = false;

            floors.Clear();

            if (!buildings.Any()) return;
            foreach (var building in buildings)
                building.Model.Floors.CollectionChanged -= FloorsCollectionChanged;
            buildings.Clear();
        }

        private void AddVisualEntity(VisualEntity entry)
        {
            if (CurrentFloor == null) return;
            CurrentFloor.AddVisualEntity(entry);
        }

        private void ApplyingNewValues(ObservableCollection<Building> newBuildings)
        {
            ToDefaultValues();

            if (newBuildings == null) return;
            newBuildings.CollectionChanged += BuildingsCollectionChanged;
            foreach (var building in newBuildings)
                AddBuilding(building);

            CurrentFloor = floors.FirstOrDefault();
        }


        #region Здания

        private List<VisualBuilding> buildings;

        private void BuildingsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var building in e.NewItems)
                            AddBuilding(building as Building);
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var building in e.OldItems)
                            RemoveBuilding(building as Building);
                        break;
                    }
            }
        }

        private void AddBuilding(Building building)
        {
            var clone = buildings.FirstOrDefault(x => x.Model == building);
            if (clone != null) return;

            buildings.Add(new VisualBuilding(building));
            foreach (var floor in building.Floors)
                AddFloor(floor);
            building.Floors.CollectionChanged += FloorsCollectionChanged;
        }

        private void RemoveBuilding(Building building)
        {
            building.Floors.CollectionChanged -= FloorsCollectionChanged;
            foreach (var floor in building.Floors)
                RemoveFloor(floor);

            var visual = buildings.FirstOrDefault(x => x.Model == building);
            buildings.Remove(visual);
        }

        #endregion

        #region Этажи

        private List<VisualFloor> floors;

        private VisualFloor CurrentFloor
        {
            get { return currentFloor; }
            set
            {
                if (currentFloor == value) return;
                currentFloor = value;
                visuals.Clear();
                IsCreatingLine = false;
                if (currentFloor == null) return;
                visuals.Add(currentFloor);
            }
        }
        private VisualFloor currentFloor;
        private void FloorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var floor in e.NewItems)
                            AddFloor(floor as Floor);
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var floor in e.OldItems)
                            RemoveFloor(floor as Floor);
                        break;
                    }
            }
        }

        private void AddFloor(Floor floor)
        {
            var clone = floors.FirstOrDefault(x => x.Model == floor);
            if (clone != null) return;

            floors.Add(new VisualFloor(floor));
        }

        private void RemoveFloor(Floor floor)
        {
            floor.ParentBuilding.Floors.CollectionChanged -= FloorsCollectionChanged;

            var visual = floors.FirstOrDefault(x => x.Model == floor);
            floors.Remove(visual);

            if (floor.ParentBuilding.IsEmpty)
                CurrentFloor = null;
        }

        #endregion

        #region Базовые

        private VisualCollection visuals;
        protected override int VisualChildrenCount { get { return visuals.Count; } }
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= visuals.Count) { throw new ArgumentOutOfRangeException(); }

            return visuals[index];
        }
        #endregion
    }
}
