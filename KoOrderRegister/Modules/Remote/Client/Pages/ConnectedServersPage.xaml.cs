using KoOrderRegister.Modules.Remote.Client.Behavior;
using KoOrderRegister.Modules.Remote.Client.ViewModel;

namespace KoOrderRegister.Modules.Remote.Client.Pages;

public partial class ConnectedServersPage : ContentPage
{
	private readonly ConnectedServersViewModel _viewModel;
	private readonly ConnectionHighlightBehavior _connectionHighlightBehavior;
	private readonly ConnectionMenuBehavior _connectionMenuBehavior;
    public ConnectedServersPage(ConnectedServersViewModel connectedDevicesViewModel, ConnectionHighlightBehavior connectionHighlightBehavior, ConnectionMenuBehavior connectionMenuBehavior)
	{
		_viewModel = connectedDevicesViewModel;
        BindingContext = _viewModel;
        InitializeComponent();
        _connectionHighlightBehavior = connectionHighlightBehavior;
        _connectionMenuBehavior = connectionMenuBehavior;
        connectionListView.Behaviors.Add(_connectionHighlightBehavior);
       
        
    }

    protected void OnTextChanged(object sender, EventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;
        _viewModel.Search(searchBar.Text);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
}