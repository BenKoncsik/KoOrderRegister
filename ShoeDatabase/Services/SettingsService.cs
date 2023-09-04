using Microsoft.Win32;
using ShoeDatabase.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShoeDatabase.Services
{

    public class SettingsService
    {
        public static SQLiteConnection connection;
        public static bool databesInitzialized = false;

        public static string DataBaseLocation { get; } = "dataBaseLocation";
        public SettingsService() 
        {
            if (!databesInitzialized)
            {
                ConectDateBase();
            }
        }

        private void ConectDateBase()
        {
            
            if (!File.Exists("settings.db"))
            {
                using (connection = new SQLiteConnection("Data Source=settings.db"))
                {
                    connection.Open();

                    string createBackupTableQuery = @"CREATE TABLE backup (
                id INTEGER PRIMARY KEY,
                location TEXT NOT NULL,
                name TEXT NOT NULL,
                date TEXT NOT NULL
            )";

                    string createSettingsTableQuery = @"CREATE TABLE settings (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                setting_name TEXT NOT NULL,
                setting_value TEXT NOT NULL
            )";

                    using (SQLiteCommand command = new SQLiteCommand(createBackupTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    using (SQLiteCommand command = new SQLiteCommand(createSettingsTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                databesInitzialized = true;
            }
            else
            {
                connection = new SQLiteConnection(@"Data Source=settings.db;");
                connection.Open();
                CheckDatabaseIntegrity();
                databesInitzialized = true;
            }
        }

        public Setting GetSetting(string name)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=settings.db"))
            {
                connection.Open();

                string sql = $"SELECT * FROM settings WHERE setting_name = @name LIMIT 1";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@name", name);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Setting setting = new Setting()
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Value = reader.GetString(2)
                            };

                            return setting;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }
        public Dictionary<string, string> GetAllSettings()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=settings.db"))
            {
                connection.Open();

                string sql = $"SELECT * FROM settings";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            settings[reader.GetString(1)] = reader.GetString(2);
                        }
                    }
                }
            }

            return settings;
        }


        public bool SaveSetting(Setting setting)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=settings.db"))
                {
                    connection.Open();
                    string sql = "INSERT OR REPLACE INTO settings (setting_name, setting_value) VALUES (@name, @value)";


                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", setting.Name);
                        command.Parameters.AddWithValue("@value", setting.Value);

                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a beállítás mentésébe: " + ex.Message);
                return false;
            }
        }

        private void CheckDatabaseIntegrity()
        {
            string checkIntegrityQuery = "PRAGMA integrity_check";
            using (SQLiteCommand command = new SQLiteCommand(checkIntegrityQuery, connection))
            {
                string result = (string)command.ExecuteScalar();
                if (result != "ok")
                {
                    throw new Exception("Adatbázis integritás hiba: " + result);
                }
            }
        }





    }
}
