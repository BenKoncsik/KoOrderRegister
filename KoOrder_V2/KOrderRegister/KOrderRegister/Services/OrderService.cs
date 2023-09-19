using Microsoft.Win32;
using KoOrderRegister.Logs;
using KoOrderRegister.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Shapes;
using MessageBox = System.Windows.Forms.MessageBox;
using Path = System.IO.Path;
using KoOrderRegister.I18N;

namespace KoOrderRegister.Services
{
    public class OrderService
    {
        public static bool databesInitzialized = false;
        private static string _databasePath;

        public OrderService()
        {
            if (!databesInitzialized) OrderService.OpenConnection();
        }

        public static SQLiteConnection OpenConnection()
        {
            Setting dataBaseLocation = SettingsService.GetSetting(SettingsService.DataBaseLocation);
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _databasePath = Path.Combine(documentsPath, "koorderregister", "products.db");

            SQLiteConnection connection;
            if (dataBaseLocation != null && File.Exists(dataBaseLocation.Value))
            {
                connection = new SQLiteConnection($@"Data Source={dataBaseLocation.Value};");
                connection.Open();
                databesInitzialized = true;
                return connection;
            }

            if (!File.Exists(_databasePath))
            {
                if (!Directory.Exists(Path.GetDirectoryName(_databasePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_databasePath));
                }
                CreateDatabase();
                connection = new SQLiteConnection($@"Data Source={_databasePath};");
                if (connection != null)
                {
                    databesInitzialized = true;
                }
                else
                {
                    MessageBox.Show(Resources.ErrorDataaseConnection);
                }
                return connection;
            }

            connection = new SQLiteConnection($@"Data Source={_databasePath};");
            SettingsService.SaveSetting(new Setting(SettingsService.DataBaseLocation, _databasePath));
            connection.Open();
            databesInitzialized = true;
            return connection;
        }

        public static void CreateDatabase()
        {
            try
            {
                Setting dataBaseLocation = SettingsService.GetSetting(SettingsService.DataBaseLocation);
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                _databasePath = Path.Combine(documentsPath, "koorderregister", "products.db");
                SQLiteConnection.CreateFile(_databasePath);
                SettingsService.SaveSetting(new Setting(SettingsService.DataBaseLocation, _databasePath));
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_databasePath};"))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"CREATE TABLE customers (
                                    id INTEGER PRIMARY KEY, 
                                    name TEXT, 
                                    address TEXT,
                                    note TEXT,
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
                                    note TEXT,  
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
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                MessageBox.Show($"{Resources.FaliedDatabaseConnection}:{ex.Message}");
            }
        }
    public static List<CustomerProduct> GetCustumerPriducts(string searchText = "")
        {
            List<CustomerProduct> customerProducts = new List<CustomerProduct>();
            SQLiteConnection connection = null;
            try
            {
                connection = OpenConnection();
                string query;
                if (string.IsNullOrEmpty(searchText))
                {
                    query = @"SELECT p.id, p.orderNumber, p.orderDate, p.orderReleaseDate, p.note,
                        c.name, c.address, c.tajNumber, c.id as customerId
              FROM products p
              LEFT JOIN customers c ON p.customerId = c.id";
                }
                else
                {
                    query = @"SELECT p.id, p.orderNumber, p.orderDate, p.orderReleaseDate, p.note,
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
                                Note = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                Name = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                Address = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                TajNumber = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                CustomerId = reader.IsDBNull(8) ? -1 : reader.GetInt64(8)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                System.Windows.MessageBox.Show($"{Resources.InValidDatabes} {ex.Message}");
                return null;
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return customerProducts;
        }



        public static bool deletOrder(CustomerProduct delteOreder)
        {
            using (var connection = OpenConnection())
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
                    Logger.LogException(ex);
                    transaction.Rollback();
                    MessageBox.Show($"{Resources.ErrorOrderDelete} {Resources.ErrorMsgLabel} {ex.Message}");
                    return false;
                }
            }
        }

        public static bool saveNewOrder(CustomerProduct customerProduct, bool newOrder = true)
        {
            try
            {
                long customerId;
                long productId;
                using (var connection = OpenConnection())
                {
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
                        string sql = @"INSERT INTO products (orderNumber, orderDate, orderReleaseDate, note, customerId) 
                   VALUES (@OrderNumber, @OrderDate, @OrderReleaseDate, @Note, @CustomerId);
                   SELECT last_insert_rowid();";

                        using (var command = new SQLiteCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@OrderNumber", customerProduct.OrderNumber);
                            command.Parameters.AddWithValue("@OrderDate", customerProduct.OrderDate);
                            command.Parameters.AddWithValue("@OrderReleaseDate", customerProduct.OrderReleaseDate);
                            command.Parameters.AddWithValue("@Note", customerProduct.Note);
                            command.Parameters.AddWithValue("@CustomerId", customerId);
                            productId = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                    else
                    {
                        string sql = @"UPDATE products SET orderNumber = @OrderNumber, orderDate = @OrderDate, note = @Note, customerId = @CustomerId, 
                          orderReleaseDate = @OrderReleaseDate WHERE id = @ProductId";

                        using (var command = new SQLiteCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@OrderNumber", customerProduct.OrderNumber);
                            command.Parameters.AddWithValue("@OrderDate", customerProduct.OrderDate);
                            command.Parameters.AddWithValue("@OrderReleaseDate", customerProduct.OrderReleaseDate);
                            command.Parameters.AddWithValue("@CustomerId", customerProduct.Custumer.Id);
                            command.Parameters.AddWithValue("@Note", customerProduct.Note);
                            command.Parameters.AddWithValue("@ProductId", customerProduct.ProductId);
                            command.ExecuteNonQuery();
                        }
                        productId = customerProduct.ProductId;
                    }
                    connection.Close();
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
                    MessageBox.Show($"{Resources.SuccessfullSaving}");
                else
                    MessageBox.Show($"{Resources.SuccessfullUpdating}");

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                MessageBox.Show($"{Resources.ErrorSavingData} {Resources.ErrorMsgLabel} {ex.Message}");
                return false;
            }
        }



        public static List<Customer> GetCustomers()
        {
            using (var connection = OpenConnection())
            {
                List<Customer> customers = new List<Customer>();
                customers.Add(new Customer($"{Resources.NewMan}"));
                return CustomerService.getAllCustumers(customers);
            }
        }


    }
}
