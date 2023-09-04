using Microsoft.Win32;
using ShoeDatabase.Model;
using ShoeDatabase.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShoeDatabase
{
    /// <summary>
    /// Interaction logic for NewEntryWindow.xaml
    /// </summary>
    public partial class NewEntryWindow : Window
    { 
        private string photoFilePath = "NULL";
        private List<Custumer> custumers = new List<Custumer>();
        private bool newOrder = true;
        private DateTime OrderDateTime { get; set; } = DateTime.Now;
        private CustomerProduct customerShoeInfo = new CustomerProduct();
        private OrderService orderService = new OrderService();
        private CustumerService custumerService = new CustumerService();
        private ObservableCollection<FileBLOB> images = new ObservableCollection<FileBLOB>();
        public NewEntryWindow()
        {
            InitializeComponent();
            noteBox.Text = "";
            custumers = custumerService.getAllCustumers(custumers);
            if(custumers.Count <= 0)
            {
                custumers.Add(new Custumer("Nincs ember felvéve!"));
            }
            custumerComboBox.ItemsSource = custumers;

            ImagesListView.ItemsSource = images;
        }

        public NewEntryWindow(CustomerProduct customer)
        {
            InitializeComponent();
            newOrder = false;
            this.customerShoeInfo = customer;
            nameBox.Text = customer.Name;
            addressBox.Text = customer.Address;
            tajNumberBox.Text = customer.TajNumber;
            orderNumberBox.Text = customer.OrderNumber;
            custumers = custumerService.getAllCustumers();
            noteBox.Text = customer.Note;
            custumerComboBox.ItemsSource = custumers;
            DateTime dateTime = DateTime.Now;
            if (!customer.OrderDate.Equals("NULL"))
            {
                if (DateTime.TryParseExact(customer.OrderDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    orderDateBox.SelectedDate = dateTime;
                }
                else
                {
                    MessageBox.Show("Hiba a megrendelési dátum megjelenítésénél!");
                }
            }
            
            if (!customer.OrderReleaseDate.Equals("NULL") && DateTime.TryParseExact(customer.OrderDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                orderReleaseDateBox.SelectedDate = dateTime;
            }
            images = FileService.getFilesObservable(customerShoeInfo.ProductId);
            ImagesListView.ItemsSource = images;
            foreach (Custumer c in custumers) 
            {
                if (c.TAJNumber.Equals(customer.TajNumber))
                {
                    custumerComboBox.SelectedItem = c;
                    return;
                }
            }
           

        }
     

     


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
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

            if (string.IsNullOrWhiteSpace(orderNumberBox.Text))
            {
                MessageBox.Show("A Rendelési szám mező kitöltése kötelező!");
                return;
            }

            if (string.IsNullOrWhiteSpace(tajNumberBox.Text))
            {
                MessageBox.Show("A TAJ szám mező kitöltése kötelező!");
                return;
            }

            if (orderDateBox.SelectedDate == null)
            {
                MessageBox.Show("A Rendelési dátum mező kitöltése kötelező!");
                return;
            }
            string name = nameBox.Text;
            string address = addressBox.Text;
            string tajNumber = tajNumberBox.Text;
            string orderNumber = orderNumberBox.Text;
            string orderDate = orderDateBox.SelectedDate.Value.ToString("yyyy-MM-dd");
            string orderReleaseDate = orderReleaseDateBox.SelectedDate.HasValue ? orderReleaseDateBox.SelectedDate.Value.ToString("yyyy-MM-dd") : "NULL";
            string photoNewName = HandlePhotoFile(photoFilePath, orderNumber, name, orderDate);
            string note = "";
            if(!string.IsNullOrWhiteSpace(noteBox.Text))
            {
                note = noteBox.Text;
            }
            try {
                customerShoeInfo.Name = name;
                customerShoeInfo.Address = address;
                customerShoeInfo.TajNumber = tajNumber;
                customerShoeInfo.Note = note;
                customerShoeInfo.OrderReleaseDate = orderReleaseDate;
                customerShoeInfo.OrderDate = orderDate;
                customerShoeInfo.FileName = photoNewName;
                customerShoeInfo.OrderNumber = orderNumber;
                customerShoeInfo.Files = images.ToList<FileBLOB>();
                if (OrderService.saveNewOrder(customerShoeInfo, newOrder))
                {
                    this.Close();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt az adatok mentése során! Hibaüzenet: {ex.Message}");
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


     
        private void CustomerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCustomer = (Custumer)custumerComboBox.SelectedItem;
            if(selectedCustomer != null && selectedCustomer.Id != -1) 
            {
                customerShoeInfo.Name = selectedCustomer.Name;
                nameBox.Text = selectedCustomer.Name;

                customerShoeInfo.Address = selectedCustomer.Address;
                addressBox.Text = selectedCustomer.Address;

                customerShoeInfo.TajNumber = selectedCustomer.TAJNumber;
                tajNumberBox.Text = selectedCustomer.TAJNumber;

                customerShoeInfo.CustomerId = selectedCustomer.Id;

                
            }
            else 
            {
                nameBox.Text = "";
                addressBox.Text = "";
                tajNumberBox.Text = "";
            }
        }


        private static string GenerateRandomSuffix(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        private string HandlePhotoFile(string photoFilePath, string orderNumber, string name, string date)
        {
            if ((string.IsNullOrEmpty(photoFilePath) || !photoFilePath.Equals("NULL")) && Directory.Exists(photoFilePath))
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string newDirectory = System.IO.Path.Combine(baseDirectory, "images/order");

                if (!Directory.Exists(newDirectory))
                {
                    Directory.CreateDirectory(newDirectory);
                }
                
                string newFileName = "";
                if (newOrder || (customerShoeInfo != null && !customerShoeInfo.FileName.Equals("NULL")))
                {
                    newFileName = $"{orderNumber}_{name}_{date}_{GenerateRandomSuffix(5)}{System.IO.Path.GetExtension(photoFilePath)}";
                }
                else
                {
                    newFileName = customerShoeInfo.FileName;
                }
                string newFilePath = System.IO.Path.Combine(newDirectory, newFileName);

                

                File.Copy(photoFilePath, newFilePath, true);
                return newFileName;
            }
            else return "NULL";
        }


        private void PhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*",
                Title = "Válassz ki egy fényképet"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                photoButton.Content = System.IO.Path.GetFileName(openFileDialog.FileName);
                photoFilePath = openFileDialog.FileName;
                
                foreach (string filename in openFileDialog.FileNames)
                {
                    images.Add(FileService.getFileBLOB(System.IO.Path.GetFileName(openFileDialog.FileName), photoFilePath));
                }
            }
        }

        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImagesListView.SelectedItem is FileBLOB selectedFileBlob)
            {
                BitmapImage bitmapImage = selectedFileBlob.BitmapImage;

                // Fájl elmentése
                string tempPath = Path.Combine(Path.GetTempPath(), selectedFileBlob.Name + ".jpg");
                using (FileStream fileStream = new FileStream(tempPath, FileMode.Create))
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                    encoder.Save(fileStream);
                }

                // Megnyitás az alapértelmezett képnézegetőben
                Process.Start(new ProcessStartInfo(tempPath) { UseShellExecute = true });
            }
        }

        private void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImagesListView.SelectedItem is FileBLOB selectedImage)
            {
                if (FileService.deleteFile(selectedImage))
                {
                    images.Remove(selectedImage);
                }
            }
        }
        private void RenameImage_Click(object sender, RoutedEventArgs e) 
        {
            if (ImagesListView.SelectedItem is FileBLOB selectedImage)
            {
                if (FileService.renameFile(selectedImage))
                {
                    int index = images.IndexOf(selectedImage);
                    if (index != -1)
                    {
                        images[index] = selectedImage;
                    }
                }
            }
        }


        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImagesListView.SelectedItem is FileBLOB selectedFileBlob)
            {
                BitmapImage bitmapImage = selectedFileBlob.BitmapImage;

                // Megnyitás a SaveFileDialog segítségével
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = selectedFileBlob.Name; // Alapértelmezett fájlnév
                saveFileDialog.DefaultExt = ".jpg"; // Alapértelmezett fájlkiterjesztés
                saveFileDialog.Filter = "JPEG képek (.jpg)|*.jpg"; // Szűrők

                // Dialógus megnyitása és fájl mentése, ha a felhasználó megnyomja a 'Mentés' gombot
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                        encoder.Save(fileStream);
                    }
                }
            }
        }





        //private void SaveImage_Click(object sender, RoutedEventArgs e) { }
        private void OnImageSelected(object sender, RoutedEventArgs e) { }
        




    }
}
