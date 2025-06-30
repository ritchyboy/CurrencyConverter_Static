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
            GetData();
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
            cmd = new SqlCommand("SELECT Id,CurrencyName FROM Currency_Master", sqlConnection);
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

            
            cmbFromCurrency.DisplayMemberPath = "CurrencyName";
            cmbFromCurrency.SelectedValuePath = "Id";
            cmbFromCurrency.SelectedIndex = 0;


            
            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Id";
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
                else if(txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
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
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName",txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            sqlConnection.Close();

                            MessageBox.Show("Data updated successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        if(MessageBox.Show("Are you sure you want to save?","Information",MessageBoxButton.YesNo
                            ,MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                          myConnection();
                          cmd = new SqlCommand("INSERT INTO Currency_Master(Amount,CurrencyName) VALUES(@Amount,@CurrencyName)", sqlConnection);
                          cmd.CommandType = CommandType.Text;
                          cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                          cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                          cmd.ExecuteNonQuery();
                          sqlConnection.Close();

                          MessageBox.Show("Data saved successfully","Information",MessageBoxButton.OK,MessageBoxImage.Information);
                          ClearMaster();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ClearMaster()
        {
            txtAmount.Text = string.Empty;
            txtCurrency.Text = string.Empty;
            btnSave.Content = "Save";
            GetData();
            CurrencyId = 0;
            BindCurrency();
            txtAmount.Focus();
        }
        private void GetData()
        {
            myConnection();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("Select * FROM Currency_Master",sqlConnection);
            cmd.CommandType = CommandType.Text;
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if (dt != null && dt.Rows.Count > 0)
            {
                dgvCurrency.ItemsSource = dt.DefaultView;
            }
            else
                dgvCurrency.ItemsSource = null;

            sqlConnection.Close();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearMaster();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                //Create object for DataGrid
                DataGrid grd = (DataGrid)sender;
                //Create object for DataRowView
                DataRowView row_selected = grd.CurrentItem as DataRowView;

                //row_selected is not null
                if (row_selected != null)
                {

                    //dgvCurrency items count greater than zero
                    if (dgvCurrency.Items.Count > 0)
                    {
                        if (grd.SelectedCells.Count > 0)
                        {

                            //Get selected row Id column value and Set in CurrencyId variable
                            CurrencyId = Int32.Parse(row_selected["Id"].ToString());

                            //DisplayIndex is equal to zero than it is Edit cell
                            if (grd.SelectedCells[0].Column.DisplayIndex == 0)
                            {

                                //Get selected row Amount column value and Set in Amount textbox
                                txtAmount.Text = row_selected["Amount"].ToString();

                                //Get selected row CurrencyName column value and Set in CurrencyName textbox
                                txtCurrencyName.Text = row_selected["CurrencyName"].ToString();

                                //Change save button text Save to Update
                                btnSave.Content = "Update";
                            }

                            //DisplayIndex is equal to one than it is Delete cell                    
                            if (grd.SelectedCells[0].Column.DisplayIndex == 1)
                            {
                                //Show confirmation dialogue box
                                if (MessageBox.Show("Are you sure you want to delete ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    myConnection();
                                    DataTable dt = new DataTable();

                                    //Execute delete query for delete record from table using Id
                                    cmd = new SqlCommand("DELETE FROM Currency_Master WHERE Id = @Id",sqlConnection);
                                    cmd.CommandType = CommandType.Text;

                                    //CurrencyId set in @Id parameter and send it in delete statement
                                    cmd.Parameters.AddWithValue("@Id", CurrencyId);
                                    cmd.ExecuteNonQuery();
                                    sqlConnection.Close();

                                    MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    ClearMaster();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    }

