using FireSafety.VisualModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    class Floor : Entity
    {
        public Func<СхемаЭвакуации> ДатьСхемуЭвакуации { get; set; }
        public Building РодительскоеЗдание { get { return Parent as Building; } }
        public Floor(Building parent) : this("Этаж", parent) { }
        public Floor(string title, Building parent) : base(title, parent)
        {
            Objects = new ObservableCollection<Entity>();
            ЗагрузитьПланЭтажаCommand = new RelayCommand(param => this.ЗагрузитьПланЭтажа());
        }
        public ICommand ЗагрузитьПланЭтажаCommand { get; protected set; }
        public int НомерЭтажа
        {
            get { return РодительскоеЗдание.Floors.IndexOf(this) + 1; }
        }
        public void ОбновитьНазвание()
        {
            this.Title = string.Format("Этаж {0}", НомерЭтажа);
        }
        private void ЗагрузитьПланЭтажа()
        {
            var openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "Pdf files (*.pdf)|*.pdf|Image files |*.png;*.bmp;*jpg;*.jepg|All files (*.*)|*.*";
            openFileDialog.Filter = "Image files |*.png;*.bmp;*jpg;*.jepg|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != true) return;

            var имяФайла = openFileDialog.FileName;

            var расширение = System.IO.Path.GetExtension(имяФайла).ToLower();
            switch (расширение)
            {
                //case ".pdf": ЗагрузитьPdf(имяФайла); break;
                case ".png":
                case ".bmp":
                case ".jpg":
                case ".jepg": ЗагрузитьКартинку(имяФайла); break;
                default: break;
            }
        } 
        private void ЗагрузитьКартинку(string имяФайла)
        {
            try
            {
                План = new BitmapImage(new Uri(имяФайла));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public BitmapImage План
        {
            get { return план; }
            set { план = value; OnPropertyChanged("План"); }
        }
        private BitmapImage план;
        public ObservableCollection<Entity> Objects { get; private set; }
        public void AddObject(Entity obj)
        {
            if (Objects.Contains(obj)) return;

            Objects.Add(obj);
        }
        public void RemoveObject(Entity obj)
        {
            if (obj == null) return;

            if (obj is Node)
            {
                var узел = obj as Node;
                var удаляемыеОбьекты = new List<Entity>();
                удаляемыеОбьекты.Add(узел);

                foreach (var участок in узел.IncomingSections)
                {
                    участок.First.RemoveSection(участок);
                    удаляемыеОбьекты.Add(участок as Entity);
                    if (участок.First is УзелПути && участок.First.IsUnrelated)
                        удаляемыеОбьекты.Add(участок.First);
                }
                foreach (var участок in узел.OutgoingSections)
                {
                    участок.Last.RemoveSection(участок);
                    удаляемыеОбьекты.Add(участок as Entity);
                    if (участок.Last is УзелПути && участок.Last.IsUnrelated)
                        удаляемыеОбьекты.Add(участок.Last);
                }

                foreach (var удаляемыйОбъект in удаляемыеОбьекты)
                    Objects.Remove(удаляемыйОбъект);

                return;
            }


            if (obj is УчастокПути)
            {
                var участок = obj as УчастокПути;

                участок.First.RemoveSection(участок);
                участок.Last.RemoveSection(участок);

                var удаляемыеОбьекты = new List<Entity>();
                удаляемыеОбьекты.Add(участок);

                if (участок.First is УзелПути && участок.First.IsUnrelated)
                    удаляемыеОбьекты.Add(участок.First);
                if (участок.Last is УзелПути && участок.Last.IsUnrelated)
                    удаляемыеОбьекты.Add(участок.Last);

                foreach (var удаляемыйОбъект in удаляемыеОбьекты)
                    Objects.Remove(удаляемыйОбъект);

                return;
            }
        }

        public void ДобавитьСпуск(УзелЛестницы узел)
        {
            var этажНиже = РодительскоеЗдание.ВыдатьЭтажНиже(this);
            if (этажНиже == null) throw new Exception(string.Format("Ошибка при попытке добавить этаж. Под \"{0}\" нет другого этажа", Title));

            var новыйУзел = new УзелЛестницы(этажНиже, узел.Position, узел.Title);
            этажНиже.AddObject(новыйУзел);
            var спуск = new Спуск(узел, новыйУзел, this);
        }
        public override BitmapImage Icon { get { return Настройки.Instance.ЭтажIco; } }
    }
}
