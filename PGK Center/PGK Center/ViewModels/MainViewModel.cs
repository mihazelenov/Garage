using Microsoft.Win32;
using PGK_Center.BLL;
using PGK_Center.Commands;
using PGK_Center.DAL;
using PGK_Center.ObjectModel;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace PGK_Center.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        private readonly MainWindow _view;

        public ObservableCollection<Garage> Garages { get; set; }
        public ObservableCollection<Garage> GaragesToDisplay { get; set; }
        public ObservableCollection<Tariff> Tariffs { get; set; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                RefreshGaragesToDisplay();
            }
        }

        public Garage CurrentGarage { get; set; }
        public Tariff CurrentTariff { get; set; }

        public ListSortDirection NumberSortDirection { get; set; }

        private RelayCommand _addCommand;
        public RelayCommand AddCommand => _addCommand ??
            (_addCommand = new RelayCommand(o => AddGarage()));

        private RelayCommand _editCommand;
        public RelayCommand EditCommand => _editCommand ??
            (_editCommand = new RelayCommand(o => EditGarage()));

        private RelayCommand _deleteCommand;
        public RelayCommand DeleteCommand => _deleteCommand ??
            (_deleteCommand = new RelayCommand(o => DeleteGarage()));

        private RelayCommand _payCommand;
        public RelayCommand PayCommand => _payCommand ??
            (_payCommand = new RelayCommand(o => Pays()));

        private RelayCommand _reportCommand;
        public RelayCommand ReportCommand => _reportCommand ??
            (_reportCommand = new RelayCommand(o => Report()));

        private RelayCommand _addTariffCommand;
        public RelayCommand AddTariffCommand => _addTariffCommand ??
            (_addTariffCommand = new RelayCommand(o => AddTariff()));

        private RelayCommand _editTariffCommand;
        public RelayCommand EditTariffCommand => _editTariffCommand ??
            (_editTariffCommand = new RelayCommand(o => EditTariff()));

        private RelayCommand _deleteTariffCommand;
        public RelayCommand DeleteTariffCommand => _deleteTariffCommand ??
            (_deleteTariffCommand = new RelayCommand(o => DeleteTariff()));

        public MainViewModel(MainWindow view)
        {
            _view = view;

            Garages = DBManager.GetAllGarages()
                .OrderBy(a => a.GarageNumber)
                .ToObservableCollection();
            GaragesToDisplay = Garages.ToObservableCollection();
            Garages.CollectionChanged += Garages_CollectionChanged;
            ShowTariffs();
        }

        private void Garages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshGaragesToDisplay();
        }

        private void RefreshGaragesToDisplay()
        {
            GaragesToDisplay = (string.IsNullOrEmpty(_searchText)
                ? Garages
                : Garages.Where(a => a.Address.IsTextContains(_searchText) ||
                                     a.Comment.IsTextContains(_searchText) ||
                                     a.Name.IsTextContains(_searchText) ||
                                     a.Number.IsTextContains(_searchText) ||
                                     a.Phones.Any(p => p.Value.IsTextContains(_searchText))))
                .ToObservableCollection();
        }

        #region Garage
        public void ShowGarages()
        {
            var garages = NumberSortDirection == ListSortDirection.Descending
                ? Garages.OrderByDescending(a => a.GarageNumber)
                : Garages.OrderBy(a => a.GarageNumber);

            Garages = garages.ToObservableCollection();
            Garages.CollectionChanged += Garages_CollectionChanged;
            Garages_CollectionChanged(this, null);
        }

        private void AddGarage()
        {
            var viewModel = new GarageViewModel(new Garage());
            viewModel.GarageSaved += ViewModel_GarageSaved;
            viewModel.ShowView(_view);
        }

        private void ViewModel_GarageSaved(Garage obj)
        {
            var garage = Garages.FirstOrDefault(a => a.ID == obj.ID);

            if (garage != default)
                Garages.Remove(garage);

            Garages.Add(obj);
            ShowGarages();
        }

        private void EditGarage()
        {
            if (CurrentGarage == null)
                return;

            var viewModel = new GarageViewModel((Garage)CurrentGarage.Clone());
            viewModel.GarageSaved += ViewModel_GarageSaved;
            viewModel.ShowView(_view);
        }

        private void DeleteGarage()
        {
            if (CurrentGarage == null)
                return;

            if (MessageBox.Show($"Вы действительно хотите удалить гараж №{CurrentGarage.Number}?",
                string.Empty, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            DBManager.DeleteGarage(CurrentGarage.ID);
            Garages.Remove(CurrentGarage);
            ShowGarages();
        }

        private void Pays()
        {
            if (CurrentGarage == null)
                return;

            var viewModel = new PaysViewModel(CurrentGarage);
            viewModel.GarageSaved += ViewModel_GarageSaved;
            viewModel.ShowView(_view);
        }

        private void Report()
        {
            var dialog = new SaveFileDialog
            {
                Filter = ".csv files|*.csv"
            };
            if (dialog.ShowDialog() == true)
            {
                var csv = new StringBuilder();
                foreach (var garage in Garages.OrderBy(a => a.GarageNumber))
                    csv.AppendLine($"{garage.Number}\t{garage.Name}\t{garage.CellPhone}\t{garage.Total}");

                File.WriteAllText(dialog.SafeFileName, csv.ToString(), Encoding.UTF8);
            }
        }
        #endregion

        #region Tariff
        private void AddTariff()
        {
            var viewModel = new TariffViewModel(new Tariff());
            viewModel.TariffSaved += ViewModel_TariffSaved;
            viewModel.ShowView(_view);
        }

        private void ViewModel_TariffSaved(Tariff obj)
        {
            var tariff = CommonData.Tariffs.FirstOrDefault(a => a.ID == obj.ID);

            if (tariff != default)
                CommonData.Tariffs.Remove(tariff);

            CommonData.Tariffs.Add(obj);
            
            ShowTariffs();
            ShowGarages();
        }

        public void ShowTariffs()
        {
            Tariffs = CommonData.Tariffs
                .OrderByDescending(a => a.Year)
                .ToObservableCollection();
        }

        private void EditTariff()
        {
            if (CurrentTariff == null)
                return;

            var viewModel = new TariffViewModel((Tariff)CurrentTariff.Clone());
            viewModel.TariffSaved += ViewModel_TariffSaved;
            viewModel.ShowView(_view);
        }

        private void DeleteTariff()
        {
            if (CurrentTariff == null)
                return;

            if (MessageBox.Show($"Вы действительно хотите удалить тариф {CurrentTariff.Year} года?",
                string.Empty, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            DBManager.DeleteTariff(CurrentTariff.ID);
            CommonData.Tariffs.Remove(CurrentTariff);

            ShowTariffs();
            ShowGarages();
        }
        #endregion
    }
}
