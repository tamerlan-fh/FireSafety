using FireSafety.Models;
using FireSafety.VisualModels;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FireSafety
{
    class MainWindowViewModel : BasePropertyChanged
    {
        #region Команды
        public ICommand ExitCommand { get; protected set; }
        public ICommand СменитьПланЭтажаCommand { get; protected set; }
        public ICommand ДобавитьЗданиеCommand { get; protected set; }
        public ICommand ДобавитьЭтажCommand { get; protected set; }
        public ICommand УдалитьОбъектCommand { get; protected set; }
        public ICommand ЗагрузитьЗданиеCommand { get; protected set; }
        public ICommand РассчетБлокированияПутейЭвакуацииCommand { get; protected set; }
        public ICommand РассчетПожарногоРискаCommand { get; protected set; }
        public ICommand СформироватьОтчетCommand { get; protected set; }
        public ICommand СохранитьCommand { get; protected set; }

        #endregion

        #region Режим действия
        public РежимДействия Режим
        {
            get { return режим; }
            set
            {
                var староеЗначение = Режим;
                режим = value;
                switch (староеЗначение)
                {
                    case РежимДействия.ДобавитьВыход: OnPropertyChanged("ДобавитьВыход"); break;
                    case РежимДействия.ДобавитьГруппуМобильности: OnPropertyChanged("ДобавитьГруппуМобильности"); break;
                    case РежимДействия.ДобавитьДвернойПроем: OnPropertyChanged("ДобавитьДвернойПроем"); break;
                    case РежимДействия.ДобавитьЛестницу: OnPropertyChanged("ДобавитьЛестницу"); break;
                    case РежимДействия.Перемещение: OnPropertyChanged("Перемещение"); break;
                    case РежимДействия.ПостроениеПутиЭвакуации: OnPropertyChanged("ПостроениеПутиЭвакуации"); break;
                    case РежимДействия.Удаление: OnPropertyChanged("Удаление"); break;
                    case РежимДействия.ЗадатьМасштаб: OnPropertyChanged("ЗадатьМасштаб"); break;
                    case РежимДействия.ДобавитьОчагВозгорания: OnPropertyChanged("ДобавитьОчагВозгорания"); break;
                }
                режим = value; OnPropertyChanged("Режим");
            }
        }
        private РежимДействия режим;
        public bool ДобавитьДвернойПроем
        {
            get { return Режим == РежимДействия.ДобавитьДвернойПроем; }
            set { if (value) { Режим = РежимДействия.ДобавитьДвернойПроем; OnPropertyChanged("ДобавитьДвернойПроем"); } }
        }
        public bool Перемещение
        {
            get { return Режим == РежимДействия.Перемещение; }
            set { if (value) Режим = РежимДействия.Перемещение; OnPropertyChanged("Перемещение"); }
        }
        public bool ДобавитьВыход
        {
            get { return Режим == РежимДействия.ДобавитьВыход; }
            set { if (value) { Режим = РежимДействия.ДобавитьВыход; OnPropertyChanged("ДобавитьВыход"); } }
        }
        public bool ДобавитьГруппуМобильности
        {
            get { return Режим == РежимДействия.ДобавитьГруппуМобильности; }
            set { if (value) { Режим = РежимДействия.ДобавитьГруппуМобильности; OnPropertyChanged("ДобавитьГруппуМобильности"); } }
        }
        public bool ПостроениеПутиЭвакуации
        {
            get { return Режим == РежимДействия.ПостроениеПутиЭвакуации; }
            set { if (value) { Режим = РежимДействия.ПостроениеПутиЭвакуации; OnPropertyChanged("ПостроениеПутиЭвакуации"); } }
        }
        public bool Удаление
        {
            get { return Режим == РежимДействия.Удаление; }
            set { if (value) { Режим = РежимДействия.Удаление; OnPropertyChanged("Удаление"); } }
        }
        public bool ДобавитьЛестницу
        {
            get { return Режим == РежимДействия.ДобавитьЛестницу; }
            set { if (value) { Режим = РежимДействия.ДобавитьЛестницу; OnPropertyChanged("ДобавитьЛестницу"); } }
        }
        public bool ДобавитьОчагВозгорания
        {
            get { return Режим == РежимДействия.ДобавитьОчагВозгорания; }
            set { if (value) { Режим = РежимДействия.ДобавитьОчагВозгорания; OnPropertyChanged("ДобавитьОчагВозгорания"); } }
        }
        public bool ЗадатьМасштаб
        {
            get { return Режим == РежимДействия.ЗадатьМасштаб; }
            set { if (value) { Режим = РежимДействия.ЗадатьМасштаб; OnPropertyChanged("ЗадатьМасштаб"); } }
        }

        #endregion
        public ObservableCollection<Building> Здания { get; private set; }
        public Entity ВыделенныйОбъект
        {
            get { return выделенныйОбъект; }
            set
            {
                выделенныйОбъект = value;
                if (value == null) return;

                ВыделенныйОбъект.IsSelected = true;
                OnPropertyChanged("ВыделенныйОбъект");
                if (ВыделенныйОбъект is Floor)
                    ТекущийЭтаж = value as Floor;
                else if (ВыделенныйОбъект is Building)
                    ТекущееЗдание = value as Building;
                else if (ВыделенныйОбъект is Путь || ВыделенныйОбъект is КоллекцияПутей)
                    return;
                else if (ВыделенныйОбъект != null)
                    ТекущийЭтаж = ВыделенныйОбъект.Parent as Floor;
            }
        }
        private Entity выделенныйОбъект;
        public Floor ТекущийЭтаж
        {
            get { return текущийЭтаж; }
            set
            {
                текущийЭтаж = value;
                OnPropertyChanged("ТекущийЭтаж");
                if (ТекущийЭтаж != null)
                    ТекущееЗдание = ТекущийЭтаж.РодительскоеЗдание;
            }
        }
        private Floor текущийЭтаж;
        public Building ТекущееЗдание
        {
            get { return текущееЗдание; }
            set
            {
                if (текущееЗдание == value) return;
                текущееЗдание = value;
                OnPropertyChanged("ТекущееЗдание");
                if (ТекущийЭтаж == null && ТекущееЗдание != null) ТекущийЭтаж = ТекущееЗдание.Floors.FirstOrDefault();
            }
        }
        private Building текущееЗдание;
        public MainWindowViewModel()
        {
            ExitCommand = new RelayCommand(param => App.Current.Shutdown());
            СменитьПланЭтажаCommand = new RelayCommand(param => this.СменитьПланЭтажа(), param => ТекущийЭтаж != null);
            ДобавитьЗданиеCommand = new RelayCommand(param => this.ДобавитьЗдание());
            ДобавитьЭтажCommand = new RelayCommand(param => this.ДобавитьЭтаж(), param => ТекущееЗдание != null);
            УдалитьОбъектCommand = new RelayCommand(param => this.УдалитьОбъект(), param => МожноУдалитьОбъект());
            ЗагрузитьЗданиеCommand = new RelayCommand(param => ЗагрузитьЗдание());
            РассчетБлокированияПутейЭвакуацииCommand = new RelayCommand(param => ТекущееЗдание.РассчетБлокированияПутейЭвакуации(), param => ТекущееЗдание != null);
            РассчетПожарногоРискаCommand = new RelayCommand(param => ТекущееЗдание.РассчетПожарногоРиска(), param => ТекущееЗдание != null);
            СформироватьОтчетCommand = new RelayCommand(param => ТекущееЗдание.СформироватьОтчет(), param => ТекущееЗдание != null);
            СохранитьCommand = new RelayCommand(param => ТекущееЗдание.Сохранить(), param => ТекущееЗдание != null);

            Здания = new ObservableCollection<Building>();
            var здание = new Building();
            //здание.ДобавитьЭтаж(new Этаж(здание) { План = Настройки.GetBitmapImage(Resources.FileName1) });
            //здание.ДобавитьЭтаж(new Этаж(здание) { План = Настройки.GetBitmapImage(Resources.FileName1) });
            //здание.ДобавитьЭтаж(new Этаж(здание) { План = Настройки.GetBitmapImage(Resources.FileName1) });

            Режим = РежимДействия.Перемещение;
            Здания.Add(здание);

            ВыделенныйОбъект = Здания.FirstOrDefault();
        }
        private void СменитьПланЭтажа()
        {
            if (ТекущийЭтаж == null) return;
            ТекущийЭтаж.ЗагрузитьПланЭтажаCommand.Execute(null);
        }
        private void ДобавитьЗдание()
        {
            Здания.Add(new Building());
        }
        private void ДобавитьЭтаж()
        {
            if (ТекущееЗдание == null) return;
            var этаж = new Floor(ТекущееЗдание);
            ТекущееЗдание.AddFloor(этаж);
            ТекущееЗдание.CurrentFloor = этаж;
            if (ТекущийЭтаж == null) ТекущийЭтаж = этаж;
        }
        private void УдалитьОбъект()
        {
            Режим = РежимДействия.Удаление;
            if (ВыделенныйОбъект == null) return;

            if (ВыделенныйОбъект is Building)
            {
                Здания.Remove(ВыделенныйОбъект as Building);

                ТекущееЗдание = Здания.FirstOrDefault();
                ВыделенныйОбъект = ТекущееЗдание;
                if (ТекущееЗдание != null)
                    ТекущийЭтаж = ТекущееЗдание.CurrentFloor;
                else
                    ТекущийЭтаж = null;
                return;
            }

            if (ВыделенныйОбъект is Floor)
            {
                ТекущееЗдание.RemoveFloor(ВыделенныйОбъект as Floor);
                ТекущийЭтаж = ТекущееЗдание.CurrentFloor;
                ВыделенныйОбъект = ТекущийЭтаж;
                return;
            }

            ТекущийЭтаж.RemoveObject(ВыделенныйОбъект);
            ВыделенныйОбъект = ТекущийЭтаж.Objects.FirstOrDefault();
            if (ВыделенныйОбъект == null) ВыделенныйОбъект = ТекущийЭтаж;



            //if (ВыделенныйОбъект is Здание)
            //{
            //    Здания.Remove(ВыделенныйОбъект as Здание);

            //    ТекущееЗдание = Здания.FirstOrDefault();
            //    ВыделенныйОбъект = ТекущееЗдание;
            //    if (ТекущееЗдание != null)
            //        ТекущийЭтаж = ТекущееЗдание.ТекущийЭтаж;
            //    else
            //        ТекущийЭтаж = null;
            //    return;
            //}

            //if (ВыделенныйОбъект is Этаж)
            //{
            //    ТекущееЗдание.УдалитьЭтаж(ВыделенныйОбъект as Этаж);
            //    ТекущийЭтаж = ТекущееЗдание.Этажи.FirstOrDefault();
            //    if (ТекущийЭтаж == null)
            //        ВыделенныйОбъект = ТекущееЗдание;
            //    else
            //        ВыделенныйОбъект = ТекущийЭтаж;
            //    return;
            //}

            //if (ТекущийЭтаж == null) return;

            //ТекущийЭтаж.УдалитьОбъект(ВыделенныйОбъект);
            //ВыделенныйОбъект = ТекущийЭтаж.Объекты.FirstOrDefault();
            //if (ВыделенныйОбъект == null) ВыделенныйОбъект = ТекущийЭтаж;
        }
        private bool МожноУдалитьОбъект()
        {
            return !(ВыделенныйОбъект is Путь || ВыделенныйОбъект is КоллекцияПутей) && ВыделенныйОбъект != null;
        }
        private void ЗагрузитьЗдание()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != true) return;

        }

        #region Иконки
        public BitmapImage ВыходIco { get { return Настройки.Instance.ВыходIco; } }
        public BitmapImage ДверьIco { get { return Настройки.Instance.ДверьIco; } }
        public BitmapImage ПутьIco { get { return Настройки.Instance.ПутьIco; } }
        public BitmapImage СтартIco { get { return Настройки.Instance.СтартIco; } }
        public BitmapImage УказательIco { get { return Настройки.Instance.УказательIco; } }
        public BitmapImage УдалитьIco { get { return Настройки.Instance.УдалитьIco; } }
        public BitmapImage ЛестницаIco { get { return Настройки.Instance.ЛестницаIco; } }
        //public BitmapImage ОгоньIco { get { return Настройки.Instance.ОгоньIco; } }
        //public BitmapImage ЛинейкаIco { get { return Настройки.Instance.ЛинейкаIco; } }
        //public BitmapImage СлоиIco { get { return Настройки.Instance.СлоиIco; } }
        //public BitmapImage ЗданиеIco { get { return Настройки.Instance.ЗданиеIco; } }
        //public BitmapImage ЭтажIco { get { return Настройки.Instance.ЭтажIco; } }
        //public BitmapImage СохранитьIco { get { return Настройки.Instance.СохранитьIco; } }
        //public BitmapImage ДокументIco { get { return Настройки.Instance.ДокументIco; } }
        //public BitmapImage РискIco { get { return Настройки.Instance.РискIco; } }
        //public BitmapImage БлокировкаIco { get { return Настройки.Instance.БлокировкаIco; } }
        #endregion
    }
}
