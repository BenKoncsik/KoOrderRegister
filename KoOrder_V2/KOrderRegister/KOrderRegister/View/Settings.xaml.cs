using Microsoft.Win32;
using KoOrderRegister.Model;
using KoOrderRegister.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KoOrderRegister.View
{
  
    public partial class Settings : Window
    {
        private static SettingsService settingService = new SettingsService();
        private static OrderService orderService = new OrderService();
      
        public Settings()
        {
            InitializeComponent();
            GetSettings();
        }

        private void SelectDataBaseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SQLite database (*.db)|*.db";
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                SettingsService.SaveSetting(new Setting(SettingsService.DataBaseLocation, selectedFilePath));
                dataBaseLocation.Content = selectedFilePath;
            }
        }

        private void GetSettings()
        {
            Setting dataBaseLocationSetting = SettingsService.GetSetting(SettingsService.DataBaseLocation);
            if (dataBaseLocationSetting != null)
            {
                dataBaseLocation.Content = dataBaseLocationSetting.Value;
            }
        }



    }
}
