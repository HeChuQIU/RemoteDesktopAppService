using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RemoteDesktopAppService;

namespace RemoteDesktopAppServiceWpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    [STAThread]
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<MainWindow>();
        builder.Services.AddSignalR();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                corsPolicyBuilder =>
                {
                    corsPolicyBuilder.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        using var host = builder.Build();

        host.UseCors();

        host.MapHub<ClientHub>("/clientHub");
        host.MapGet("/hello", () => "Hello!");
        host.RunAsync();

        var app = new App();
        app.InitializeComponent();
        app.MainWindow = host.Services.GetRequiredService<MainWindow>();
        app.MainWindow.Visibility = Visibility.Visible;
        app.Run();
    }
}