using FireSafety.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FireSafety.VisualModels
{
    class ЗданиеControl : FrameworkElement
    {

        #region Свойство Здания

        public static readonly DependencyProperty BuildingsProperty = DependencyProperty.Register("Здания", typeof(ObservableCollection<Building>), typeof(ЗданиеControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(BuildingsPropertyChanged)));
        private static void BuildingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ЗданиеControl).ПрименениеНовыхЗначений(e.NewValue as ObservableCollection<Building>);
        }

        public ObservableCollection<Building> Здания
        {
            get { return (ObservableCollection<Building>)GetValue(BuildingsProperty); }
            set { SetValue(BuildingsProperty, value); }
        }

        #endregion

        #region Свойство Режим Взаимодействия
        public static readonly DependencyProperty ModeActionProperty = DependencyProperty.Register("РежимВзаимодействия", typeof(РежимДействия), typeof(ЗданиеControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(ModeActionPropertyChanged)));
        private static void ModeActionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
                (d as ЗданиеControl).ИдетПостроениеПути = false;
            //if (((РежимДействия)e.OldValue) == РежимДействия.ПостроениеПутиЭвакуации && ((РежимДействия)e.NewValue) != РежимДействия.ПостроениеПутиЭвакуации)
            //    (d as ЗданиеControl).ИдетПостроениеПути = false;
        }

        public РежимДействия РежимВзаимодействия
        {
            get { return (РежимДействия)GetValue(ModeActionProperty); }
            set { SetValue(ModeActionProperty, value); }
        }

        #endregion

        #region Свойство Текущий Этаж
        public static readonly DependencyProperty SelectedFloorProperty = DependencyProperty.Register("ВыбранныйЭтаж", typeof(Floor), typeof(ЗданиеControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(SelectedFloorPropertyChanged)));
        private static void SelectedFloorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var выбранныйЭтаж = e.NewValue as Floor;
            var control = d as ЗданиеControl;
            if (выбранныйЭтаж == null)
                control.ТекущийЭтаж = null;
            else
                control.ТекущийЭтаж = control.этажи.FirstOrDefault(x => x.Модель == выбранныйЭтаж);
        }
        public Floor ВыбранныйЭтаж
        {
            get { return (Floor)GetValue(SelectedFloorProperty); }
            set { SetValue(SelectedFloorProperty, value); }
        }

        #endregion

        public ЗданиеControl()
        {
            этажи = new List<VisualЭтаж>();
            здания = new List<VisualЗдание>();

            this.обьекты = new VisualCollection(this);
            this.Width = 1920;
            this.Height = 1080;
            ОбнулениеЗначений();
        }

        #region Перемещение

        private ИнформацияПеремещения информацияПеремещения = null;
        private bool ИдетПеремещение
        {
            get { return информацияПеремещения != null; }
            set { if (value == false) информацияПеремещения = null; }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (ИдетПеремещение)
            {
                e.Handled = true;
                var текущаяПозиция = e.GetPosition(this);
                var сдвиг = текущаяПозиция - информацияПеремещения.НачальнаяПозиция;
                информацияПеремещения.Элемент.Перемещение(сдвиг);

                информацияПеремещения.НачальнаяПозиция = текущаяПозиция;
            }
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            ИдетПеремещение = false;
        }

        #endregion

        #region ПостроениеПути

        private VisualУзел НачалоУчастка = null;
        private bool ИдетПостроениеПути
        {
            get { return НачалоУчастка != null; }
            set { if (value == false) НачалоУчастка = null; }
        }
        private void СоздатьУчастокПути(Point позиция)
        {
            VisualУзел узел = null;
            var объект = ТекущийЭтаж.ДатьОбъект(позиция);

            if (объект is VisualУзел)
            {
                узел = объект as VisualУзел;
            }
            else
            {
                switch (РежимВзаимодействия)
                {
                    case РежимДействия.ДобавитьЛестницу:
                        узел = new VisualУзелЛестницы(new УзелЛестницы(ТекущийЭтаж.Модель, позиция), позиция, ТекущийЭтаж); break;
                    case РежимДействия.ПостроениеПутиЭвакуации:
                        узел = new VisualУзел(new УзелПути(ТекущийЭтаж.Модель, позиция), позиция, ТекущийЭтаж); break;
                }
                ДобавитьОбьект(узел);

            }

            if (ИдетПостроениеПути)
            {
                if (!узел.УзелМодель.IncomingSectionsAllowed) return;
                if (НачалоУчастка == узел) return;
                VisualУчастокПути участок = null;

                switch (РежимВзаимодействия)
                {
                    case РежимДействия.ДобавитьЛестницу:
                        участок = new VisualУчастокЛестницы(НачалоУчастка, узел, new УчастокЛестницы(НачалоУчастка.УзелМодель, узел.УзелМодель, ТекущийЭтаж.Модель), ТекущийЭтаж); break;
                    case РежимДействия.ПостроениеПутиЭвакуации:
                        участок = new VisualУчастокПути(НачалоУчастка, узел, new УчастокПути(НачалоУчастка.УзелМодель, узел.УзелМодель, ТекущийЭтаж.Модель), ТекущийЭтаж); break;
                }

                ДобавитьОбьект(участок);
                НачалоУчастка = узел;
            }
            else
            {
                НачалоУчастка = узел;
            }

            if (!НачалоУчастка.УзелМодель.OutgoingSectionsAllowed)
                ИдетПостроениеПути = false;
        }

        #endregion

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (ТекущийЭтаж == null) return;
            e.Handled = true;
            var позиция = e.GetPosition(this);
            try
            {
                switch (РежимВзаимодействия)
                {
                    case РежимДействия.ДобавитьГруппуМобильности:
                        {
                            var старт = new StartNode(ТекущийЭтаж.Модель, позиция);
                            ДобавитьОбьект(new VisualСтарт(старт, позиция, ТекущийЭтаж)); break;
                        }
                    case РежимДействия.ДобавитьВыход:
                        {
                            var выход = new ExitNode(ТекущийЭтаж.Модель, позиция);
                            ДобавитьОбьект(new VisualВыход(выход, позиция, ТекущийЭтаж)); break;
                        }
                    case РежимДействия.ДобавитьДвернойПроем:
                        {
                            var дверь = new EntryNode(ТекущийЭтаж.Модель, позиция);
                            ДобавитьОбьект(new VisualДверь(дверь, позиция, ТекущийЭтаж)); break;
                        }
                    case РежимДействия.ДобавитьЛестницу:
                    case РежимДействия.ПостроениеПутиЭвакуации:
                        {
                            СоздатьУчастокПути(позиция);
                            break;
                        }
                    case РежимДействия.Перемещение:
                        {
                            var объект = ТекущийЭтаж.ДатьОбъект(позиция);
                            if (объект != null)
                            {
                                e.Handled = true;
                                информацияПеремещения = new ИнформацияПеремещения(объект.ДатьЭлемент(позиция), позиция);
                                объект.Модель.IsSelected = true;
                            }
                            else
                                e.Handled = false;

                            break;
                        }
                    case РежимДействия.Удаление:
                        {
                            var объект = ТекущийЭтаж.ДатьОбъект(позиция);
                            ТекущийЭтаж.УдалитьОбъект(объект);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        //{
        //    BitmapEncoder imgEncoder = new PngBitmapEncoder();

        //    var fileName = @"C:\t.png";

        //    RenderTargetBitmap bmpSource = new RenderTargetBitmap((int)Width, (int)Height, 96, 96, PixelFormats.Pbgra32);
        //    bmpSource.Render(ТекущийЭтаж);
        //    imgEncoder.Frames.Add(BitmapFrame.Create(bmpSource));
        //    using (System.IO.Stream stream = System.IO.File.Create(fileName))
        //    {
        //        imgEncoder.Save(stream);
        //        stream.Close();
        //    }

        //}

        private List<VisualЭтаж> этажи;
        private List<VisualЗдание> здания;
        private VisualЭтаж ТекущийЭтаж
        {
            get { return текущийЭтаж; }
            set
            {
                if (текущийЭтаж == value) return;
                текущийЭтаж = value;
                обьекты.Clear();
                ИдетПостроениеПути = false;
                if (текущийЭтаж == null) return;
                обьекты.Add(текущийЭтаж);
            }
        }
        private VisualЭтаж текущийЭтаж;
        private void ОбнулениеЗначений()
        {
            ТекущийЭтаж = null;
            ИдетПеремещение = false;
            ИдетПостроениеПути = false;

            этажи.Clear();

            if (!здания.Any()) return;
            foreach (var здание in здания)
                здание.Модель.Floors.CollectionChanged -= ЭтажиCollectionChanged;
            здания.Clear();
        }
        private void ДобавитьОбьект(VisualОбъект объект)
        {
            if (ТекущийЭтаж == null) return;
            ТекущийЭтаж.ДобавитьОбъект(объект);
        }
        private void ПрименениеНовыхЗначений(ObservableCollection<Building> новыеЗдания)
        {
            ОбнулениеЗначений();

            if (здания == null) return;
            новыеЗдания.CollectionChanged += ЗданияCollectionChanged;
            foreach (var здание in новыеЗдания)
                ДобавитьНовоеЗдание(здание);

            ТекущийЭтаж = этажи.FirstOrDefault();
        }
        private void ЗданияCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var здание in e.NewItems)
                            ДобавитьНовоеЗдание(здание as Building);
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        break;
                    }
            }
        }
        private void ДобавитьНовоеЗдание(Building здание)
        {
            var дубликат = здания.FirstOrDefault(x => x.Модель == здание);
            if (дубликат != null) return;

            здания.Add(new VisualЗдание(здание));
            foreach (var этаж in здание.Floors)
                ДобавитьЭтаж(этаж);
            здание.Floors.CollectionChanged += ЭтажиCollectionChanged;
        }
        private void ЭтажиCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var этаж in e.NewItems)
                            ДобавитьЭтаж(этаж as Floor);
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var этаж in e.OldItems)
                            УдалитьЭтаж(этаж as Floor);
                        break;
                    }
            }
        }
        private void ДобавитьЭтаж(Floor этаж)
        {
            var дубликат = этажи.FirstOrDefault(x => x.Модель == этаж);
            if (дубликат != null) return;

            этажи.Add(new VisualЭтаж(этаж));
        }
        private void УдалитьЭтаж(Floor этаж)
        {
            var visual = этажи.FirstOrDefault(x => x.Модель == этаж);
            этажи.Remove(visual);
        }

        #region Базовые

        private VisualCollection обьекты;
        protected override int VisualChildrenCount { get { return обьекты.Count; } }
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= обьекты.Count) { throw new ArgumentOutOfRangeException(); }

            return обьекты[index];
        }
        #endregion
    }
}
