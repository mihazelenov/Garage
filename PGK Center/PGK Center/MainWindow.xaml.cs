using PGK_Center.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace PGK_Center
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel
        {
            get => DataContext as MainViewModel;
            set => DataContext = value;
        }

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel(this);
        }

        private void Clients_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (e.Column.Header.ToString() == "№")
            {
                switch (_viewModel.NumberSortDirection)
                {
                    case ListSortDirection.Descending:
                        _viewModel.NumberSortDirection = ListSortDirection.Ascending;
                        break;

                    default:
                        _viewModel.NumberSortDirection = ListSortDirection.Descending;
                        break;
                }

                _viewModel.ShowGarages();
            }
        }
    }
}
