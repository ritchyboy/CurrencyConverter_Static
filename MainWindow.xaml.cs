using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
        SqlConnection sqlConnection = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();

        private int CurrencyId = 0;
        private double FromAmount = 0;
        private double toAmount = 0;
        

        public MainWindow()
        {
            InitializeComponent();
            BindCurrency();
        }
        public void myConnection()
        {
            String connection = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connection);
            sqlConnection.Open();
        }
        private void BindCurrency()
        {
            myConnection();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("select Id,CurrencyName from Currency_Master", sqlConnection);
            cmd.CommandType = CommandType.Text;
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            DataRow newRow = dt.NewRow();
            newRow["Id"] = 0;
            newRow["CurrencyName"] = "--Select --";

            dt.Rows.InsertAt(newRow, 0);

            if (dt!=null &&dt.Rows.Count > 0)
            {
                cmbFromCurrency.ItemsSource = dt.DefaultView;
                cmbToCurrency.ItemsSource = dt.DefaultView;
            }
            sqlConnection.Close();

            
            cmbFromCurrency.DisplayMemberPath = "Text";
            cmbFromCurrency.SelectedValuePath = "Value";
            cmbFromCurrency.SelectedIndex = 0;


            
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

       //Function for the master tab (tbMaster)//

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtAmount.Text  == null || txtAmount.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter the amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtAmount.Focus();
                    return;
                }
                else if(txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter the currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrency.Focus();
                    return;
                }
                else
                {
                    if (CurrencyId > 0)
                    {
                        if (MessageBox.Show("Are you sure you want to update ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            myConnection();
                            DataTable dt = new DataTable();
                            cmd = new SqlCommand("UPDATE Currency_Master SET Amount = @Amount,CurrencyName = @CurrencyName WHERE Id = @Id",sqlConnection);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id", CurrencyId);
                            cmd.Parameters.AddWithValue("@Amount", txtAmount);
                            cmd.Parameters.AddWithValue("@CurrencyName",txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            sqlConnection.Close();

                            MessageBox.Show("Data updated successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        myConnection();
                        cmd = new SqlCommand("INSERT INTO Currency_Master(Amount,CurrencyName) VALUES(@Amount,@CurrencyName)", sqlConnection);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Amount", txtAmount);
                        cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                        cmd.ExecuteNonQuery();
                        sqlConnection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }
    }
}
