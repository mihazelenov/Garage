using PGK_Center.BLL;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PGK_Center.Views
{
    /// <summary>
    /// Interaction logic for GarageWindow.xaml
    /// </summary>
    public partial class GarageWindow : Window
    {
        public GarageWindow()
        {
            InitializeComponent();
        }

        private void DecimalTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(CommonData.DecimalRegex.IsMatch(e.Text) &&
                !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)));
        }
    }
}
