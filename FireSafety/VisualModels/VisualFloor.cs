using FireSafety.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FireSafety.VisualModels
{
    class VisualFloor : DrawingVisual
    {
        public Floor Model { get; private set; }

        public VisualFloor(Floor model)
        {
            Model = model;

            this.VisualEntities = new List<VisualEntity>();

            CreateBackground();
            Model.PropertyChanged += ModelPropertyChanged;
            Model.Objects.CollectionChanged += EntitiesCollectionChanged;
            Model.GetEvacuationPlanImage = GetEvacuationPlanImage;

            foreach (var obj in Model.Objects)
                AddEntity(obj);
        }

        /// <summary>
        /// Точка симметрии. графический центр
        /// </summary>
        public Point AxisPoint
        {
            get { return new Point(Width / 2, Height / 2); }
        }
        public double ActualWidth { get { return Drawing.Bounds.Width; } }
        public double ActualHeight { get { return Drawing.Bounds.Height; } }

        #region Фон

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "FloorPlanImage": { CreateBackground(); break; }
            }
        }

        private const int Width = 1920;
        private const int Height = 1080;
        /// <summary>
        /// прямоугольная область, где располагаются визуальные обьекты этажа
        /// </summary>
        private Int32Rect cropedRect = new Int32Rect(0, 0, Width, Height);
        private void CreateBackground(bool shadowIsActive = true)
        {
            using (var dc = RenderOpen())
            {
                double width = Width;
                double height = Height;

                if (Model.FloorPlanImage != null)
                {
                    if (Model.FloorPlanImage.Width * Height / Model.FloorPlanImage.Height > Width)
                        height = Model.FloorPlanImage.Height * Width / Model.FloorPlanImage.Width;
                    else
                        width = Model.FloorPlanImage.Width * Height / Model.FloorPlanImage.Height;
                }
                var rect = new Rect((Width - width) / 2, (Height - height) / 2, width, height);
                cropedRect = new Int32Rect((int)rect.Location.X, (int)rect.Location.Y, (int)rect.Width, (int)rect.Height);


                if (shadowIsActive)
                {
                    var vector = new Vector(10, 10);
                    dc.DrawRectangle(Brushes.Gray, null, new Rect(rect.TopLeft + vector, rect.BottomRight + vector));
                }


                if (Model.FloorPlanImage == null)
                    dc.DrawRectangle(Brushes.White, new Pen(Brushes.Black, 1), rect);
                else
                    dc.DrawImage(Model.FloorPlanImage, rect);
            }
        }

        /// <summary>
        /// Функция, предоставляющая графический снимок этажа, включающий все графические обьекты и фон
        /// </summary>
        /// <returns></returns>
        private EvacuationPlanImage GetEvacuationPlanImage()
        {
            //исходная область этажа
            var bmpSource = new RenderTargetBitmap(Width, Height, 96, 96, PixelFormats.Pbgra32);
            bmpSource.Render(this);

            //область, включающая схему этажа
            var croped = new CroppedBitmap(bmpSource, cropedRect);
            return new EvacuationPlanImage(bmpSource);
        }

        #endregion

        #region Обьекты этажа

        public List<VisualEntity> VisualEntities { get; private set; }

        private void EntitiesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    var obj = VisualEntities.FirstOrDefault(x => x.Model == item);
                    if (obj == null) continue;

                    VisualEntities.Remove(obj);
                    Children.Remove(obj);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (VisualEntities.Any(x => x.Model == item)) continue;
                    AddEntity(item as Entity);
                }
            }
        }

        private void AddEntity(Entity obj)
        {
            if (obj is StartNode) { AddVisual(new VisualStartNode(obj as StartNode, this)); return; }
            if (obj is EntryNode) { AddVisual(new VisualEntryNode(obj as EntryNode, this)); return; }
            if (obj is ExitNode) { AddVisual(new VisualExitNode(obj as ExitNode, this)); return; }
            if (obj is StairsNode) { AddVisual(new VisualStairsNode(obj as StairsNode, this)); return; }
            if (obj is RoadNode) { AddVisual(new VisualNode(obj as RoadNode, this)); return; }

            if (obj is Section)
            {
                var first = (VisualNode)VisualEntities.Where(x => x.Model is Node).FirstOrDefault(x => x.Model == (obj as Section).First);
                var last = (VisualNode)VisualEntities.Where(x => x.Model is Node).FirstOrDefault(x => x.Model == (obj as Section).Last);
                if (first != null && last != null)
                {
                    if (obj is RoadSection)
                        AddVisual(new VisualRoadSection(first, last, obj as RoadSection, this));
                    else if (obj is StairsSection)
                        AddVisual(new VisualStairsSection(first, last, obj as StairsSection, this));
                }
            }
        }

        public void AddVisualEntity(VisualEntity entity)
        {
            AddVisual(entity);
            Model.AddObject(entity.Model);
        }

        private void AddVisual(VisualEntity entity)
        {
            if (entity == null || VisualEntities.Contains(entity) || VisualEntities.Any(x => x.Model == entity.Model)) return;

            if (entity.Model is Section)
            {
                VisualEntities.Insert(0, entity);
                Children.Insert(0, entity);
            }
            else
            {
                VisualEntities.Add(entity);
                Children.Add(entity);
            }
        }

        public void RemoveVisualEntity(VisualEntity entity)
        {
            if (entity == null) return;

            //удаляем элемент в модели, 
            //и через изменении коллеции модели происходит удаление самого визуала
            Model.RemoveObject(entity.Model);
        }

        public VisualEntity GetVisualEntity(Point position)
        {
            //Поиск в обратном порядке: приоритет на верхних объектах

            for (int i = VisualEntities.Count - 1; i >= 0; i--)
                if (VisualEntities[i].Contains(position)) return VisualEntities[i];

            return null;
        }

        #endregion

        public void ApplyScale()
        {
            foreach (var entuty in VisualEntities)
                entuty.ApplyScale(Model.Scale);
        }
    }
}