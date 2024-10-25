using CommunityToolkit.Maui;
using KoOrderRegister.Modules.Customer.Pages;
using KoOrderRegister.Modules.Customer.ViewModels;
using KoOrderRegister.Modules.Database.Services;
using KoOrderRegister.Modules.Order.Pages;
using KoOrderRegister.Modules.Order.ViewModels;
using KoOrderRegister.Utility;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using KoOrderRegister.Modules.Settings.Pages;
using KoOrderRegister.Modules.Settings.ViewModels;
using System.Globalization;
using KoOrderRegister.Localization.SupportedLanguage;
using KoOrderRegister.Services;
using KoOrderRegister.Modules.DatabaseFile.Page;
using KoOrderRegister.Modules.DatabaseFile.ViewModel;
using KoOrderRegister.Modules.BetaFunctions.Pages;
using KoOrderRegister.Modules.BetaFunctions.ViewModels;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using KoOrderRegister.ViewModel;
using KoOrderRegister.Modules.Order.Services;
using KoOrderRegister.Modules.Export.Exporters.Excel.Services;

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
                .UseLocalNotification(config =>
                {
                    config.AddAndroid(android =>
                    {
                        android.AddChannel(new NotificationChannelRequest
                        {
                            Id = $"kor_general",
                            Name = "General",
                            Description = "General",
                        });
                        android.AddChannel(new NotificationChannelRequest
                        {
                            Id = $"kor_special",
                            Name = "Special",
                            Description = "Special",
                        });
                    });
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            #region database
            builder.Services.AddTransient<IDatabaseModel, DatabaseModel>();
            #endregion

            #region AppShell
            /*builder.Services.AddSingleton<AppShellViewModel>();
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<App>();*/
            #endregion

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


            #endregion

            #region Settings Modul
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddSingleton<SettingsViewModel>();
            #endregion

            #region Update
            builder.Services.AddHttpClient("GitHubClient", client =>
            {

            });
            builder.Services.AddSingleton<IAppUpdateService, AppUpdateService>();
#if WINDOWS
                builder.Services.AddSingleton<IVersionService, Platforms.Windows.Serivces.VersionService>();
#elif ANDROID
            builder.Services.AddSingleton<IVersionService, Platforms.Android.Services.VersionService>();
            builder.Services.AddSingleton<Platforms.Android.Services.IPermissions, KoOrderRegister.Platforms.Android.Services.Permissions>();
#endif

            #endregion

            #region File Modul
            builder.Services.AddTransient<FilePropertiesPopup>();
            builder.Services.AddTransient<FilePropertiesViewModel>();
            #endregion

            #region Export Modul
            builder.Services.AddTransient<IExcelExportService, ExcelExportService>();
            //excel
            builder.Services.AddTransient<Modules.Export.Excel.Pages.ExcelExportersPage>();
            builder.Services.AddSingleton<Modules.Export.Exporters.Excel.View.ViewModel.ExportersViewModel>();
            //pdf
            builder.Services.AddTransient<Modules.Export.Pdf.Pages.PdfExportersPage>();
            builder.Services.AddSingleton< Modules.Export.Exporters.Pdf.View.ViewModel.ExportersViewModel >();
            //html
            builder.Services.AddTransient<Modules.Export.Html.Pages.HtmlExportersPage>();
            builder.Services.AddSingleton< Modules.Export.Exporters.Html.View.ViewModel.ExportersViewModel >();
            #endregion

            #region Beta function modul
            builder.Services.AddTransient<BetaFunctionsPages>();
            builder.Services.AddSingleton<BetaFunctionsViewModel>();
            #endregion

            #region Language settings
            ILanguageSettings languageSettings = LanguageManager.GetCurrentLanguage();
            languageSettings.SetRegioSpecification();
            CultureInfo culture = new CultureInfo(languageSettings.GetCultureName());
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            #endregion

            #region Notification
            builder.Services.AddSingleton<ILocalNotificationService, LocalNotificationService>();
#if WINDOWS
            builder.Services.AddSingleton<KoOrderRegister.Modules.Windows.Notification.Pages.NotificationPages>();
            builder.Services.AddSingleton<KoOrderRegister.Modules.Windows.Notification.ViewModel.NotificationViewModel>();            
#endif
            #endregion

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
