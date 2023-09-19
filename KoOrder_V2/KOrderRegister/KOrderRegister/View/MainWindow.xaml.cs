using Microsoft.Win32;
using KoOrderRegister.Logs;
using KoOrderRegister.Model;
using KoOrderRegister.Services;
using KoOrderRegister.View;
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

namespace KoOrderRegister
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
    
        private static OrderService orderService = new OrderService();
        private static SettingsService settingsService = new SettingsService();
        private static CustomerService custumerService = new CustomerService();
        public MainWindow()
        {
            InitializeComponent();
            RefreshData();
        }

      

        public void RefreshData()
        {  
            dataGrid.ItemsSource = OrderService.GetCustumerPriducts(searchBox.Text);            
        }




        private void New_Click(object sender, RoutedEventArgs e)
        {
            NewEntryWindow newEntryWindow = new NewEntryWindow();
            newEntryWindow.Closed += NewEntryWindow_Closed;
            newEntryWindow.Show();
            
        }
        private void newCustumerBox_CLick(object sender, RoutedEventArgs e)
        {
            NewCustomer newCustumer = new NewCustomer();
            newCustumer.Closed += NewEntryWindow_Closed;
            newCustumer.Show();

        }
        private void NewEntryWindow_Closed(object sender, EventArgs e)
        {
            RefreshData();
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
        private void SettingsBox_CLick(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.Closed += NewEntryWindow_Closed;
            settings.Show();
        }

        private void ContextMenu_Delete(object sender, RoutedEventArgs e)
        {
            try
            {
                CustomerProduct selectedItem = (CustomerProduct)dataGrid.SelectedItem;

                if (OrderService.deletOrder(selectedItem))
                {
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                MessageBox.Show("Nem sikerült törölni a sor adatait, mert a kiválasztott elem nem a várt típusú. \nHiba" + ex.Message);
            }
        }

        private void ContextMenu_Change(object sender, RoutedEventArgs e)
        {
            CustomerProduct selectedItem = (CustomerProduct)dataGrid.SelectedItem;
            if (selectedItem != null)
            {
                NewEntryWindow newEntryWindow = new NewEntryWindow(selectedItem);
                newEntryWindow.Closed += NewEntryWindow_Closed;
                newEntryWindow.Show();
            }
        }

        private void ContextMenu_Delete_Custumer(object sender, RoutedEventArgs e)
        {
            try
            {
                CustomerProduct selectedItem = (CustomerProduct)dataGrid.SelectedItem;

                if (CustomerService.deleteCustumer(selectedItem))
                {
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                MessageBox.Show("Nem sikerült törölni az ügyél adatait, mert a kiválasztott elem nem a várt típusú. \nHiba" + ex.Message);
            }
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
                Logger.LogException(ex);
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
                Logger.LogException(ex);
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
                Logger.LogException(ex);
                MessageBox.Show($"Hiba a böngésző megnyitásakor: {ex.Message}\n url: {url}");
            }
        }


        private void CreateDatabase_Click(object sender, RoutedEventArgs e)
        {
            OrderService.CreateDatabase();
            RefreshData();
        }
        private void CheckDB_Click(object sender, RoutedEventArgs e)
        {
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table';", OrderService.OpenConnection()))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MessageBox.Show(reader.GetString(0));
                    }
                }
            }
        }
       



    }
}
    