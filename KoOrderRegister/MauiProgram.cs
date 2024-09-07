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
using System.Globalization;
using KoOrderRegister.Localization.SupportedLanguage;
using KoOrderRegister.Services;

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

            builder.Services.AddTransient<OrderListPage>();
            builder.Services.AddTransient<OrderListViewModel>();

            builder.Services.AddTransient<OrderDetailsPage>();
            builder.Services.AddTransient<OrderDetailViewModel>();

            builder.Services.AddSingleton<IFileService, FileService>();

            #endregion
            #region Customer Modul

            builder.Services.AddTransient<CustomerListPage>();
            builder.Services.AddTransient<CustomerListViewModel>();

            builder.Services.AddTransient<PersonDetailsPage>();
            builder.Services.AddTransient<PersonDetailsViewModel>();

            builder.Services.AddTransient<ShowCustomerPopUp>();
            builder.Services.AddTransient<PersonDetailPopUp>();

            #endregion

            #region Settings Modul
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddSingleton<SettingsViewModel>();
            #endregion

            #region Update
            #if WINDOWS
                builder.Services.AddSingleton<IAppUpdateService, KoOrderRegister.Platforms.Windows.Service.UpdateService>();
            #else
                 builder.Services.AddSingleton<IAppUpdateService, PlaceHolderAppUpdateService>();
            #endif

            #endregion
#if DEBUG
            builder.Logging.AddDebug();
#endif

            ILanguageSettings languageSettings = LanguageManager.GetCurrentLanguage();
            CultureInfo culture = new CultureInfo(languageSettings.GetCultureName());
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            return builder.Build();
        }
    }
}
