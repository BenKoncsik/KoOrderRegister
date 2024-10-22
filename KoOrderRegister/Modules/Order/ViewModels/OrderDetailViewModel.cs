using CommunityToolkit.Maui.Storage;
using KoOrderRegister.Localization;
using KoOrderRegister.Modules.Database.Models;
using KoOrderRegister.Modules.Database.Services;
using KoOrderRegister.Modules.DatabaseFile.Page;
using KoOrderRegister.Modules.Order.List.Services;
using KoOrderRegister.Services;
using KoOrderRegister.Utility;
using KoOrderRegister.ViewModel;
using Microsoft.Maui.Storage;
using Mopups.Services;
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
    public class OrderDetailViewModel : BaseViewModel
    {
        #region DI
        private readonly IDatabaseModel _database;
        private readonly IFileService _fileService;
        private readonly FilePropertiesPopup _filePropertiesPopup;
        #endregion
        #region Binding varrible
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

        private bool _isLoadingFiles = false;
        public bool IsLoadingFiles
        {
            get => _isLoadingFiles;
            set
            {
                _isLoadingFiles = value;
                OnPropertyChanged(nameof(IsLoadingFiles));
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
        #endregion
        public ObservableCollection<FileModel> Files { get; set; } = new ObservableCollection<FileModel>();
        private CancellationToken cancellationToken = new CancellationToken();
        #region Commands
        public ICommand ReturnCommand => new Command(Return);
        public ICommand SaveCommand => new Command(SaveOrder);
        public ICommand DeleteCommand => new Command(DeleteOrder);
        public ICommand SelectedFilesCommand => new Command(SelectedFiles);
        public Command<FileModel> RemoveFileCommand => new Command<FileModel>(RemoveFile);
        public Command<FileModel> OpenFileCommand => new Command<FileModel>(OpenFile);
        public Command<FileModel> SaveFileCommand => new Command<FileModel>(SaveFile);
        public Command<FileModel> EditFileCommand => new Command<FileModel>(EditFile);
        public ICommand UpdateFilesCommand => new Command(UpdateFiles);
        #endregion
        public OrderDetailViewModel(IDatabaseModel database, IFileService fileService, FilePropertiesPopup filePropertiesPopup, IAppUpdateService updateService, ILocalNotificationService notificationService) : base(updateService, notificationService)
        {
            _database = database;
            _fileService = fileService;
            _filePropertiesPopup = filePropertiesPopup;
        }

        public void EditOrder(OrderModel order)
        {
            SelectedItem = order.Customer;
            Order = order;
            SelectedEndDate = Order.EndDate;
            SelectedEndTime = Order.EndDate.TimeOfDay;
            SelectedStartDate = Order.StartDate;
            SelectedStartTime = Order.StartDate.TimeOfDay;
            Files = new ObservableCollection<FileModel>(Order.Files);
            
#if DEBUG
            Debug.WriteLine("Start date: " + SelectedStartDate.ToString("yyyy-MM-dd"));
            Debug.WriteLine("Start time: " + SelectedStartTime.ToString(@"hh\:mm"));
            Debug.WriteLine("End date: " + SelectedEndDate.ToString("yyyy-MM-dd"));
            Debug.WriteLine("End time: " + SelectedEndTime.ToString(@"hh\:mm"));
            Debug.WriteLine("Files: " + Order.Files.Count());
#endif
        }
        public async void SaveOrder()
        {
            using (new LowPriorityTaskManager())
            {
                IsRefreshing = true;
                if (Files != null)
                {
                    List<Task> tasks = new List<Task>();
                    foreach (FileModel file in Files)
                    {
                        if (file.IsDatabaseContent)
                        {
                            continue;
                        }
                        tasks.Add(ThreadManager.Run(async () =>
                        {
                            if (!file.IsDatabaseContent && file.FileResult != null)
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
                    IsRefreshing = false;
                    await Application.Current.MainPage.DisplayAlert(AppRes.Save, AppRes.SuccessToSave + " " + Order.OrderNumber, AppRes.Ok);
                }
                else
                {
                    IsRefreshing = false;
                    await Application.Current.MainPage.DisplayAlert(AppRes.Save, AppRes.FailedToSave + " " + Order.OrderNumber, AppRes.Ok);
                }
            }
        }

        public async void DeleteOrder()
        {
            if (await Application.Current.MainPage.DisplayAlert(AppRes.Delete, AppRes.AreYouSureYouWantToDelete + " " + Order.OrderNumber, AppRes.No, AppRes.Yes))
            {
                IsRefreshing = true;
                if (await _database.DeleteOrder(Order.Guid) > 0)
                {
                    IsRefreshing = false;
                    Return();
                }
                else
                {
                    IsRefreshing = false;
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
            using (new LowPriorityTaskManager())
            {
                if (Customers != null)
                {
                    Customers.Clear();
                }

                await foreach(var customer in  _database.GetAllCustomersAsStream(cancellationToken))
                {
                    Customers.Add(customer);
                }
                if (Customers.Count > 0)
                {
                    SelectedItem = Customers.First();
                }
                if (IsEdit)
                {
                    await ThreadManager.Run(async () =>
                    {
                        var orderFiles = await _database.GetFilesByOrderIdWithOutContent(Order.Guid);
                        Order.Files = orderFiles;

                        var filesToAdd = orderFiles
                            .Where(of => !Files.Any(f => f.Guid.Equals(of.Guid)))
                            .ToList();

                        var filesToRemove = Files
                            .Where(f => !orderFiles.Any(of => of.Guid.Equals(f.Guid)))
                            .ToList();

                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            foreach (var file in filesToAdd)
                            {
                                Files.Add(file);
                            }

                            foreach (var file in filesToRemove)
                            {
                                Files.Remove(file);
                            }
                        });
                    });
                }
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
            IsRefreshing = true;
            if (file.FileResult == null)
            {
              await _database.DeleteFile(file.Guid);
            }
            Files.Remove(file);
            IsRefreshing = false;
        }

        public async void OpenFile(FileModel file)
        {
            IsRefreshing = true;
            file = await _database.GetFileById(file.Guid);
            
            if (file.Content == null)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(AppRes.Open, AppRes.FailedToOpen + " " + file.Name, AppRes.Ok);
                return;
            }
            var filePath = await _fileService.SaveFileToTmp(file);
            IsRefreshing = false;
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }

        public async void SaveFile(FileModel file)
        {
            IsRefreshing = true;
            file = await _database.GetFileById(file.Guid);
            if (file.Content == null)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(AppRes.Save, AppRes.FailedToSave + " " + file.Name, AppRes.Ok);
                return;
            }
            try
            {
                IsRefreshing = false;
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
                Debug.WriteLine($"Cancel folder picker! | Ex msg: {ex.Message}");
            }
        }

        public async void UpdateFiles()
        {
            IsLoadingFiles = true;
            List<FileModel> files = await _database.GetFilesByOrderIdWithOutContent(Order.Guid);
            if(files == null)
            {
                IsLoadingFiles = false;
                return;
            }
            if (Order.Files != null)
            {
                Order.Files.Clear();
                Files.Clear();
            }
            foreach (FileModel file in files)
            {
                Order.Files.Add(file);
                Files.Add(file);
            }

            IsLoadingFiles = false;
        }

        public async void EditFile(FileModel file)
        {
            _filePropertiesPopup.EditFile(file);
            await MopupService.Instance.PushAsync(_filePropertiesPopup);
        }




    }
}
