using CommunityToolkit.Maui;
using KoOrderRegister.Modules.Customer.Pages;
using KoOrderRegister.Modules.Customer.ViewModels;
using KoOrderRegister.Modules.Database.Services;
using KoOrderRegister.Modules.Order.Pages;
using KoOrderRegister.Modules.Order.List.Services;
using KoOrderRegister.Modules.Order.List.ViewModels;
using KoOrderRegister.Modules.Order.ViewModels;
using KoOrderRegister.Utility;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using KoOrderRegister.Modules.Settings.Pages;
using KoOrderRegister.Modules.Settings.ViewModels;

namespace KoOrderRegister
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureMopups()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddTransient<IDatabaseModel, DatabaseModel>();

            #region Order Modul

            builder.Services.AddSingleton<OrderListPage>();
            builder.Services.AddTransient<OrderListViewModel>();

            builder.Services.AddSingleton<OrderDetailsPage>();
            builder.Services.AddTransient<OrderDetailViewModel>();

            builder.Services.AddSingleton<IFileService, FileService>();

            #endregion
            #region Customer Modul

            builder.Services.AddSingleton<CustomerListPage>();
            builder.Services.AddTransient<CustomerListViewModel>();

            builder.Services.AddSingleton<PersonDetailsPage>();
            builder.Services.AddTransient<PersonDetailsViewModel>();

            builder.Services.AddSingleton<ShowCustomerPopUp>();
            builder.Services.AddTransient<PersonDetailPopUp>();

            #endregion

            #region Settings Modul
            builder.Services.AddSingleton<SettingsPage>();
            builder.Services.AddTransient<SettingsViewModel>();
            #endregion

#if DEBUG
            builder.Logging.AddDebug();
#endif
            

            return builder.Build();
        }
    }
}
