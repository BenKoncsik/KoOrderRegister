using System.Net.Sockets;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace KORConnect;
public class Program
{
    private static IHost _webHost;
    public static void Main(string[] args)
    {
        /*var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapGet("/helloworld", () => "Hello World!");

        app.Run();*/
        //CreateAndRunWebHost(args, null);
    }

    public static int CreateAndRunWebHost(string[] args, int? port)
    {
        var builder = WebApplication.CreateBuilder(args);
        string url = string.Empty;

        if (port.HasValue)
        {
            url = $"http://{GetLocalIPAddress}:{port.Value}";
        }
        else
        {
            int freePort = GetFreePort();
            url = $"http://{GetLocalIPAddress}:{freePort}";
            port = freePort;
        }
        Debug.WriteLine($"URL: {url}");
        builder.WebHost.UseUrls(url);

        WebApplication app = builder.Build();

        app.MapGet("/helloworld", () => "Hello World!");

        if(app != null)
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