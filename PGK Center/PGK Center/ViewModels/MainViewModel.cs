using Microsoft.Win32;
using PGK_Center.BLL;
using PGK_Center.Commands;
using PGK_Center.DAL;
using PGK_Center.ObjectModel;
using PropertyChanged;
using System;
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

        private RelayCommand _statisticCommand;
        public RelayCommand StatisticCommand => _statisticCommand ??
            (_statisticCommand = new RelayCommand(o => Statistic()));

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
                FileName = $"Отчёт от {DateTime.Now.ToString("dd.MM.yyyy")}",
                Filter = ".csv files|*.csv"
            };
            if (dialog.ShowDialog() == true)
            {
                var csv = new StringBuilder("Номер\tФИО\tСчётчик\tМобильный\tСумма");
                csv.AppendLine();
                foreach (var garage in Garages.OrderBy(a => a.GarageNumber))
                    csv.AppendLine($"{garage.Number}\t{garage.Name}\t{garage.CounterStateToDisplay}\t{garage.CellPhone}\t{garage.Total}");

                File.WriteAllText(dialog.FileName, csv.ToString(), Encoding.UTF8);
            }
        }

        private void Statistic()
        {
            var dialog = new SaveFileDialog
            {
                FileName = $"Статистика от {DateTime.Now.ToString("dd.MM.yyyy")}",
                Filter = ".csv files|*.csv"
            };
            if (dialog.ShowDialog() == true)
            {
                var csv = new StringBuilder();
                csv.AppendLine($"Всего гаражей:\t{Garages.Count}");
                csv.AppendLine($"Общая площадь:\t{Garages.Sum(a => a.Square)}");
                csv.AppendLine();
                csv.AppendLine($"Счётчиков установлено:\t{Garages.Count(a => a.IsCounterSet)}");
                csv.AppendLine($"Счётчиков не установлено:\t{Garages.Count(a => a.IsCounterNotSet)}");
                csv.AppendLine($"Нет информации о счётчике:\t{Garages.Count(a => !a.IsCounterSet && !a.IsCounterNotSet)}");
                csv.AppendLine();
                csv.AppendLine($"Общая задолженность:\t{Garages.Where(a => a.Total > 0).Sum(a => a.Total)}");
                //csv.AppendLine($"Задолженность за текущий квартал:\t{Garages.Sum(a => a.TotalOnCurrentQuarter)}");
                csv.AppendLine($"Задолженность за текущий год:\t{Garages.Sum(a => a.TotalOnCurrentYear)}");
                csv.AppendLine($"Задолженность за предыдущие периоды:\t{Garages.Sum(a => a.TotalOnPreviousYears)}");

                File.WriteAllText(dialog.FileName, csv.ToString(), Encoding.UTF8);
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
