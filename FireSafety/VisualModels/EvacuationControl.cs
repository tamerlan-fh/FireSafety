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
using System.Windows.Controls;
using System.Globalization;

namespace FireSafety.VisualModels
{
    class EvacuationControl : FrameworkElement
    {

        #region Свойство Здания

        public static readonly DependencyProperty BuildingsProperty =
            DependencyProperty.Register("Buildings", typeof(ObservableCollection<Building>), typeof(EvacuationControl),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(BuildingsPropertyChanged)));
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

        public static readonly DependencyProperty SelectedActionModeProperty =
            DependencyProperty.Register("SelectedActionMode", typeof(ActionMode), typeof(EvacuationControl),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(SelectedActionModePropertyChanged)));
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

        #region Свойства, связанные с Масштабом

        /// <summary>
        /// Масштаб
        /// </summary>
        public double? Ratio
        {
            get
            {
                double? ratio = (double?)GetValue(RatioProperty);
                if (ratio != null)
                    ratio /= 100;
                return ratio;
            }
            set
            {

                double? ratio = value;
                if (ratio != null)
                    ratio = Math.Round((double)ratio * 100, 2);
                SetValue(RatioProperty, ratio);
            }
        }

        public static readonly DependencyProperty RatioProperty =
            DependencyProperty.Register("Ratio", typeof(double?), typeof(EvacuationControl),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(RatioPropertyChanged)));
        private static void RatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Вписать
        /// </summary>
        public bool IsInscribe
        {
            get { return (bool)GetValue(IsInscribeProperty); }
            set { SetValue(IsInscribeProperty, value); }
        }

        public static readonly DependencyProperty IsInscribeProperty =
                DependencyProperty.Register("IsInscribe", typeof(bool), typeof(EvacuationControl),
                    new FrameworkPropertyMetadata(new PropertyChangedCallback(IsInscribeChanged)));

        private static void IsInscribeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool && ((bool)e.NewValue))
                (d as EvacuationControl).Shell?.Inscribe();
        }
        #endregion

        #region Свойство Текущий Этаж

        public static readonly DependencyProperty SelectedFloorProperty =
            DependencyProperty.Register("SelectedFloor", typeof(Floor), typeof(EvacuationControl),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(SelectedFloorPropertyChanged)));
        private static void SelectedFloorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var floor = e.NewValue as Floor;
            var control = d as EvacuationControl;
            if (floor == null)
                control.CurrentFloor = null;
            else
                control.CurrentFloor = control.floors.FirstOrDefault(x => x.Model == floor);
        }

        public Floor SelectedFloor
        {
            get { return (Floor)GetValue(SelectedFloorProperty); }
            set { SetValue(SelectedFloorProperty, value); }
        }

        #endregion

        public EvacuationControl()
        {
            ClipToBounds = true;

            floors = new List<VisualFloor>();
            shells = new List<VisualShell>();
            buildings = new List<VisualBuilding>();

            visuals = new VisualCollection(this);

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
            base.OnMouseMove(e);

            if (!IsMoving) return;

            var position = e.GetPosition(this);
            if (!moveInformation.IsUsedAbsoluteValues)
                position = GetAbsolutePosition(position);

            var shift = position - moveInformation.StartPosition;

            if (moveInformation.IsUsedAbsoluteValues)
                shift = Shell.TransformVector(shift);

            moveInformation.Unit.Move(shift);

            moveInformation.StartPosition = position;
            e.Handled = true;
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
                    Shell?.RemoveVisual(creatingLineInf.Drawing);

                creatingLineInf = value;
                if (creatingLineInf != null)
                {
                    Shell?.AddVisual(creatingLineInf.Drawing);
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

        #region Задание масштаба

        DrawingVisual visualScale = new DrawingVisual();


        private Pen scalePen = new Pen(Brushes.Black, 2);

        private byte[] arrayUp = new byte[] { 2, 5, 10, 25, 50, 100 };
        private double[] arrayDown = new double[] { 0.5, 0.25, 0.2, 0.1, 0.05, 0.025, 0.005, 0.001 };

        public void DrawingScale()
        {
            IsInscribe = false;

            if (Shell == null)
            {
                visuals.Remove(visualScale);
                return;
            }

            if (!visuals.Contains(visualScale))
                visuals.Add(visualScale);

            double maxLength = 100;
            double minLength = 50;
            double coefficient = 1;
            var point = new Point(ActualWidth / 2, ActualHeight - 30);
            var length = Shell.Ratio * CurrentFloor.Model.Scale.TransformToGraphicLength(coefficient);

            if (length < minLength)
            {
                foreach (var value in arrayUp)
                    if (length * value > minLength)
                    {
                        coefficient = value;
                        break;
                    }
            }
            else if (length > maxLength)
            {
                foreach (var value in arrayDown)
                    if (length * value < maxLength)
                    {
                        coefficient = value;
                        break;
                    }
            }


            var vectorScale = new Vector(length * coefficient / 2, 0);
            var text = new FormattedText(string.Format("{0} м", Math.Round(coefficient, 3)), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Verdana"), 14, Brushes.Black);

            using (var dc = visualScale.RenderOpen())
            {
                var vector = new Vector(0, 3);
                var firstPoint = point - vectorScale;
                var lastPoint = point + vectorScale;


                dc.DrawLine(scalePen, firstPoint, lastPoint);
                dc.DrawLine(scalePen, firstPoint + vector, firstPoint - vector);
                dc.DrawLine(scalePen, lastPoint + vector, lastPoint - vector);

                dc.DrawText(text, point - new Vector(text.Width / 2, 0));
            }
        }

        private void SetScale()
        {
            if (!(SelectedActionMode == ActionMode.SetScale) || !IsCreatingLine) return;

            var length = CreatingLineInf.Length;
            if (length > 0)
            {
                var win = new ScaleWindow(length, CurrentFloor.Model.Scale);
                if (win.ShowDialog() == true)
                {
                    CurrentFloor.Model.Scale = win.Scale;
                    CurrentFloor.ApplyScale();
                    DrawingScale();
                }
            }

            IsCreatingLine = false;
        }

        #endregion

        #region графические трансформации

        private List<VisualShell> shells;
        private VisualShell Shell
        {
            get { return shell; }
            set
            {
                visuals.Clear();
                shell = value;
                if (Shell != null)
                {
                    Ratio = Shell.Ratio;
                    visuals.Add(Shell);
                    DrawingScale();
                }
                else
                    Ratio = null;
            }
        }
        private VisualShell shell;

        public Point AxisPoint { get { return new Point(ActualWidth / 2, ActualHeight / 2); } }

        private Point GetAbsolutePosition(Point point)
        {
            if (Shell == null)
                return point;
            else
                return Shell.TransformPoint(point);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
                Shell?.ToNearly(e.GetPosition(this));
            else
                Shell?.ToFar(e.GetPosition(this));

            DrawingScale();

            base.OnMouseWheel(e);
        }

        #endregion

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (IsCreatingLine)
            {
                IsCreatingLine = false;
                e.Handled = true;
            }

            base.OnMouseRightButtonDown(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (CurrentFloor == null) return;

            e.Handled = true;
            var position = GetAbsolutePosition(e.GetPosition(this));

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
                                moveInformation = new MoveInformation(entity.GetUnit(position), position);
                                entity.Model.IsSelected = true;
                            }
                            else if (Shell != null)
                            {
                                moveInformation = new MoveInformation(Shell, e.GetPosition(this), true);
                                e.Handled = false;
                            }
                            e.Handled = true;
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

            shells.Clear();
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
                IsCreatingLine = false;

                if (currentFloor != null)
                    Shell = shells.FirstOrDefault(x => x.Floor == CurrentFloor);
                else
                    Shell = null;
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

            var vFloor = new VisualFloor(floor);
            floors.Add(vFloor);
            shells.Add(new VisualShell(vFloor, this));
        }

        private void RemoveFloor(Floor floor)
        {
            var visual = floors.FirstOrDefault(x => x.Model == floor);
            floors.Remove(visual);

            var shell = shells.FirstOrDefault(x => x.Floor == visual);
            shells.Remove(shell);

            if (floor.ParentBuilding.IsEmpty)
                CurrentFloor = null;
        }

        #endregion

        #region Базовые

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            base.OnContextMenuOpening(e);
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.AliceBlue, new Pen(Brushes.Black, 1), new Rect(0, 0, this.ActualWidth, this.ActualHeight));

            DrawingScale();
            base.OnRender(drawingContext);
        }

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