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
                (d as EvacuationControl).IsMakingSection = false;
        }

        /// <summary>
        /// SelectedActionMode
        /// </summary>
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
            var выбранныйЭтаж = e.NewValue as Floor;
            var control = d as EvacuationControl;
            if (выбранныйЭтаж == null)
                control.CurrentFloor = null;
            else
                control.CurrentFloor = control.floors.FirstOrDefault(x => x.Model == выбранныйЭтаж);
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

            if (IsSetScale)
            {
                lastPointScale = e.GetPosition(this);
                DrawScaleSection();
            }
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            IsMoving = false;

            if (IsSetScale)
            {
                var length = (lastPointScale - firstPointScale).Length;
                if (length > 0)
                {
                    var win = new ScaleWindow(length, CurrentFloor.Model.Scale);
                    if (win.ShowDialog() == true)
                    {
                        CurrentFloor.Model.Scale = win.Scale;
                        CurrentFloor.ApplyScale();
                    }
                }
                visuals.Remove(scaleSection);
                scaleSection = null;
            }
        }

        #endregion

        #region Построение отрезков

        private VisualNode startSection = null;
        private bool IsMakingSection
        {
            get { return startSection != null; }
            set { if (value == false) startSection = null; }
        }
        private void MakeSection(Point position)
        {
            VisualNode node = null;
            var entity = CurrentFloor.GetVisualEntity(position);
            if (entity is VisualNode)
            {
                node = entity as VisualNode;
            }
            else
            {
                switch (SelectedActionMode)
                {
                    case ActionMode.AddStairs:
                        node = new VisualStairsNode(new StairsNode(CurrentFloor.Model, position), position, CurrentFloor); break;
                    case ActionMode.AddRoad:
                        node = new VisualNode(new RoadNode(CurrentFloor.Model, position), position, CurrentFloor); break;
                }
                AddVisualEntity(node);

            }

            if (IsMakingSection)
            {
                if (!node.NodeModel.IncomingSectionsAllowed) return;
                if (startSection == node) return;
                VisualRoadSection участок = null;

                switch (SelectedActionMode)
                {
                    case ActionMode.AddStairs:
                        участок = new VisualStairsSection(startSection, node, new StairsSection(startSection.NodeModel, node.NodeModel, CurrentFloor.Model), CurrentFloor); break;
                    case ActionMode.AddRoad:
                        участок = new VisualRoadSection(startSection, node, new RoadSection(startSection.NodeModel, node.NodeModel, CurrentFloor.Model), CurrentFloor); break;
                }

                AddVisualEntity(участок);
                startSection = node;
            }
            else
            {
                startSection = node;
            }

            if (!startSection.NodeModel.OutgoingSectionsAllowed)
                IsMakingSection = false;
        }

        #endregion

        #region Масштабирование

        private bool IsSetScale
        {
            get { return SelectedActionMode == ActionMode.SetScale && scaleSection != null; }
        }

        private Point firstPointScale;
        private Point lastPointScale;
        private DrawingVisual scaleSection;
        private void DrawScaleSection()
        {
            if (scaleSection == null) return;
            using (DrawingContext dc = scaleSection.RenderOpen())
            {
                var radius = 10;
                var pen = new Pen(Brushes.Black, radius / 5);

                dc.DrawEllipse(null, pen, firstPointScale, radius, radius);
                dc.DrawLine(pen, firstPointScale + new Vector(-radius, 0), firstPointScale + new Vector(radius, 0));
                dc.DrawLine(pen, firstPointScale + new Vector(0, -radius), firstPointScale + new Vector(0, radius));

                dc.DrawEllipse(null, pen, lastPointScale, radius, radius);
                dc.DrawLine(pen, lastPointScale + new Vector(-radius, 0), lastPointScale + new Vector(radius, 0));
                dc.DrawLine(pen, lastPointScale + new Vector(0, -radius), lastPointScale + new Vector(0, radius));

                dc.DrawLine(new Pen(Brushes.Black, radius / 5) { DashStyle = new DashStyle(new double[] { radius / 2, radius / 2 }, radius / 2) }, firstPointScale, lastPointScale);
            }
        }

        #endregion

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
                            AddVisualEntity(new VisualStartNode(start, position, CurrentFloor)); break;
                        }
                    case ActionMode.AddExit:
                        {
                            var exit = new ExitNode(CurrentFloor.Model, position);
                            AddVisualEntity(new VisualExitNode(exit, position, CurrentFloor)); break;
                        }
                    case ActionMode.AddEntry:
                        {
                            var entry = new EntryNode(CurrentFloor.Model, position);
                            AddVisualEntity(new VisualEntryNode(entry, position, CurrentFloor)); break;
                        }
                    case ActionMode.AddStairs:
                    case ActionMode.AddRoad:
                        {
                            MakeSection(position);
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
                            firstPointScale = position;
                            lastPointScale = position;

                            scaleSection = new DrawingVisual();
                            visuals.Add(scaleSection);
                            DrawScaleSection();
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<VisualFloor> floors;
        private List<VisualBuilding> buildings;
        private VisualFloor CurrentFloor
        {
            get { return currentFloor; }
            set
            {
                if (currentFloor == value) return;
                currentFloor = value;
                visuals.Clear();
                IsMakingSection = false;
                if (currentFloor == null) return;
                visuals.Add(currentFloor);
            }
        }
        private VisualFloor currentFloor;

        private void ToDefaultValues()
        {
            CurrentFloor = null;
            IsMoving = false;
            IsMakingSection = false;

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
                        break;
                    }
            }
        }
        private void AddBuilding(Building building)
        {
            var дубликат = buildings.FirstOrDefault(x => x.Model == building);
            if (дубликат != null) return;

            buildings.Add(new VisualBuilding(building));
            foreach (var floor in building.Floors)
                AddFloor(floor);
            building.Floors.CollectionChanged += FloorsCollectionChanged;
        }
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
            var visual = floors.FirstOrDefault(x => x.Model == floor);
            floors.Remove(visual);
        }

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
