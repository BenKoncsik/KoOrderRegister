using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
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
using System.IO;
using System.Globalization;

namespace ShoeDatabase
{
    /// <summary>
    /// Interaction logic for NewEntryWindow.xaml
    /// </summary>
    public partial class NewEntryWindow : Window
    {
        private SQLiteConnection connection;
        private MainWindow window;
        private string photoFilePath = "NULL";
        private List<Custumer> customers = new List<Custumer>();
        private bool newOrder = true;
        private DateTime OrderDateTime { get; set; } = DateTime.Now;
        private CustomerShoeInfo customer;

        public NewEntryWindow()
        {
            InitializeComponent();
            customers.Add(new Custumer("Új ember"));
        }

        public NewEntryWindow(CustomerShoeInfo customer, SQLiteConnection connection, MainWindow main)
        {
            InitializeComponent();
            this.connection = connection;
            window = main;
            customers.Add(new Custumer("Új ember"));
            SettDataBse(connection, main);
            newOrder = false;
            this.customer = customer;
            nameBox.Text = customer.Name;
            addressBox.Text = customer.Address;
            tajNumberBox.Text = customer.TajNumber;
            orderNumberBox.Text = customer.OrderNumber;
            DateTime dateTime = DateTime.Now;
            if (!customer.OrderDate.Equals("NULL"))
            {
                if (DateTime.TryParseExact(customer.OrderDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    orderDateBox.SelectedDate = dateTime;
                }
                else
                {
                    MessageBox.Show("Hiba a megrendelési dátum megjelenítésénél!");
                }
            }
            
            if (!customer.OrderReleaseDate.Equals("NULL") && DateTime.TryParseExact(customer.OrderDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                orderReleaseDateBox.SelectedDate = dateTime;
            }
            if(!customer.PhotoPath.Equals("null") && !customer.PhotoPath.Equals("NULL"))
            {
                photoFilePath = customer.PhotoPath;
                photoButton.Content = photoFilePath;
            }
            
            foreach (Custumer c in customers) 
            {
                if (c.Name.Equals(customer.Name))
                {
                    customerComboBox.SelectedItem = c;
                    return;
                }
            }
           

        }
        public void SettDataBse(SQLiteConnection connection, MainWindow main)
        {
            this.connection = connection;
            window = main;
            GetCustomers();
            customerComboBox.ItemsSource = customers;
        }

        private void PhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*",
                Title = "Válassz ki egy fényképet"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                photoButton.Content = System.IO.Path.GetFileName(openFileDialog.FileName);
                photoFilePath = openFileDialog.FileName;
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameBox.Text))
            {
                MessageBox.Show("A Név mező kitöltése kötelező!");
                return;
            }

            if (string.IsNullOrWhiteSpace(addressBox.Text))
            {
                MessageBox.Show("A Cím mező kitöltése kötelező!");
                return;
            }

            if (string.IsNullOrWhiteSpace(orderNumberBox.Text))
            {
                MessageBox.Show("A Rendelési szám mező kitöltése kötelező!");
                return;
            }

            if (string.IsNullOrWhiteSpace(tajNumberBox.Text))
            {
                MessageBox.Show("A TAJ szám mező kitöltése kötelező!");
                return;
            }

            if (orderDateBox.SelectedDate == null)
            {
                MessageBox.Show("A Rendelési dátum mező kitöltése kötelező!");
                return;
            }
            string name = nameBox.Text;
            string address = addressBox.Text;
            string tajNumber = tajNumberBox.Text;
            string orderNumber = orderNumberBox.Text;
            string orderDate = orderDateBox.SelectedDate.Value.ToString("yyyy-MM-dd");
            string orderReleaseDate = orderReleaseDateBox.SelectedDate.HasValue ? orderReleaseDateBox.SelectedDate.Value.ToString("yyyy-MM-dd") : "NULL";
            string photoNewName = HandlePhotoFile(photoFilePath, orderNumber, name, orderDate);

