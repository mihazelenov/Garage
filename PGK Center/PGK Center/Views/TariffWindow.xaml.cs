using PGK_Center.BLL;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PGK_Center.Views
{
    /// <summary>
    /// Interaction logic for GarageWindow.xaml
    /// </summary>
    public partial class TariffWindow : Window
    {
        public TariffWindow()
        {
            InitializeComponent();
        }

        private void DecimalTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out int result);
        }
    }
}
