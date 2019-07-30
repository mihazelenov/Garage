using PGK_Center.BLL;
using PGK_Center.Commands;
using PGK_Center.DAL;
using PGK_Center.ObjectModel;
using PGK_Center.Views;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PGK_Center.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class GarageViewModel
    {
        private GarageWindow _view;

        public Garage Garage { get; set; }

        public ObservableCollection<Phone> Phones { get; set; }
        public ObservableCollection<Phone> CellPhones { get; set; }
        public ObservableCollection<Phone> StaticPhones { get; set; }

        public Phone CurrentMobile { get; set; }

        public Phone CurrentStatic { get; set; }

        public event Action<Garage> GarageSaved;

        private RelayCommand _addMobileCommand;
        public RelayCommand AddMobileCommand => _addMobileCommand ??
            (_addMobileCommand = new RelayCommand(o => AddPhone(new Phone { IsMobile = true })));

        private RelayCommand _deleteMobileCommand;
        public RelayCommand DeleteMobileCommand => _deleteMobileCommand ??
            (_deleteMobileCommand = new RelayCommand(o => DeletePhone(true)));

        private RelayCommand _addStaticCommand;
        public RelayCommand AddStaticCommand => _addStaticCommand ??
            (_addStaticCommand = new RelayCommand(o => AddPhone(new Phone { IsMobile = false })));

        private RelayCommand _deleteStaticCommand;
        public RelayCommand DeleteStaticCommand => _deleteStaticCommand ??
            (_deleteStaticCommand = new RelayCommand(o => DeletePhone(false)));

        private RelayCommand _okCommand;
        public RelayCommand OkCommand => _okCommand ??
            (_okCommand = new RelayCommand(o => Save()));

        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand => _cancelCommand ??
            (_cancelCommand = new RelayCommand(o => _view.DialogResult = false));

        public GarageViewModel(Garage garage)
        {
            Garage = garage;
            Phones = garage.Phones.ToObservableCollection();
            Phones.CollectionChanged += Phones_CollectionChanged;
            Phones_CollectionChanged(this, null);
        }

        private void Phones_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CellPhones = Phones.Where(a => a.IsMobile).ToObservableCollection();
            StaticPhones = Phones.Where(a => !a.IsMobile).ToObservableCollection();
        }

        public void ShowView(Window owner)
        {
            _view = new GarageWindow
            {
                DataContext = this,
                Owner = owner
            };
            _view.ShowDialog();
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(Garage.Number))
            {
                MessageBox.Show("Необходимо указать номер гаража");
                return;
            }

            if (DBManager.IsSameGarageExists(Garage))
            {
                MessageBox.Show("Гараж с таким номером уже существует");
                return;
            }

            DBManager.SaveGarage(Garage);

            foreach (var phone in Phones)
                phone.GarageID = Garage.Id;

            DBManager.SaveGaragePhones(Garage.Id, Phones);
            Garage.Phones = Phones.ToList();

            GarageSaved?.Invoke(Garage);
            _view.DialogResult = true;
        }

        private void AddPhone(Phone phone)
        {
            var view = new AddPhoneWindow
            {
                DataContext = phone,
                Owner = _view
            };
            if (view.ShowDialog() == true)
                Phones.Add(phone);
        }

        private void DeletePhone(bool isMobile)
        {
            void DeletePhone(Phone phone)
            {
                if (phone == null)
                    return;

                Phones.Remove(phone);
            }

            DeletePhone(isMobile
                ? CurrentMobile
                : CurrentStatic);
        }
    }
}
