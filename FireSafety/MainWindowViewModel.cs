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
        public ICommand AddBuildingCommand { get; protected set; }
        public ICommand AddFloorCommand { get; protected set; }
        public ICommand RemoveSelectedEntityCommand { get; protected set; }
        public ICommand LoadBuildingCommand { get; protected set; }
        public ICommand SaveBuldingCommand { get; protected set; }
        public ICommand CalculateBlockageEvacuationRoutesCommand { get; protected set; }
        public ICommand CalculateFireRiskCommand { get; protected set; }
        public ICommand ComposeReportCommand { get; protected set; }

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
                selectedEntity = value;
                if (value == null) return;

                SelectedEntity.IsSelected = true;
                OnPropertyChanged("SelectedEntity");
                if (SelectedEntity is Floor)
                    CurrentFloor = value as Floor;
                else if (SelectedEntity is Building)
                    CurrentBuilding = value as Building;
                else if (SelectedEntity is Route)
                    return;
                else if (SelectedEntity != null)
                    CurrentFloor = SelectedEntity.Parent as Floor;
            }
        }
        private Entity selectedEntity;
        public Floor CurrentFloor
        {
            get { return currentFloor; }
            set
            {
                currentFloor = value;
                OnPropertyChanged("CurrentFloor");
                if (CurrentFloor != null)
                    CurrentBuilding = CurrentFloor.ParentBuilding;
            }
        }
        private Floor currentFloor;
        public Building CurrentBuilding
        {
            get { return currentBuilding; }
            set
            {
                if (currentBuilding == value) return;
                currentBuilding = value;
                OnPropertyChanged("CurrentBuilding");
                if (CurrentFloor == null && CurrentBuilding != null) CurrentFloor = CurrentBuilding.Floors.FirstOrDefault();
            }
        }
        private Building currentBuilding;
        public MainWindowViewModel()
        {
            ExitCommand = new RelayCommand(param => App.Current.Shutdown());
            AddBuildingCommand = new RelayCommand(param => this.AddBuilding());
            AddFloorCommand = new RelayCommand(param => this.AddFloor(), param => CurrentBuilding != null);
            RemoveSelectedEntityCommand = new RelayCommand(param => this.RemoveSelectedEntity(), param => CanRemoveSelectedEntity());
            LoadBuildingCommand = new RelayCommand(param => LoadBuilding());
            CalculateBlockageEvacuationRoutesCommand = new RelayCommand(param => CurrentBuilding.CalculateBlockageEvacuationRoutes(), param => CurrentBuilding != null);
            CalculateFireRiskCommand = new RelayCommand(param => CurrentBuilding.CalculateFireRisk(), param => CurrentBuilding != null);
            ComposeReportCommand = new RelayCommand(param => CurrentBuilding.ComposeReport(), param => CurrentBuilding != null);
            SaveBuldingCommand = new RelayCommand(param => CurrentBuilding.Save(), param => CurrentBuilding != null);

            Buildings = new ObservableCollection<Building>();
            var bilding = new Building();

            Mode = ActionMode.Move;
            Buildings.Add(bilding);

            SelectedEntity = Buildings.FirstOrDefault();
        }

        private void AddBuilding()
        {
            Buildings.Add(new Building());
        }
        private void AddFloor()
        {
            if (CurrentBuilding == null) return;
            var этаж = new Floor(CurrentBuilding);
            CurrentBuilding.AddFloor(этаж);
            CurrentBuilding.CurrentFloor = этаж;
            if (CurrentFloor == null) CurrentFloor = этаж;
        }
        private void RemoveSelectedEntity()
        {
            Mode = ActionMode.Remove;
            if (SelectedEntity == null) return;

            if (SelectedEntity is Building)
            {
                Buildings.Remove(SelectedEntity as Building);

                CurrentBuilding = Buildings.FirstOrDefault();
                SelectedEntity = CurrentBuilding;
                if (CurrentBuilding != null)
                    CurrentFloor = CurrentBuilding.CurrentFloor;
                else
                    CurrentFloor = null;
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
        private bool CanRemoveSelectedEntity()
        {
            return !(SelectedEntity is Route) && SelectedEntity != null;
        }
        private void LoadBuilding()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != true) return;

        }
        private void SaveBulding()
        {

        }
    }
}
