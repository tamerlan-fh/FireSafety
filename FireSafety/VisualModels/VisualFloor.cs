using FireSafety.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FireSafety.VisualModels
{
    class VisualFloor : DrawingVisual
    {
        public Floor Model { get; private set; }
        public List<VisualEntity> VisualEntities { get; private set; }
        public VisualFloor(Floor модель)
        {
            Model = модель;

            this.VisualEntities = new List<VisualEntity>();

            CreateBackground();
            Model.PropertyChanged += ModelPropertyChanged;
            Model.Objects.CollectionChanged += EntitiesCollectionChanged;
            Model.GetEvacuationPlanImage = GetEvacuationPlanImage;
        }

        private EvacuationPlanImage GetEvacuationPlanImage()
        {
            //исходная область этажа
            var bmpSource = new RenderTargetBitmap(CWidth, CHeight, 96, 96, PixelFormats.Pbgra32);
            bmpSource.Render(this);

            //область, включающая схему этажа
            var croped = new CroppedBitmap(bmpSource, cropedRect);

            return new EvacuationPlanImage(croped);
        }

        private void EntitiesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    var объект = VisualEntities.FirstOrDefault(x => x.Model == item);
                    if (объект == null) continue;

                    VisualEntities.Remove(объект);
                    Children.Remove(объект);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (VisualEntities.FirstOrDefault(x => x.Model == item) != null) continue;

                    if (item.GetType() == typeof(StairsNode))
                    {
                        var узел = item as StairsNode;
                        AddVisualEntity(new VisualNode(узел, узел.Position, this));
                    }
                }
            }
        }
        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "FloorPlanImage": { CreateBackground(); break; }
            }
        }
        private double Width { get { return this.Drawing.Bounds.Width; } }
        private double Height { get { return this.Drawing.Bounds.Height; } }

        #region Фон

        private const int CWidth = 1920;
        private const int CHeight = 1080;
        private Int32Rect cropedRect = new Int32Rect(0, 0, CWidth, CHeight);
        private void CreateBackground(bool shadowIsActive = true)
        {
            using (DrawingContext dc = this.RenderOpen())
            {
                double width = CWidth;
                double height = CHeight;

                if (Model.FloorPlanImage != null)
                {
                    if (Model.FloorPlanImage.Width * CHeight / Model.FloorPlanImage.Height > CWidth)
                        height = Model.FloorPlanImage.Height * CWidth / Model.FloorPlanImage.Width;
                    else
                        width = Model.FloorPlanImage.Width * CHeight / Model.FloorPlanImage.Height;
                }
                var rect = new Rect((CWidth - width) / 2, (CHeight - height) / 2, width, height);
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

        #endregion
        public void AddVisualEntity(VisualEntity entity)
        {
            if (entity == null || VisualEntities.Contains(entity) || VisualEntities.FirstOrDefault(x => x.Model == entity.Model) != null) return;

            if (entity is VisualRoadSection)
            {
                // ДобавитьУчасток(объект as VisualУчастокПути);
                VisualEntities.Insert(0, entity);
                Children.Insert(0, entity);
            }
            else
            {
                VisualEntities.Add(entity);
                Children.Add(entity);
            }
            Model.AddObject(entity.Model);

        }
        public void RemoveVisualEntity(VisualEntity entity)
        {
            if (entity == null) return;
            Model.RemoveObject(entity.Model);
        }
        public VisualEntity GetVisualEntity(Point position)
        {
            //Поиск в обратном порядке: приоритет на верхних объектах

            for (int i = VisualEntities.Count - 1; i >= 0; i--)
                if (VisualEntities[i].Contains(position)) return VisualEntities[i];

            return null;
        }
    }
}
