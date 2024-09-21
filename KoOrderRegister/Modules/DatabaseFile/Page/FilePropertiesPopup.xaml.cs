using KoOrderRegister.Localization;
using KoOrderRegister.Modules.Database.Models;
using KoOrderRegister.Modules.DatabaseFile.ViewModel;
using Mopups.Pages;

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
        if(_viewModel.File == null)
        {
            Error();
        }
        else
        {
            base.OnAppearing();
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