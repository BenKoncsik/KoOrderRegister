using ShoeDatabase.Model;
using ShoeDatabase.Services;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ShoeDatabase.View
{
    
    public partial class NewCustumer : Window
    {
        private List<Custumer> custumers = new List<Custumer>();
        private OrderService orderService = new OrderService();
        private CustumerService custumerService = new CustumerService();
        private long custumerId = -1;
        public NewCustumer()
        {
            InitializeComponent();
            custumers = OrderService.GetCustomers();
            custumerComboBox.ItemsSource = custumers;
        }

        private void CustomerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCustomer = (Custumer)custumerComboBox.SelectedItem;
            if (selectedCustomer != null && selectedCustomer.Id != -1)
            {
                nameBox.Text = selectedCustomer.Name;
                addressBox.Text = selectedCustomer.Address;
                tajNumberBox.Text = selectedCustomer.TAJNumber;
                custumerId = selectedCustomer.Id;
                noteBox.Text = selectedCustomer.Note;
                DeleteButton.Visibility = Visibility.Visible;

            }
            else
            {
                nameBox.Text = "";
                addressBox.Text = "";
                tajNumberBox.Text = "";
                noteBox.Text = "";
                custumerId = -1;
                DeleteButton.Visibility = Visibility.Collapsed;
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

        public void SaveButton_Click(object sender, RoutedEventArgs e)
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
            if (string.IsNullOrWhiteSpace(tajNumberBox.Text))
            {
                MessageBox.Show("A TAJ szám mező kitöltése kötelező!");
                return;
            }
            string note = "";
            if (!string.IsNullOrWhiteSpace(noteBox.Text))
            {
                note = noteBox.Text;
            }
            if (custumerService.saveCustumer(new Custumer(custumerId, nameBox.Text, addressBox.Text, note, tajNumberBox.Text)))
            {
                MessageBox.Show("Sikeres Mentés");
                custumers = OrderService.GetCustomers();
                custumerComboBox.ItemsSource = custumers;
                if (custumers != null)
                {
                    foreach (Custumer c in custumers)
                    {
                        if (c.TAJNumber != null && c.TAJNumber.Equals(tajNumberBox.Text))
                        {
                            custumerComboBox.SelectedItem = c;
                            return;
                        }
                    }
                }
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

            if (custumerId != -1) {
                MessageBoxResult result = MessageBox.Show("Biztosan törölni szeretné ezt az ügyfelet?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (Custumer c in custumers)
                    {
                        if (c.Id.Equals(custumerId))
                        {
                            CustumerService.deleteCustumer(new CustomerProduct(c));
                            break;
                        }
                    }
                }
                    
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
