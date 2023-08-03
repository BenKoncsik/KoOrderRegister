using Microsoft.Win32;
using ShoeDatabase.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace ShoeDatabase.Services
{
    public class OrderService
    {
        public static SQLiteConnection connection;
        private static SettingsService settingsService = new SettingsService();
        private static CustumerService custumerService = new CustumerService();
        public static bool databesInitzialized = false;

        public OrderService()
        {
            if(!databesInitzialized) ConectDateBase();
        }

        public static void ConectDateBase()
        {
            Setting dataBaseLocation = settingsService.GetSetting(SettingsService.DataBaseLocation);
            if(dataBaseLocation != null && File.Exists(dataBaseLocation.Value))
            {
                connection = new SQLiteConnection($@"Data Source={dataBaseLocation.Value};");
                connection.Open();
                databesInitzialized = true;
                return;
            }
            if (!File.Exists("shoe.db"))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "SQLite adatbázis (*.db)|*.db";
                if (openFileDialog.ShowDialog() == true)
                {
                    connection = new SQLiteConnection($@"Data Source={openFileDialog.FileName};");
                    connection.Open();
                    databesInitzialized = true;
                    settingsService.SaveSetting(new Setting(SettingsService.DataBaseLocation, openFileDialog.FileName));
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
            }
        }

        public static List<CustomerShoeInfo> GetCustumerShoInfo(string searchText = "")
        {
            if (databesInitzialized)
            {
                try
                {
                    string query;
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
                            return customerShoeInfos;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Nem megfelelő adatbázis!\n" + ex.Message);
                }
            }
            return new List<CustomerShoeInfo>();
        }


        public static SQLiteConnection createDateBase()
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
                return connection;
            }
            return null;
        }

        public static bool deletOrder(CustomerShoeInfo delteOreder)
        {
            try
            {
                if (delteOreder != null)
                {
                    var messageBoxResult = MessageBox.Show($"Biztosan törölni szeretné a {delteOreder.Name} ügyfél " +
                        $"{delteOreder.OrderNumber} számú rendelését?", "Törlés megerősítése", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        if (!delteOreder.PhotoPath.Equals("null"))
                        {
                            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                            string directory = System.IO.Path.Combine(baseDirectory, "images/order");
                            string filePath = System.IO.Path.Combine(directory, delteOreder.PhotoPath);
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
                            command.Parameters.AddWithValue("@id", delteOreder.ShoeId);
                            command.ExecuteNonQuery();
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nem sikerült törölni a sor adatait, mert a kiválasztott elem nem a várt típusú. \nHiba" + ex.Message);
            }
            return false;
        }

   


        public static string openImage(CustomerShoeInfo customerShoeInfo)
        {
            string photoPath = customerShoeInfo.PhotoPath;
            return AppDomain.CurrentDomain.BaseDirectory + "images/order/" + photoPath;
        }

        public static bool saveNewOrder(CustomerShoeInfo customerShoeInfo, bool newOrder = true)
        {
            try
            {
              string sql = "";

                    if (newOrder)
                    {
                //    sql = @"SELECT * FROM customers WHERE id = @Id;";


                    using (var command = new SQLiteCommand(sql, connection))
                        {
                            // Add parameters
                            command.Parameters.AddWithValue("@Name", customerShoeInfo.Custumer.Name);
                            command.Parameters.AddWithValue("@Address", customerShoeInfo.Custumer.Address);
                            command.Parameters.AddWithValue("@TajNumber", customerShoeInfo.Custumer.TAJNumber);

                            // Execute command and get the ID of the inserted customer
                            var customerId = (long)command.ExecuteScalar();

                            sql = @"INSERT INTO shoes (orderNumber, orderDate, orderReleaseDate, photoPath, customerId) 
                            VALUES (@OrderNumber, @OrderDate, @OrderReleaseDate, @PhotoPath, @CustomerId)";

                            using (var command2 = new SQLiteCommand(sql, connection))
                            {
                                // Add parameters
                                command2.Parameters.AddWithValue("@OrderNumber", customerShoeInfo.OrderNumber);
                                command2.Parameters.AddWithValue("@OrderDate", customerShoeInfo.OrderDate);
                                command2.Parameters.AddWithValue("@OrderReleaseDate", customerShoeInfo.OrderReleaseDate);
                                command2.Parameters.AddWithValue("@PhotoPath", customerShoeInfo.PhotoPath);
                                command2.Parameters.AddWithValue("@CustomerId", customerShoeInfo.Custumer.Id);

                                // Execute command
                                command2.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        sql = @"UPDATE shoes SET orderNumber = @OrderNumber, orderDate = @OrderDate, 
                        orderReleaseDate = @OrderReleaseDate, photoPath = @PhotoPath
                        WHERE id = @ShoeId";

                        using (var command = new SQLiteCommand(sql, connection))
                        {
                            // Add parameters
                            command.Parameters.AddWithValue("@OrderNumber", customerShoeInfo.OrderNumber);
                            command.Parameters.AddWithValue("@OrderDate", customerShoeInfo.OrderDate);
                            command.Parameters.AddWithValue("@OrderReleaseDate", customerShoeInfo.OrderReleaseDate);
                            command.Parameters.AddWithValue("@PhotoPath", customerShoeInfo.PhotoPath);
                            command.Parameters.AddWithValue("@ShoeId", customerShoeInfo.ShoeId);

                            // Execute command
                            command.ExecuteNonQuery();
                        }
                  }
                if (newOrder) MessageBox.Show("Adatok sikeresen elmentve!");
                else MessageBox.Show("Adatok sikeresen frissitve!");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt az adatok mentése során! Hibaüzenet: {ex.Message}");
            }
            return false;
        }


        public static List<Custumer> GetCustomers()
        {
            List<Custumer> customers = new List<Custumer>();
            customers.Add(new Custumer("Új ember"));
            return custumerService.getAllCustumers(customers);   
        }


       












    }
}
