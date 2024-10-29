using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore;
using System.Diagnostics;
using System.Net.NetworkInformation;
using KORConnect.SinalR;
using KORCore.Modules.Database.Services;
using KORConnect.Controllers;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;


namespace KORConnect;
public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
    }

    private static IHost _webHost;
    

    public static int CreateAndRunWebHost(string[] args, int? port)
    {
        string url = string.Empty;

        if (port.HasValue)
        {
            url = $"http://{GetLocalIPAddress()}:{port.Value}";
        }
        else
        {
            int freePort = GetFreePort();
            url = $"http://{GetLocalIPAddress()}:{freePort}";
            port = freePort;
        }
        Debug.WriteLine($"URL: {url}");

        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddScoped<IDatabaseModel, DatabaseModel>();
        builder.Services.AddSignalR();
        builder.Services.AddControllers().AddNewtonsoftJson();
        //builder.Services.AddRazorPages();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddControllers().AddApplicationPart(typeof(CustomerController).Assembly);
        builder.Services.AddControllers().AddApplicationPart(typeof(DatabaseContreller).Assembly);
        builder.Services.AddControllers().AddApplicationPart(typeof(FileController).Assembly);
        builder.Services.AddControllers().AddApplicationPart(typeof(OrderController).Assembly);

        builder.WebHost.UseUrls(url);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAuthorization();

        app.MapControllers();
        app.MapGet("/helloworld", () => "Hello World!");
        app.MapGet("/", () => "King of Koncsik");
        if (app != null)
        {
            
            _webHost = app as IHost;
            _webHost.Start();
            return port.Value;
            
        }
        else
        {
            throw new Exception("Failed to create web host");
        }

    }

    public static async Task StopWebHost()
    {
        if (_webHost != null)
        {
            await _webHost.StopAsync();
            _webHost.Dispose();
        }
    }

    private static int GetFreePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    public static string GetLocalIPAddress()
    {
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet && ni.OperationalStatus == OperationalStatus.Up)
            {
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.Address.ToString();
                    }
                }
            }
        }
        throw new Exception("No active Ethernet network adapters with an IPv4 address in the system!");
    }

}
