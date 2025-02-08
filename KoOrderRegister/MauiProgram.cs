using CommunityToolkit.Maui;
using KoOrderRegister.Modules.Customer.Pages;
using KoOrderRegister.Modules.Customer.ViewModels;
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
using KoOrderRegister.Modules.Remote.Server.Service;
using KoOrderRegister.Modules.Remote.Server.Pages;
using KoOrderRegister.Modules.Remote.ViewModel;
using Camera.MAUI;
using KORCore.Modules.Database.Services;
using KoOrderRegister.Modules.Remote.Client.ViewModel;
using KoOrderRegister.Modules.Remote.Client.Pages;
using KoOrderRegister.Modules.Remote.Client.Service;
using KORCore.Modules.Database.Factory;
using KoOrderRegister.Modules.Remote.Client.Behavior;

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
                .UseMauiCameraView()
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
            builder.Services.AddTransient<ILocalDatabase, LocalDatabaseModel>();
            builder.Services.AddHttpClient("kor_connection_client", client =>
            {

            });
            builder.Services.AddTransient<IRemoteDatabase, RemoteDatabaseModel>();
            builder.Services.AddSingleton<IDatabaseModelFactory, DatabaseModelFactory>();
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

            builder.Services.AddTransient<IFileService, FileService>();

            #endregion

            #region Customer Modul

            builder.Services.AddTransient<CustomerListPage>();
            builder.Services.AddTransient<CustomerListViewModel>();

            builder.Services.AddTransient<PersonDetailsPage>();
            builder.Services.AddTransient<PersonDetailsViewModel>();


            #endregion

            #region Settings Modul
            builder.Services.AddSingleton<LocalDatabaseModel>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<SettingsViewModel>();
            #endregion

            #region Update Modul
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
            builder.Services.AddSingleton<IExcelExportService, ExcelExportService>();
            //excel
            builder.Services.AddTransient<Modules.Export.Excel.Pages.ExcelExportersPage>();
            builder.Services.AddTransient<Modules.Export.Exporters.Excel.View.ViewModel.ExportersViewModel>();
            //pdf
            builder.Services.AddTransient<Modules.Export.Pdf.Pages.PdfExportersPage>();
            builder.Services.AddTransient< Modules.Export.Exporters.Pdf.View.ViewModel.ExportersViewModel >();
            //html
            builder.Services.AddTransient<Modules.Export.Html.Pages.HtmlExportersPage>();
            builder.Services.AddTransient< Modules.Export.Exporters.Html.View.ViewModel.ExportersViewModel >();
            #endregion

            #region Beta function Modul
            builder.Services.AddTransient<BetaFunctionsPages>();
            builder.Services.AddTransient<BetaFunctionsViewModel>();
            #endregion

            #region Language settings Modul
            ILanguageSettings languageSettings = LanguageManager.GetCurrentLanguage();
            languageSettings.SetRegioSpecification();
            CultureInfo culture = new CultureInfo(languageSettings.GetCultureName());
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            #endregion

            #region Notification Modul
            builder.Services.AddSingleton<ILocalNotificationService, LocalNotificationService>();
#if WINDOWS
            builder.Services.AddSingleton<KoOrderRegister.Modules.Windows.Notification.Pages.NotificationPages>();
            builder.Services.AddSingleton<KoOrderRegister.Modules.Windows.Notification.ViewModel.NotificationViewModel>();            
#endif
            #endregion 

            #region Remote Modul
            #region Server
#if WINDOWS
           builder.Services.AddSingleton<IRemoteServerService, RemoteServerService>();
           builder.Services.AddTransient<RemoteServerViewModel>();
           builder.Services.AddTransient<RemoteServerPage>();
#endif
            #endregion
            #region Client
            builder.Services.AddSingleton<IRemoteClientService, RemoteClientService>();
            builder.Services.AddTransient<ClientConnectionViewModel>();
            builder.Services.AddTransient<ClientConnectionPage>();

            builder.Services.AddTransient<ConnectedServersViewModel>();
            builder.Services.AddTransient<ConnectedServersPage>();

            builder.Services.AddTransient<ConnectionHighlightBehavior>();
            builder.Services.AddTransient<ConnectionMenuBehavior>();


            #endregion
            #endregion

#if DEBUG
            builder.Logging.AddDebug();
#endif
            var app = builder.Build();
#if WINDOWS
            var remoteService = app.Services.GetRequiredService<IRemoteServerService>();
            remoteService.Init();
#endif
            return app;
        }
    }
}
