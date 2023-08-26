using Microsoft.Win32;
using ShoeDatabase.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShoeDatabase.Services
{
    public class CustumerService
    {
        public static SQLiteConnection connection;
        private static SettingsService settingsService = new SettingsService();
        private static OrderService orderService = new OrderService();
        public static bool databesInitzialized = false;
        public CustumerService() 
        {
            if (!databesInitzialized)
            {
                ConectDateBase();
            }
        }

        public static void ConectDateBase()
        {
            Setting dataBaseLocation = settingsService.GetSetting(SettingsService.DataBaseLocation);
            if (dataBaseLocation != null && File.Exists(dataBaseLocation.Value))
            {
                connection = new SQLiteConnection($@"Data Source={dataBaseLocation.Value};");
                connection.Open();
                databesInitzialized = true;
            }
            else
            {
                if (!File.Exists("products.db") || settingsService.GetSetting(SettingsService.DataBaseLocation) == null)
                {
                    connection = new SQLiteConnection(@"Data Source=pruducts.db;");
                    connection.Open();
                    databesInitzialized = true;
                }
                else 
                {
                    OrderService.ConectDateBase();
                }
            }
        }


        public List<Custumer> getAllCustumers(List<Custumer> custumers = null)
        {
            if(custumers == null)
            {
                custumers = new List<Custumer>();
            }
            
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
                    custumers.Add(customer);
                }
            }
            return custumers;
        }


        public static Custumer GetCustomer(string name, string address, string tajNumber)
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
                        return new Custumer(reader.GetInt32(0), name, address, tajNumber);
                    }
                    else
                    {
                        throw new Exception("Customer not found");
                    }
                }
            }
        }

            public static bool deleteCustumer(CustomerProduct pruduct)
            {
                try
                {
                    if (pruduct != null)
                    {
                        var messageBoxResult = MessageBox.Show($"Biztosan törölni szeretné a {pruduct.Name} ügyfél adatait?", "Törlés megerősítése", MessageBoxButton.YesNo);
                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(connection))
                            {
                                cmd.CommandText = $"UPDATE products SET customerId = -1 WHERE customerId = {pruduct.CustomerId};";
                                cmd.ExecuteNonQuery();

                                cmd.CommandText = $"DELETE FROM customers WHERE id = {pruduct.CustomerId};";
                                cmd.ExecuteNonQuery();
                            }
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Nem sikerült törölni az ügyél adatait, mert a kiválasztott elem nem a várt típusú. \nHiba" + ex.Message);
                }
                return false;
            }

        public bool saveCustumer(Custumer custumer)
        {
            try
            {
                string sql = "INSERT INTO customers (name, address, tajNumber) VALUES (@name, @address, @tajNumber)";
                if (custumer.Id != -1)
                {
                    sql = "UPDATE customers SET name = @name, address = @address, tajNumber = @tajNumber WHERE id = @id"; 
                    
                }
                
                using (var command = new SQLiteCommand(sql, connection))
                    {
                       if(custumer.Id != -1) command.Parameters.AddWithValue("@id", custumer.Id);
                        command.Parameters.AddWithValue("@name", custumer.Name);
                        command.Parameters.AddWithValue("@address", custumer.Address);
                        command.Parameters.AddWithValue("@tajNumber", custumer.TAJNumber);

                        command.ExecuteNonQuery();
                        return true;
                    }
                }
                
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a vásárló mentése során: " + ex.Message);
                return false;
            }
        

        }

        
    }
}
