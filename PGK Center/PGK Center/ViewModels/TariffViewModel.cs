using PGK_Center.BLL;
using PGK_Center.Commands;
using PGK_Center.DAL;
using PGK_Center.ObjectModel;
using PGK_Center.Views;
using PropertyChanged;
using System;
using System.Linq;
using System.Windows;

namespace PGK_Center.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class TariffViewModel
    {
        private TariffWindow _view;

        public Tariff Tariff { get; set; }

        public event Action<Tariff> TariffSaved;

        private RelayCommand _okCommand;
        public RelayCommand OkCommand => _okCommand ??
            (_okCommand = new RelayCommand(o => Save()));

        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand => _cancelCommand ??
            (_cancelCommand = new RelayCommand(o => _view.DialogResult = false));

        public TariffViewModel(Tariff tariff)
        {
            Tariff = tariff;
        }

        public void ShowView(Window owner)
        {
            _view = new TariffWindow
            {
                DataContext = this,
                Owner = owner
            };
            _view.ShowDialog();
        }

        private void Save()
        {
            if (Tariff.Year < 0 || Tariff.Value < 0)
            {
                MessageBox.Show("Необходимо указать корректные данные тарифа");
                return;
            }

            if (CommonData.Tariffs.Any(a => a.Year == Tariff.Year && a.ID != Tariff.ID))
            {
                MessageBox.Show($"Тариф {Tariff.Year} года уже существует");
                return;
            }

            DBManager.SaveTariff(Tariff);

            TariffSaved?.Invoke(Tariff);
            _view.DialogResult = true;
        }
    }
}
