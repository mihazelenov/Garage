using PGK_Center.BLL;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PGK_Center.Views
{
    /// <summary>
    /// Interaction logic for GarageWindow.xaml
    /// </summary>
    public partial class AddPayWindow : Window
    {
        public AddPayWindow()
        {
            InitializeComponent();
        }

        private void DecimalTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(CommonData.DecimalRegex.IsMatch(e.Text) &&
                !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)));
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
