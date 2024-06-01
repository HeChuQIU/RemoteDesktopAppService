using Microsoft.Win32;
using RemoteDesktopAppService.RemoteApplication;
using RemoteDesktopAppService.Shared;

namespace RemoteDesktopAppService
{
    public class Worker(ILogger<Worker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

        }
    }
}