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
using KoOrderRegister.Platforms.Windows.Serivces;
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
            #region Database
            builder.Services.AddTransient<IDatabaseModel, DatabaseModel>();
            #region Database Socket

         /*   builder.Services.AddTransient<RealTimeDatabaseHub>();
                builder.Services.AddTransient<RealTimeDatabaseClient>();
                builder.Services.AddScoped<IDatabaseModel>(provider =>
                new RealTimeDatabaseModel(
                    provider.GetRequiredService<DatabaseModel>(),
                    provider.GetRequiredService<IHubContext<RealTimeDatabaseHub>>(),
                    provider.GetRequiredService<RealTimeDatabaseClient>()
                ));

                builder.Services.AddSignalRCore();*/
#endregion
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
                builder.Services.AddSingleton<IVersionService, VersionService>();
#elif ANDROID
            builder.Services.AddSingleton<IVersionService, KoOrderRegister.Platforms.Android.Services.VersionService>();
            builder.Services.AddSingleton<KoOrderRegister.Platforms.Android.Services.IPermissions, KoOrderRegister.Platforms.Android.Services.Permissions>();

#else
            builder.Services.AddSingleton<IAppUpdateService, AppUpdateService>();
#endif

            #endregion

            #region File Modul
            builder.Services.AddTransient<FilePropertiesPopup>();
            builder.Services.AddTransient<FilePropertiesViewModel>();
            #endregion

            #region Export Modul
            builder.Services.AddTransient<IExcelExportService, ExcelExportService>();
            //excel

/* Unmerged change from project 'KoOrderRegister (net8.0-windows10.0.19041.0)'
Before:
            builder.Services.AddTransient<Modules.Export.Excel.Pages.ExcelExportersPage>();
            builder.Services.AddSingleton<Modules.Export.Excel.ViewModel.ExportersViewModel>();
After:
            builder.Services.AddTransient<ExcelExportersPage>();
            builder.Services.AddSingleton<Modules.Export.Excel.ViewModel.ExportersViewModel>();
*/
            builder.Services.AddTransient<Modules.Export.Exporters.Excel.View.Pages.ExcelExportersPage>();

/* Unmerged change from project 'KoOrderRegister (net8.0-windows10.0.19041.0)'
Before:
            builder.Services.AddSingleton<Modules.Export.Excel.ViewModel.ExportersViewModel>();
            //pdf
After:
            builder.Services.AddSingleton<ExportersViewModel>();
            //pdf
*/
            builder.Services.AddSingleton<Modules.Export.Exporters.Excel.View.ViewModel.ExportersViewModel>();
            //pdf

/* Unmerged change from project 'KoOrderRegister (net8.0-windows10.0.19041.0)'
Before:
            builder.Services.AddTransient<Modules.Export.Pdf.Pages.PdfExportersPage>();
After:
            builder.Services.AddTransient<PdfExportersPage>();
*/
            builder.Services.AddTransient<Modules.Export.Exporters.Pdf.View.Pages.PdfExportersPage>();

/* Unmerged change from project 'KoOrderRegister (net8.0-windows10.0.19041.0)'
Before:
            builder.Services.AddSingleton<Modules.Export.Pdf.ViewModel.ExportersViewModel>();
            //html
After:
            builder.Services.AddSingleton<ExportersViewModel>();
            //html
*/
            builder.Services.AddSingleton<Modules.Export.Exporters.Pdf.View.ViewModel.ExportersViewModel>();
            //html

/* Unmerged change from project 'KoOrderRegister (net8.0-windows10.0.19041.0)'
Before:
            builder.Services.AddTransient<Modules.Export.Html.Pages.HtmlExportersPage>();
After:
            builder.Services.AddTransient<HtmlExportersPage>();
*/
            builder.Services.AddTransient<Modules.Export.Exporters.Html.View.Pages.HtmlExportersPage>();

/* Unmerged change from project 'KoOrderRegister (net8.0-windows10.0.19041.0)'
Before:
            builder.Services.AddSingleton<Modules.Export.Html.ViewModel.ExportersViewModel>();
            #endregion
After:
            builder.Services.AddSingleton<ExportersViewModel>();
            #endregion
*/
            builder.Services.AddSingleton<Modules.Export.Exporters.Html.View.ViewModel.ExportersViewModel>();
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
