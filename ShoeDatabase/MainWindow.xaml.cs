using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
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

namespace ShoeDatabase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SQLiteConnection connection;
        private SQLiteDataAdapter adapter;
        private DataTable dataTable;
        private static bool databesInitzialized = false;
        public MainWindow()
        {
            InitializeComponent();
            ConectDateBase();

        }

        private void ConectDateBase()
        {
            if (!File.Exists("shoe.db"))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "SQLite adatbázis (*.db)|*.db";
                if (openFileDialog.ShowDialog() == true)
                {
                    connection = new SQLiteConnection($@"Data Source={openFileDialog.FileName};");
                    connection.Open();
                    databesInitzialized = true;
                    RefreshData();
                }
                else
                {
                    MessageBox.Show("Nem választott ki adatbázist.");
                    Application.Current.Shutdown();
                }
            }
            else
            {
                connection = new SQLiteConnection(@"Data Source=shoe.db;");
                connection.Open();
                databesInitzialized = true;
                RefreshData();
            }

        }

        public void RefreshData()
        {
            if (databesInitzialized)
            {
                try
                {
                    string query;
                    string searchText = searchBox.Text;
                    Console.WriteLine($"Search Text: {searchText}");
                    if (string.IsNullOrEmpty(searchText))
                    {
                        query = @"SELECT s.id, s.orderNumber, s.orderDate, s.orderReleaseDate, s.photoPath,
                                  c.name, c.address, c.tajNumber, c.id as coustumer_id
                                    FROM shoes s 
                                    LEFT JOIN customers c 
                                    ON s.customerId = c.id";
                    }
                    else
                    {
                        query = $@"
                                SELECT s.id as shoue_id, s.orderNumber, s.orderDate, s.orderReleaseDate, s.photoPath,
                                        c.name, c.address, c.tajNumber, c.id as coustumer_id
                                FROM shoes s 
                                LEFT JOIN customers c 
                                ON s.customerId = c.id 
                                WHERE c.name LIKE '%{searchText}%' OR
                                      c.address LIKE '%{searchText}%' OR
                                      c.tajNumber LIKE '%{searchText}%' OR
                                      s.orderNumber LIKE '%{searchText}%' OR
                                      s.orderDate LIKE '%{searchText}%' OR
                                      s.orderReleaseDate LIKE '%{searchText}%'";
                    }

                    List<CustomerShoeInfo> customerShoeInfos = new List<CustomerShoeInfo>();

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                customerShoeInfos.Add(new CustomerShoeInfo
                                {
                                    ShoeId = reader.IsDBNull(0) ? -1 : reader.GetInt32(0),
                                    OrderNumber = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    OrderDate = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    OrderReleaseDate = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    PhotoPath = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    Name = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                    Address = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                    TajNumber = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                    CustomerId = reader.IsDBNull(8) ? -1 : reader.GetInt32(8)
                                });
                            }

                            dataGrid.ItemsSource = customerShoeInfos;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Nem megfelelő adatbázis!\n" + ex.Message);
                }
            }
        }




        private void New_Click(object sender, RoutedEventArgs e)
        {
            NewEntryWindow newEntryWindow = new NewEntryWindow();
            newEntryWindow.SettDataBse(connection, this);
            newEntryWindow.Show();
        }

        private void SearchBox_CLick(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshData();
        }
        private void UpdateBox_CLick(object sender, RoutedEventArgs e)
        {
            searchBox.Text = "";
            RefreshData();
        }

        private void DeveloperContact_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Email: kocsik.benedek.andras@gmail.com");
        }

        private void DeveloperSourceCode_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/Koncsik-cyber/ShoeOrderCustumerRegister";

            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a böngésző megnyitásakor: {ex.Message}\n url: {url}");
            }
        }
        private void DeveloperWebSite_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://koncsik.hopto.org/";

            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a böngésző megnyitásakor: {ex.Message}\n url: {url}");
            }
        }
        private void DeveloperX_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://twitter.com/BenedekKoncsik?t=JmLhG8S77hWOWqdrpitPJw&s=09";

            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a böngésző megnyitásakor: {ex.Message}\n url: {url}");
            }
        }


        private void CreateDatabase_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "SQLite Database|*.db";
            if (saveFileDialog.ShowDialog() == true)
            {
                string dbPath = saveFileDialog.FileName;
                SQLiteConnection.CreateFile(dbPath);
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};"))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"
                            CREATE TABLE customers (
                            id INTEGER PRIMARY KEY, 
                            name TEXT, 
                            address TEXT,
                            tajNumber TEXT UNIQUE
                );";
                        command.ExecuteNonQuery();

                        command.CommandText = @"
                            CREATE TABLE shoes (
                            id INTEGER PRIMARY KEY, 
                            orderNumber TEXT,
                            orderDate TEXT,
                            orderReleaseDate TEXT,
                            photoPath TEXT,
                            customerId INTEGER,
                            FOREIGN KEY(customerId) REFERENCES customers(id)
                );";
                        command.ExecuteNonQuery();
                    }
                }

                this.connection = new SQLiteConnection($"Data Source={dbPath};");
                this.connection.Open();
                RefreshData();
            }
        }

        private void ContextMenu_Delete(object sender, RoutedEventArgs e)
        {
            try
            {
                CustomerShoeInfo selectedItem = (CustomerShoeInfo)dataGrid.SelectedItem;

                if (selectedItem != null)
                {
                    var messageBoxResult = MessageBox.Show($"Biztosan törölni szeretné a {selectedItem.Name} ügyfél {selectedItem.OrderNumber}?", "Törlés megerősítése", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        var selectedShoe = selectedItem;
                        if (!selectedShoe.PhotoPath.Equals("null"))
                        {
                            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                            string directory = System.IO.Path.Combine(baseDirectory, "images/order");
                            string filePath = System.IO.Path.Combine(directory, selectedItem.PhotoPath);
                            if (File.Exists(filePath))
                            {
                                try
                                {
                                    File.Delete(filePath);
                                }
                                catch (IOException ex)
                                {
                                    MessageBox.Show($"Nem sikerült törölni a képet: {filePath}");
                                }
                            }
                        }
                        string sql = $"DELETE FROM shoes WHERE id = @id";

                        using (var command = new SQLiteCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@id", selectedShoe.ShoeId);
                            command.ExecuteNonQuery();
                        }

                        RefreshData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nem sikerült törölni a sor adatait, mert a kiválasztott elem nem a várt típusú. \nHiba" + ex.Message);
            }
        }

        private void ContextMenu_Change(object sender, RoutedEventArgs e)
        {
            CustomerShoeInfo selectedItem = (CustomerShoeInfo)dataGrid.SelectedItem;
            if (selectedItem != null)
            {
                NewEntryWindow newEntryWindow = new NewEntryWindow(selectedItem, connection, this);
                newEntryWindow.Show();
            }
        }

        private void ContextMenu_Delete_Custumer(object sender, RoutedEventArgs e)
        {
            try
            {
                CustomerShoeInfo selectedItem = (CustomerShoeInfo)dataGrid.SelectedItem;

                if (selectedItem != null)
                {
                    var messageBoxResult = MessageBox.Show($"Biztosan törölni szeretné a {selectedItem.Name} ügyfél adatait?", "Törlés megerősítése", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(connection))
                        {
                            cmd.CommandText = $"UPDATE shoes SET customerId = -1 WHERE customerId = {selectedItem.CustomerId};";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = $"DELETE FROM customers WHERE id = {selectedItem.CustomerId};";
                            cmd.ExecuteNonQuery();
                        }

                        RefreshData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nem sikerült törölni az ügyél adatait, mert a kiválasztott elem nem a várt típusú. \nHiba" + ex.Message);
            }
        }

        private void OpenPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string photoPath = ((CustomerShoeInfo)button.DataContext).PhotoPath;
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "images/order/" + photoPath;
            if (File.Exists(filePath))
            {
                System.Diagnostics.Process.Start(filePath);
            }
            else
            {
                MessageBox.Show("A képfájl nem található: " + photoPath);
            }
        }



    }
}
    