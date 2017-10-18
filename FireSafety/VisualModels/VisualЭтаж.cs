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
    class VisualЭтаж : DrawingVisual
    {
        public Floor Модель { get; private set; }
        public List<VisualОбъект> Обьекты { get; private set; }
        public VisualЭтаж(Floor модель)
        {
            Модель = модель;

            this.Обьекты = new List<VisualОбъект>();

            CreateBackground();
            Модель.PropertyChanged += ЭтажPropertyChanged;
            Модель.Objects.CollectionChanged += ОбъектыCollectionChanged;
            Модель.ДатьСхемуЭвакуации = ДатьСхемаЭвакуации;
        }

        private СхемаЭвакуации ДатьСхемаЭвакуации()
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                //var imgEncoder = new JpegBitmapEncoder();
                var imgEncoder = new PngBitmapEncoder();
                var bmpSource = new RenderTargetBitmap(Width, Height, 96, 96, PixelFormats.Pbgra32);
                bmpSource.Render(this);
                imgEncoder.Frames.Add(BitmapFrame.Create(bmpSource));

                imgEncoder.Save(memStream);
                var bitmap = new System.Drawing.Bitmap(memStream);

                var clone = bitmap.Clone(область, bitmap.PixelFormat);

                var converter = new System.Drawing.ImageConverter();
                return new СхемаЭвакуации(Ширина, Высота, (byte[])converter.ConvertTo(clone, typeof(byte[])));
            }
        }

        private void ОбъектыCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    var объект = Обьекты.FirstOrDefault(x => x.Модель == item);
                    if (объект == null) continue;

                    Обьекты.Remove(объект);
                    Children.Remove(объект);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (Обьекты.FirstOrDefault(x => x.Модель == item) != null) continue;

                    if (item.GetType() == typeof(УзелЛестницы))
                    {
                        var узел = item as УзелЛестницы;
                        ДобавитьОбъект(new VisualУзел(узел, узел.Position, this));
                    }
                }
            }
        }
        private void ЭтажPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "План":
                    {
                        CreateBackground();
                        break;
                    }
            }
        }
        private double Ширина { get { return this.Drawing.Bounds.Width; } }
        private double Высота { get { return this.Drawing.Bounds.Height; } }

        #region Фон
        private const int Width = 1920;
        private const int Height = 1080;
        private System.Drawing.Rectangle область;
        private void CreateBackground(bool теньАктивна = true)
        {
            //using (DrawingContext dc = this.RenderOpen())
            //{
            //    var прямоугольник = new Rect(0, 0, Width, Height);
            //    dc.DrawRectangle(Brushes.White, null, прямоугольник);
            //    dc.DrawRectangle(null, new Pen(Brushes.Black, 1), прямоугольник);

            //    if (Модель.План == null) return;

            //    double w = Width;
            //    double h = Height;

            //    if (Модель.План.Width * Height / Модель.План.Height > Width)
            //        h = Модель.План.Height * Width / Модель.План.Width;
            //    else
            //        w = Модель.План.Width * Height / Модель.План.Height;

            //    dc.DrawImage(Модель.План, new Rect((Width - w) / 2, (Height - h) / 2, w, h));
            //}

            using (DrawingContext dc = this.RenderOpen())
            {
                double ширина = Width;
                double высота = Height;

                if (Модель.План != null)
                {
                    if (Модель.План.Width * Height / Модель.План.Height > Width)
                        высота = Модель.План.Height * Width / Модель.План.Width;
                    else
                        ширина = Модель.План.Width * Height / Модель.План.Height;
                }
                var прямоугольник = new Rect((Width - ширина) / 2, (Height - высота) / 2, ширина, высота);
                область = new System.Drawing.Rectangle((int)прямоугольник.Location.X, (int)прямоугольник.Location.Y, (int)прямоугольник.Width, (int)прямоугольник.Height);
                if (теньАктивна)
                {
                    var вектор = new Vector(10, 10);
                    dc.DrawRectangle(Brushes.Gray, null, new Rect(прямоугольник.TopLeft + вектор, прямоугольник.BottomRight + вектор));
                }


                if (Модель.План == null)
                    dc.DrawRectangle(Brushes.White, new Pen(Brushes.Black, 1), прямоугольник);
                else
                    dc.DrawImage(Модель.План, прямоугольник);
            }
        }

        #endregion
        public void ДобавитьОбъект(VisualОбъект объект)
        {
            if (объект == null || Обьекты.Contains(объект) || Обьекты.FirstOrDefault(x => x.Модель == объект.Модель) != null) return;

            if (объект is VisualУчастокПути)
            {
                // ДобавитьУчасток(объект as VisualУчастокПути);
                Обьекты.Insert(0, объект);
                Children.Insert(0, объект);
            }
            else
            {
                Обьекты.Add(объект);
                Children.Add(объект);
            }
            Модель.AddObject(объект.Модель);

        }
        public void УдалитьОбъект(VisualОбъект объект)
        {
            if (объект == null) return;
            Модель.RemoveObject(объект.Модель);
        }
        public VisualОбъект ДатьОбъект(Point позиция)
        {
            //Поиск в обратном порядке: приоритет на верхних объектах

            for (int i = Обьекты.Count - 1; i >= 0; i--)
                if (Обьекты[i].Содержит(позиция)) return Обьекты[i];
            //foreach (var объект in Обьекты)
            //    if (объект.Содержит(позиция)) return объект;

            return null;
        }

        #region Путевые Дела
        //private void ДобавитьУчасток(VisualУчастокПути участок)
        //{
        //    bool участокПристроен = false;

        //    foreach (var путь in Пути.ToArray())
        //    {
        //        if (путь.КонечныйУзел == участок.Начало)
        //        {
        //            путь.ДобавитьУчасток(участок);
        //            ПостроениеПути(путь);
        //            участокПристроен = true;
        //        }
        //        else if (путь.СодержитУзел(участок.Начало))
        //        {
        //            var новыйПуть = ДубликатПути(путь, участок.Начало);
        //            новыйПуть.ДобавитьУчасток(участок);
        //            ДобавитьПуть(новыйПуть);
        //            ПостроениеПути(новыйПуть);
        //            участокПристроен = true;
        //        }
        //    }

        //    if (участокПристроен) return;
        //    else
        //    {
        //        var путь = new VisualПуть(new Путь(this.Модель.РодительскоеЗдание), this);
        //        путь.ДобавитьУчасток(участок);
        //        ДобавитьПуть(путь);
        //        ПостроениеПути(путь);
        //    }
        //}
        //private void УдалитьУчасток(VisualУчастокПути участок)
        //{
        //    foreach (var путь in Пути.ToArray())
        //    {
        //        if (путь.УчасткиПути.Contains(участок))
        //        {
        //            var индекс = путь.УчасткиПути.IndexOf(участок);
        //            if (индекс == 0 || индекс == путь.УчасткиПути.Count - 1)
        //            {
        //                путь.УчасткиПути.Remove(участок);
        //                //(путь.Модель as Путь).УдалитьКрайнийУчасток(участок.Модель as УчастокПути);
        //                return;
        //            }
        //        }
        //    }
        //}
        //private void ДобавитьПуть(VisualПуть путь)
        //{
        //    Пути.Add(путь);
        //    Модель.ДобавитьОбъект(путь.Модель);
        //}
        //private void ПостроениеПути(VisualПуть путь)
        //{
        //    if (!путь.УчасткиПути.Any()) return;
        //    var узел = путь.КонечныйУзел;

        //    if (!узел.ИсходящиеПути.Any()) return;
        //    if (узел.ИсходящиеПути.Count > 1)
        //        for (int i = 1; i < узел.ИсходящиеПути.Count; i++)
        //        {
        //            var новыйПуть = ДубликатПути(путь);
        //            новыйПуть.ДобавитьУчасток(узел.ИсходящиеПути[i]);
        //            Пути.Add(новыйПуть);
        //            ПостроениеПути(новыйПуть);
        //        }
        //    else
        //    {
        //        путь.ДобавитьУчасток(узел.ИсходящиеПути.First());
        //        ПостроениеПути(путь);
        //    }
        //}
        //private VisualПуть ДубликатПути(VisualПуть путь, VisualУзел узел = null)
        //{
        //    var новыйПуть = new VisualПуть(new Путь(this.Модель.РодительскоеЗдание), this);
        //    foreach (var участок in путь.УчасткиПути)
        //    {
        //        новыйПуть.ДобавитьУчасток(участок);
        //        if (участок.Конец == узел) return новыйПуть;
        //    }
        //    return новыйПуть;
        //}

        #endregion    

        #region Операторы сравнения
        //public static bool operator ==(VisualЭтаж этаж1, Этаж этаж2)
        //{
        //    if (object.ReferenceEquals(этаж1, null) && object.ReferenceEquals(этаж2, null))
        //        return true;
        //    if (object.ReferenceEquals(этаж1, null) || object.ReferenceEquals(этаж2, null))
        //        return false;

        //    return этаж1.Модель == этаж2;
        //}

        //public static bool operator !=(VisualЭтаж этаж1, Этаж этаж2)
        //{
        //    return !(этаж1 == этаж2);
        //}
        //public static bool operator ==(Этаж этаж1, VisualЭтаж этаж2)
        //{
        //    return этаж2 == этаж1;
        //}
        //public static bool operator !=(Этаж этаж1, VisualЭтаж этаж2)
        //{
        //    return этаж2 != этаж1;
        //}

        #endregion
    }
}
