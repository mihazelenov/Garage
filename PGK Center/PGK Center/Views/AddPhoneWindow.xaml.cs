using PGK_Center.ObjectModel;
using System.Windows;

namespace PGK_Center.Views
{
    /// <summary>
    /// Interaction logic for GarageWindow.xaml
    /// </summary>
    public partial class AddPhoneWindow : Window
    {
        public AddPhoneWindow()
        {
            InitializeComponent();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {

            if (!(DataContext is Phone phone) ||
                string.IsNullOrWhiteSpace(phone.Value))
                return;

            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
