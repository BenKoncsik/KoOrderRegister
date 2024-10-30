using KoOrderRegister.Localization;
using KoOrderRegister.Modules.DatabaseFile.ViewModel;
using KoOrderRegister.Utility;
using Mopups.Pages;
using KORCore.Modules.Database.Models;
using KORCore.Utility;

namespace KoOrderRegister.Modules.DatabaseFile.Page;

public partial class FilePropertiesPopup : PopupPage
{
    private readonly FilePropertiesViewModel _viewModel;
    public FilePropertiesPopup(FilePropertiesViewModel fileProperties)
    {
        _viewModel = fileProperties;
        InitializeComponent();
        BindingContext = _viewModel;
    }
    public void EditFile(FileModel file)
    {
        _viewModel.File = file;
    }
    protected override void OnAppearing()
    {
        using (new LowPriorityTaskManager())
        {
            if (_viewModel.File == null)
            {
                Error();
            }
            else
            {
                base.OnAppearing();
                _viewModel.OnAppearing();
            }
        }
        
    }

    private async void Error()
    {
        await Application.Current.MainPage.DisplayAlert(AppRes.Open, AppRes.SorrySomethingWrong, AppRes.Ok);
        _viewModel.Return();
    }
    protected override void OnDisappearing()
    {
        _viewModel.IsAdvancedDetails = false;
        base.OnDisappearing();
    }
}