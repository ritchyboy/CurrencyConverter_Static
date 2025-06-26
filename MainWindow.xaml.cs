using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CurrencyConverter_Static
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BindCurrency();
        }
        private void BindCurrency()
        {
            DataTable dtCurrency = new DataTable();
            dtCurrency.Columns.Add("Text");
            dtCurrency.Columns.Add("Value");


            dtCurrency.Rows.Add("--Select--", 0);
            dtCurrency.Rows.Add("INR", 1);
            dtCurrency.Rows.Add("USD", 75);
            dtCurrency.Rows.Add("EUR", 85);
            dtCurrency.Rows.Add("SAR", 20);
            dtCurrency.Rows.Add("POUND", 5);
            dtCurrency.Rows.Add("DEM", 43);

            cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbFromCurrency.DisplayMemberPath = "Text";
            cmbFromCurrency.SelectedValuePath = "Value";
            cmbFromCurrency.SelectedIndex = 0;


            cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Value";
            cmbToCurrency.SelectedIndex = 0;

        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Converter_Click(object sender, RoutedEventArgs e)
        {
           double convertedValue;

           if(txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
           {
                MessageBox.Show("Please enter a currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCurrency.Focus();
                return;
           }
           else if(cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
           {
                MessageBox.Show("Please select currency from", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCurrency.Focus();
                return;
           }
           else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
           {
                MessageBox.Show("Please select currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCurrency.Focus();
                return;
           }
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                convertedValue = double.Parse(cmbToCurrency.Text);
                lblCurrency.Content = cmbToCurrency.Text + " " + convertedValue.ToString("N3");
            }
            else
            {
                
                convertedValue = (double.Parse(cmbFromCurrency.SelectedValue.ToString()) *
                double.Parse(txtCurrency.Text)) / 
                double.Parse(cmbToCurrency.SelectedValue.ToString());

                lblCurrency.Content = cmbToCurrency.Text + " " + convertedValue.ToString("N3");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            lblCurrency.Content = string.Empty;
            txtCurrency.Text = string.Empty;
        }

    }
}
