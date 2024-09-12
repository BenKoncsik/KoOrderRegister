using CommunityToolkit.Maui.Storage;
using KoOrderRegister.Localization;
using KoOrderRegister.Modules.Database.Models;
using KoOrderRegister.Modules.Database.Services;
using KoOrderRegister.Modules.Order.List.Services;
using KoOrderRegister.Utility;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KoOrderRegister.Modules.Order.List.ViewModels
{
    public class OrderDetailViewModel : INotifyPropertyChanged
    {
        private readonly IDatabaseModel _database;
        private readonly IFileService _fileService;
        private OrderModel _order = new OrderModel();

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
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

        private DateTime _SelectedStartDate = DateTime.Now;
        public DateTime SelectedStartDate 
        {
            get => _SelectedStartDate;
            set
            {
                if (!value.Equals(_SelectedStartDate))
                {
                    _SelectedStartDate = value;
                    OnPropertyChanged(nameof(SelectedStartDate));
                }
            }
        }
        private TimeSpan _SelectedStartTime = DateTime.Now.TimeOfDay;
        public TimeSpan SelectedStartTime 
        {
            get => _SelectedStartTime;
            set
            {
                if (!value.Equals(_SelectedStartTime))
                {
                    _SelectedStartTime = value;
                    OnPropertyChanged(nameof(SelectedStartTime));
                }
            }
        }
        private DateTime _SelectedEndDate = DateTime.Now;
        public DateTime SelectedEndDate 
        {
            get => _SelectedEndDate;
            set
            {
                if (!value.Equals(_SelectedEndDate))
                {
                    _SelectedEndDate = value;
                    OnPropertyChanged(nameof(SelectedEndDate));
                }
            }
        }
        private TimeSpan _SelectedEndTime = DateTime.Now.TimeOfDay;
        public TimeSpan SelectedEndTime
        {
            get => _SelectedEndTime;
            set
            {
                if (!value.Equals(_SelectedEndTime))
                {
                    _SelectedEndTime = value;
                    OnPropertyChanged(nameof(SelectedEndTime));
                }
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
        public Command<FileModel> OpenFileCommand => new Command<FileModel>(OpenFile);
        public Command<FileModel> SaveFileCommand => new Command<FileModel>(SaveFile);
        #endregion
        public OrderDetailViewModel(IDatabaseModel database, IFileService fileService)
        {
            _database = database;
            _fileService = fileService;
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
            SelectedEndDate = Order.EndDate;
            SelectedEndTime = Order.EndDate.TimeOfDay;
            SelectedStartDate = Order.StartDate;
            SelectedStartTime = Order.StartDate.TimeOfDay;
#if DEBUG
            Console.WriteLine("Start date: " + SelectedStartDate.ToString("yyyy-MM-dd"));
            Console.WriteLine("Start time: " + SelectedStartTime.ToString(@"hh\:mm"));
            Console.WriteLine("End date: " + SelectedEndDate.ToString("yyyy-MM-dd"));
            Console.WriteLine("End time: " + SelectedEndTime.ToString(@"hh\:mm"));
#endif
        }
        public async void SaveOrder()
        {
            IsLoading = true;
            if (Files != null)
            {
                List<Task> tasks = new List<Task>();
                foreach (FileModel file in Files)
                {
                    tasks.Add(ThreadManager.Run(async () =>
                    {
                        if (file.Content == null && file.FileResult != null)
                        {
                            List<byte> contentList = new List<byte>();
                            using (var stream = await file.FileResult.OpenReadAsync())
                            {
                                byte[] buffer = new byte[1048576]; 
                                int bytesRead;
                                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    contentList.AddRange(buffer.Take(bytesRead)); 
                                }
                            }
                            file.Content = contentList.ToArray();
                            file.HashCode = await _fileService.CalculateHashAsync(file.Content);
                        }
                        await _database.CreateFile(file);
                    }));
                }

                await Task.WhenAll(tasks);
            }
            Order.StartDate = _SelectedStartDate.Date + _SelectedStartTime;
            Order.EndDate = _SelectedEndDate.Date + _SelectedEndTime;
            if (await _database.CreateOrder(Order) > 0)
            {
                IsLoading = false;
                await Application.Current.MainPage.DisplayAlert(AppRes.Save, AppRes.SuccessToSave + " " + Order.OrderNumber, AppRes.Ok);
            }
            else
            {
                IsLoading = false;
                await Application.Current.MainPage.DisplayAlert(AppRes.Save, AppRes.FailedToSave + " " + Order.OrderNumber, AppRes.Ok);
            }
        }

        public async void DeleteOrder()
        {
            if (await Application.Current.MainPage.DisplayAlert(AppRes.Delete, AppRes.AreYouSureYouWantToDelete + " " + Order.OrderNumber, AppRes.No, AppRes.Yes))
            {
                IsLoading = true;
                if (await _database.DeleteOrder(Order.Guid) > 0)
                {
                    IsLoading = false;
                    Return();
                }
                else
                {
                    IsLoading = false;
                    await Application.Current.MainPage.DisplayAlert(AppRes.Delete, AppRes.FailedToDelete + " " + Order.OrderNumber, AppRes.Ok);
                }
            }
        }

        public async void Return()
        {
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
            if(Customers.Count > 0)
            {
                SelectedItem = Customers.First();
            }
            
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
                        FilePath = fileResult.FullPath,
                        ContentType = fileResult.ContentType,
                        FileResult = fileResult
                    });
                }
            }
        }

        public async void RemoveFile(FileModel file)
        {
            IsLoading = true;
            if (file.Content != null)
            {
                await _database.DeleteFile(file.Guid);
            }
            Files.Remove(file);
            IsLoading = false;
        }

        public async void OpenFile(FileModel file)
        {
            IsLoading = true;
            file = await _database.GetFileById(file.Guid);
            
            if (file.Content == null)
            {
                IsLoading = false;
                await Application.Current.MainPage.DisplayAlert(AppRes.Open, AppRes.FailedToOpen + " " + file.Name, AppRes.Ok);
                return;
            }
            var filePath = await _fileService.SaveFileToTmp(file);
            IsLoading = false;
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }

        public async void SaveFile(FileModel file)
        {
            IsLoading = true;
            file = await _database.GetFileById(file.Guid);
            if (file.Content == null)
            {
                IsLoading = false;
                await Application.Current.MainPage.DisplayAlert(AppRes.Save, AppRes.FailedToSave + " " + file.Name, AppRes.Ok);
                return;
            }
            try
            {
                IsLoading = false;
                if (await _fileService.SaveFileToLocal(file))
                {
                    await Application.Current.MainPage.DisplayAlert(AppRes.Save, AppRes.SuccessToSave + " " + file.Name, AppRes.Ok);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(AppRes.Save, AppRes.FailedToSave + " " + file.Name, AppRes.Ok);
                }
            }
            catch (FileSaveException ex)
            {
                Console.WriteLine($"Cancel folder picker! | Ex msg: {ex.Message}");
            }


        }




    }
}
