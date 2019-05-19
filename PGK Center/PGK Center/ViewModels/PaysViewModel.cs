using PGK_Center.BLL;
using PGK_Center.Commands;
using PGK_Center.DAL;
using PGK_Center.ObjectModel;
using PGK_Center.Views;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace PGK_Center.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class PaysViewModel
    {
        private PaysWindow _view;

        private Garage _garage { get; }

        private ObservableCollection<Pay> Pays { get; }
        public ObservableCollection<Pay> OrderedPays { get; set; }

        public string Title { get; }

        public event Action<Garage> GarageSaved;

        private RelayCommand _addCommand;
        public RelayCommand AddCommand => _addCommand ??
            (_addCommand = new RelayCommand(o => Add()));

        private RelayCommand _deleteCommand;
        public RelayCommand DeleteCommand => _deleteCommand ??
            (_deleteCommand = new RelayCommand(o => Delete()));

        private RelayCommand _okCommand;
        public RelayCommand OkCommand => _okCommand ??
            (_okCommand = new RelayCommand(o => Save()));

        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand => _cancelCommand ??
            (_cancelCommand = new RelayCommand(o => _view.DialogResult = false));

        public Pay CurrentPay { get; set; }

        public PaysViewModel(Garage garage)
        {
            _garage = garage;
            Title = $"Гараж №{garage.Number}. История оплаты";

            Pays = garage.Pays.ToObservableCollection();
            Pays.CollectionChanged += Pays_CollectionChanged;
            Pays_CollectionChanged(this, null);
        }

        private void Pays_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OrderedPays = Pays.OrderBy(a => a.Date).ToObservableCollection();
        }

        public void ShowView(Window owner)
        {
            _view = new PaysWindow
            {
                DataContext = this,
                Owner = owner
            };
            _view.ShowDialog();
        }

        private void Add()
        {
            var pay = new Pay();
            var view = new AddPayWindow
            {
                DataContext = pay,
                Owner = _view
            };
            if (view.ShowDialog() == true)
                Pays.Add(pay);
        }

        private void Delete()
        {
            if (CurrentPay == null)
                return;

            Pays.Remove(CurrentPay);
        }

        private void Save()
        {
            foreach (var pay in Pays)
                pay.GarageID = _garage.ID;

            DBManager.SaveGaragePays(_garage.ID, Pays);
            _garage.Pays = Pays.ToList();

            GarageSaved?.Invoke(_garage);
            _view.DialogResult = true;
        }
    }
}
