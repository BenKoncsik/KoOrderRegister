using Microsoft.Win32;
using ShoeDatabase.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows;

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
            if (!databesInitzialized) ConectDateBase();
        }

        public static void ConectDateBase()
        {
            Setting dataBaseLocation = settingsService.GetSetting(SettingsService.DataBaseLocation);
            if (dataBaseLocation != null && File.Exists(dataBaseLocation.Value))
            {
                connection = new SQLiteConnection($@"Data Source={dataBaseLocation.Value};");
                connection.Open();
                databesInitzialized = true;
                return;
            }
            if (!File.Exists("products.db") || settingsService.GetSetting(SettingsService.DataBaseLocation) == null)
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
                connection = new SQLiteConnection(@"Data Source=products.db;");
                connection.Open();
                databesInitzialized = true;
            }
        }
        public static List<CustomerProduct> GetCustumerPriducts(string searchText = "")
        {
            List<CustomerProduct> customerProducts = new List<CustomerProduct>();

            if (!databesInitzialized)
                ConectDateBase();  // Feltételezem, hogy ez a metódus létezik és inicializálja az adatbázis kapcsolatot

            string query;
            if (string.IsNullOrEmpty(searchText))
            {
                query = @"SELECT p.id, p.orderNumber, p.orderDate, p.orderReleaseDate,
                          c.name, c.address, c.tajNumber, c.id as customerId
                  FROM products p
                  LEFT JOIN customers c ON p.customerId = c.id";
            }
            else
            {
                query = @"SELECT p.id, p.orderNumber, p.orderDate, p.orderReleaseDate,
                          c.name, c.address, c.tajNumber, c.id as customerId
                  FROM products p
                  LEFT JOIN customers c ON p.customerId = c.id
                  WHERE c.name LIKE @SearchText OR
                        c.address LIKE @SearchText OR
                        c.tajNumber LIKE @SearchText OR
                        p.orderNumber LIKE @SearchText OR
                        p.orderDate LIKE @SearchText OR
                        p.orderReleaseDate LIKE @SearchText";
            }

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                if (!string.IsNullOrEmpty(searchText))
                {
                    string searchPattern = "%" + searchText + "%";
                    command.Parameters.AddWithValue("@SearchText", searchPattern);
                }

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customerProducts.Add(new CustomerProduct
                        {
                            ProductId = reader.IsDBNull(0) ? -1 : reader.GetInt64(0),
                            OrderNumber = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            OrderDate = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            OrderReleaseDate = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Name = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            Address = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            TajNumber = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            CustomerId = reader.IsDBNull(7) ? -1 : reader.GetInt64(7)
                        });
                    }
                }
            }

            return customerProducts;
        }

       


        public static SQLiteConnection createDateBase()
        {
            try
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
                            command.CommandText = @"CREATE TABLE customers (
                                                id INTEGER PRIMARY KEY, 
                                                name TEXT, 
                                                address TEXT,
                                                tajNumber TEXT UNIQUE
                                                );";
                            command.ExecuteNonQuery();

                            command.CommandText = @"CREATE TABLE files (
                                                fileId INTEGER PRIMARY KEY,
                                                fileName TEXT,
                                                fileBlob BLOB
                                            );";
                            command.ExecuteNonQuery();

                            command.CommandText = @"CREATE TABLE products (
                                                id INTEGER PRIMARY KEY, 
                                                orderNumber TEXT,
                                                orderDate TEXT,
                                                orderReleaseDate TEXT,
                                                customerId INTEGER,
                                                FOREIGN KEY(customerId) REFERENCES customers(id)
                                                );";
                            command.ExecuteNonQuery();

                            command.CommandText = @"CREATE TABLE product_files (
                                                productId INTEGER,
                                                fileId INTEGER,
                                                FOREIGN KEY(productId) REFERENCES products(id),
                                                FOREIGN KEY(fileId) REFERENCES files(fileId)
                                            );";
                            command.ExecuteNonQuery();

                        }
                    }
                    return connection;
                }
            }catch (Exception ex)
            {
                MessageBox.Show("Nem sikerült adatbázis létrehozni: " + ex.Message);
            }
            return null;
        }

        public static bool deletOrder(CustomerProduct delteOreder)
        {
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    // Töröljük az összes hozzá kapcsolódó fájlt a product_files táblából
                    string deleteFilesQuery = @"DELETE FROM product_files WHERE productId = @ProductId";
                    using (SQLiteCommand deleteFilesCommand = new SQLiteCommand(deleteFilesQuery, connection))
                    {
                        deleteFilesCommand.Parameters.AddWithValue("@ProductId", delteOreder.ProductId);
                        deleteFilesCommand.ExecuteNonQuery();
                    }

                    // Töröljük a terméket a products táblából
                    string deleteProductQuery = @"DELETE FROM products WHERE id = @ProductId";
                    using (SQLiteCommand deleteProductCommand = new SQLiteCommand(deleteProductQuery, connection))
                    {
                        deleteProductCommand.Parameters.AddWithValue("@ProductId", delteOreder.ProductId);
                        deleteProductCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Hiba történt a rendelés törlése során! Hibaüzenet: {ex.Message}");
                    return false;
                }
            }
        }

        public static bool saveNewOrder(CustomerProduct customerProduct, bool newOrder = true)
        {
            try
            {
                if (!databesInitzialized) ConectDateBase();

                long customerId;
                long productId;

                // Check if the customer already exists
                using (var checkCommand = new SQLiteCommand("SELECT id FROM customers WHERE tajNumber = @TajNumber", connection))
                {
                    checkCommand.Parameters.AddWithValue("@TajNumber", customerProduct.Custumer.TAJNumber);
                    object result = checkCommand.ExecuteScalar();

                    if (result != null) // customer exists
                    {
                        customerId = Convert.ToInt32(result);

                        // Update the customer's details
                        string updateSql = "UPDATE customers SET name = @Name, address = @Address WHERE id = @CustomerId";
                        using (var updateCommand = new SQLiteCommand(updateSql, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@Name", customerProduct.Custumer.Name);
                            updateCommand.Parameters.AddWithValue("@Address", customerProduct.Custumer.Address);
                            updateCommand.Parameters.AddWithValue("@CustomerId", customerId);
                            updateCommand.ExecuteNonQuery();
                        }
                    }
                    else // customer doesn't exist
                    {
                        string insertSql = @"INSERT INTO customers (name, address, tajNumber) 
                                     VALUES (@Name, @Address, @TajNumber);
                                     SELECT last_insert_rowid();";

                        using (var insertCommand = new SQLiteCommand(insertSql, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@Name", customerProduct.Custumer.Name);
                            insertCommand.Parameters.AddWithValue("@Address", customerProduct.Custumer.Address);
                            insertCommand.Parameters.AddWithValue("@TajNumber", customerProduct.Custumer.TAJNumber);
                            customerId = Convert.ToInt32(insertCommand.ExecuteScalar());
                        }
                    }
                }

                if (newOrder)
                {
                    string sql = @"INSERT INTO products (orderNumber, orderDate, orderReleaseDate, customerId) 
                   VALUES (@OrderNumber, @OrderDate, @OrderReleaseDate, @CustomerId);
                   SELECT last_insert_rowid();";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@OrderNumber", customerProduct.OrderNumber);
                        command.Parameters.AddWithValue("@OrderDate", customerProduct.OrderDate);
                        command.Parameters.AddWithValue("@OrderReleaseDate", customerProduct.OrderReleaseDate);
                        command.Parameters.AddWithValue("@CustomerId", customerId);
                        productId = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
                else
                {
                    string sql = @"UPDATE products SET orderNumber = @OrderNumber, orderDate = @OrderDate, 
                          orderReleaseDate = @OrderReleaseDate WHERE id = @ProductId";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@OrderNumber", customerProduct.OrderNumber);
                        command.Parameters.AddWithValue("@OrderDate", customerProduct.OrderDate);
                        command.Parameters.AddWithValue("@OrderReleaseDate", customerProduct.OrderReleaseDate);
                        command.Parameters.AddWithValue("@ProductId", customerProduct.ProductId);
                        command.ExecuteNonQuery();
                    }
                    productId = customerProduct.ProductId;
                }

                if (customerProduct.Files != null)
                {
                    foreach (FileBLOB file in customerProduct.Files)
                    {
                        file.ProductID = productId;
                        FileService.save(file);
                    }
                }

                if (newOrder)
                    MessageBox.Show("Adatok sikeresen elmentve!");
                else
                    MessageBox.Show("Adatok sikeresen frissitve!");

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt az adatok mentése során! Hibaüzenet: {ex.Message}");
                return false;
            }
        }



        public static List<Custumer> GetCustomers()
        {
            List<Custumer> customers = new List<Custumer>();
            customers.Add(new Custumer("Új ember"));
            return custumerService.getAllCustumers(customers);
        }


    }
}