            try
            {
                SQLiteCommand cmd = new SQLiteCommand();
                if (newOrder || customer == null)
                {
                    string insertCustomerQuery = $"INSERT OR IGNORE INTO customers (name, address, tajNumber) VALUES ('{name}', '{address}', '{tajNumber}')";
                    cmd = new SQLiteCommand(insertCustomerQuery, connection);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    string updateCustomerQuery = $"UPDATE customers SET name = '{name}', address = '{address}', tajNumber = '{tajNumber}' WHERE id = {customer.CustomerId}";
                    cmd = new SQLiteCommand(updateCustomerQuery, connection);
                    cmd.ExecuteNonQuery();
                }


                string insertShoesQuery = "";

                if (newOrder || customer == null)
                {
                    int customerId = GetCustomerId(name, address, tajNumber);
                    insertShoesQuery = $"INSERT INTO shoes " +
                    $"(orderNumber, orderDate, orderReleaseDate, photoPath, customerId) VALUES " +
                    $"('{orderNumber}', '{orderDate}', '{orderReleaseDate}', '{photoNewName}', " +
                    $"'{customerId}')";
                }
                else
                {
                    insertShoesQuery = $"UPDATE shoes SET " +
                        $"orderNumber = '{orderNumber}', " +
                        $"orderDate = '{orderDate}', " +
                        $"orderReleaseDate = '{orderReleaseDate}', " +
                        $"photoPath = '{photoNewName}', " +
                        $"customerId = '{customer.CustomerId}' " +
                        $"WHERE id = {customer.ShoeId}";
                }
                cmd = new SQLiteCommand(insertShoesQuery, connection); ;
                cmd.ExecuteNonQuery();

                if(newOrder) MessageBox.Show("Adatok sikeresen elmentve!");
                else MessageBox.Show("Adatok sikeresen frissitve!");
                window.RefreshData();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt az adatok mentése során! Hibaüzenet: {ex.Message}");
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TajNumberBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                string unformattedText = new String(textBox.Text.Where(Char.IsDigit).ToArray());
                if (unformattedText.Length > 9)
                {
                    unformattedText = unformattedText.Substring(0, 9);
                }
                if (unformattedText.Length > 3) unformattedText = unformattedText.Insert(3, " ");
                if (unformattedText.Length > 7) unformattedText = unformattedText.Insert(7, " ");
                textBox.Text = unformattedText;
                textBox.CaretIndex = textBox.Text.Length;
            }
        }


        private void GetCustomers()
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT id, name, address, tajNumber  FROM customers";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var customer = new Custumer
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Address = reader.GetString(2),
                        TAJNumber = reader.GetString(3)
                    };
                    customers.Add(customer);
                }
            }
        }

        private void CustomerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCustomer = (Custumer)customerComboBox.SelectedItem;
            if(selectedCustomer != null && selectedCustomer.Id != -1) 
            {
                nameBox.Text = selectedCustomer.Name;
                addressBox.Text = selectedCustomer.Address;
                tajNumberBox.Text = selectedCustomer.TAJNumber;
            }
            else 
            {
                nameBox.Text = "";
                addressBox.Text = "";
                tajNumberBox.Text = "";
            }
        }


        private static string GenerateRandomSuffix(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        private string HandlePhotoFile(string photoFilePath, string orderNumber, string name, string date)
        {
            if (string.IsNullOrEmpty(photoFilePath) || !photoFilePath.Equals("NULL"))
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string newDirectory = System.IO.Path.Combine(baseDirectory, "images/order");

                if (!Directory.Exists(newDirectory))
                {
                    Directory.CreateDirectory(newDirectory);
                }
                string newFileName = "";
                if (newOrder || (customer != null && !customer.PhotoPath.Equals("NULL")))
                {
                    newFileName = $"{orderNumber}_{name}_{date}_{GenerateRandomSuffix(5)}{System.IO.Path.GetExtension(photoFilePath)}";
                }
                else
                {
                    newFileName = customer.PhotoPath;
                }
                string newFilePath = System.IO.Path.Combine(newDirectory, newFileName);

                File.Copy(photoFilePath, newFilePath, true);
                return newFileName;
            }
            else return "NULL";
        }
        public int GetCustomerId(string name, string address, string tajNumber)
        {
            string sql = $"SELECT id FROM customers WHERE name = @name AND address = @address AND tajNumber = @tajNumber LIMIT 1";
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@address", address);
                command.Parameters.AddWithValue("@tajNumber", tajNumber);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                    else
                    {
                        throw new Exception("Customer not found");
                    }
                }
            }
        }

     

    }
}
