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
        public CustumerService() 
        {
            if (!OrderService.databesInitzialized)
            {
                OrderService.OpenConnection();
            }
        }



        public static List<Custumer> getAllCustumers(List<Custumer> custumers = null)
        {
            using (var connection = OrderService.OpenConnection())
            {
                if (custumers == null)
                {
                    custumers = new List<Custumer>();
                }
                var command = connection.CreateCommand();
                command.CommandText = "SELECT id, name, address, note, tajNumber  FROM customers";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var customer = new Custumer
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Address = reader.GetString(2),
                            Note = reader.GetString(3),
                            TAJNumber = reader.GetString(4)

                        };
                        custumers.Add(customer);
                    }
                }
                return custumers ?? new List<Custumer>();
            }
        }


        public static Custumer GetCustomer(string name, string address, string tajNumber, string note = "")
        {
            using (var connection = OrderService.OpenConnection())
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
                            return new Custumer(reader.GetInt32(0), name, address, tajNumber, note);
                        }
                        else
                        {
                            throw new Exception("Customer not found");
                        }
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
                        using (var connection = OrderService.OpenConnection())
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
                using (var connection = OrderService.OpenConnection())
                {
                    string sql = "INSERT INTO customers (name, address, note, tajNumber) VALUES (@name, @address, @note, @tajNumber)";
                    if (custumer.Id != -1)
                    {
                        sql = "UPDATE customers SET name = @name, address = @address, note = @note, tajNumber = @tajNumber WHERE id = @id";

                    }

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        if (custumer.Id != -1) command.Parameters.AddWithValue("@id", custumer.Id);
                        command.Parameters.AddWithValue("@name", custumer.Name);
                        command.Parameters.AddWithValue("@address", custumer.Address);
                        command.Parameters.AddWithValue("@note", custumer.Note);
                        command.Parameters.AddWithValue("@tajNumber", custumer.TAJNumber);

                        command.ExecuteNonQuery();
                        return true;
                    }
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
