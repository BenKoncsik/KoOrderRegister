using KoOrderRegister.Localization;
using KoOrderRegister.Modules.Database.Models;
using KoOrderRegister.Modules.Database.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KoOrderRegister.Modules.Order.List.ViewModels
{
    public class OrderDetailViewModel : INotifyPropertyChanged
    {
        private static int MAX_DEGREE_OF_PARALLELISM = Environment.ProcessorCount;
        private static SemaphoreSlim SEMAPHORE => new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM);
        private readonly IDatabaseModel _database;
        private OrderModel _order = new OrderModel();
        public OrderModel Order
        {
            get => _order;
            set
            {
                _order = value;
                OnPropertyChanged(nameof(Order));
            }
        }

        private bool _isEdit = false;
        public bool IsEdit
        {
            get => _isEdit;
            set
            {
                _isEdit = value;
                OnPropertyChanged(nameof(IsEdit));
            }
        }

        public ObservableCollection<CustomerModel> Customers {get; set;} = new ObservableCollection<CustomerModel>();
        private CustomerModel _selectedItem;
        public CustomerModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                    Order.CustomerId = value?.Id ?? "";
                    Order.Customer = value;
                    OnPropertyChanged(nameof(Customer));
                }
            }
        }

        public ObservableCollection<FileModel> Files { get; set; } = new ObservableCollection<FileModel>();
        #region Commands
        public ICommand ReturnCommand => new Command(Return);
        public ICommand SaveCommand => new Command(SaveOrder);
        public ICommand DeleteCommand => new Command(DeleteOrder);
        public ICommand SelectedFilesCommand => new Command(SelectedFiles);
        public Command<FileModel> RemoveFileCommand => new Command<FileModel>(RemoveFile);
        #endregion
        public OrderDetailViewModel(IDatabaseModel database)
        {
            _database = database;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void EditOrder(OrderModel order)
        {
            SelectedItem = order.Customer;
            Order = order;
        }
        public async void SaveOrder()
        {
            if (Files != null)
            {
                List<Task> tasks = new List<Task>();
                foreach (FileModel file in Files)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        await SEMAPHORE.WaitAsync();
                        try
                        {
                            if(file.Content == null)
                            {
                                using (var stream = new FileStream(file.FilePath, FileMode.Open, FileAccess.Read))
                                {
                                    byte[] content = new byte[stream.Length];
                                    await stream.ReadAsync(content, 0, content.Length);
                                    file.Content = content;
                                }
                            }
                            await _database.CreateFile(file);
                        }
                        finally
                        {
                            SEMAPHORE.Release();
                        }
                    }));
                }
                await Task.WhenAll(tasks);
            }
            if (await _database.CreateOrder(Order) > 0)
            {
                await Application.Current.MainPage.DisplayAlert(AppRes.Save, AppRes.SuccessToSave + " " + Order.OrderNumber, AppRes.Ok);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(AppRes.Save, AppRes.FailedToSave + " " + Order.OrderNumber, AppRes.Ok);
            }
        }

        public async void DeleteOrder()
        {
            if (await Application.Current.MainPage.DisplayAlert(AppRes.Delete, AppRes.AreYouSureYouWantToDelete + " " + Order.OrderNumber, AppRes.No, AppRes.Yes))
            {
                if (await _database.DeleteOrder(Order.Guid) > 0)
                {
                    Return();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(AppRes.Delete, AppRes.FailedToDelete + " " + Order.OrderNumber, AppRes.Ok);
                }
            }
            
        }

        public async void Return()
        {
            Order = new OrderModel();
            IsEdit = false;
            if(Files != null)
            {
                Files.Clear();
            }
            App.Current.MainPage.Navigation.PopAsync();
        }

        public async void Update()
        {
            if (Customers != null)
            {
                Customers.Clear();
            }
          
            foreach (var customer in await _database.GetAllCustomers())
            {
                Customers.Add(customer);
            }
            SelectedItem = Customers.First();
        }

        public async void SelectedFiles()
        {
            IEnumerable<FileResult> result = await FilePicker.PickMultipleAsync(new PickOptions
            {
                PickerTitle = AppRes.SelectFiles
            });

            if (result != null)
            {
                foreach (var fileResult in result)
                {
                    Files.Add(new FileModel
                    {
                        OrderId = Order.Id,
                        Name = fileResult.FileName,
                        FilePath = fileResult.FullPath
                    });
                }
            }
        }

        public async void RemoveFile(FileModel file)
        {
            if(file.Content != null)
            {
                await _database.DeleteFile(file.Guid);
            }
            Files.Remove(file);

        }
    }
}
