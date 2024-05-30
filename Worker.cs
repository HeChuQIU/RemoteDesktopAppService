using Microsoft.Win32;
using RemoteDesktopAppService.RemoteApplication;
using RemoteDesktopAppService.SystemApplication;

namespace RemoteDesktopAppService
{
    public class Worker(ILogger<Worker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // while (!stoppingToken.IsCancellationRequested)
            // {
            //     if (logger.IsEnabled(LogLevel.Information))
            //     {
            //         logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //         List<string> installedSoftwareList = GetInstalledSoftwareList();
            //         string installedSoftwareListString = string.Join("\n", installedSoftwareList);
            //         logger.LogInformation("Installed software: {installedSoftwareListString}", installedSoftwareListString);
            //     }
            //     await Task.Delay(100000, stoppingToken);
            // }
            var startMenuApplications = StartMenuApplication.GetStartMenuApplications();
            var remoteApplicationRegedit = new RemoteApplicationRegedit();

            // remoteApplicationRegedit.RemoveAllRemoteAppsInRegistry();

            remoteApplicationRegedit.Load();
            var remoteApplications = remoteApplicationRegedit.RegistryRemoteApps;

            remoteApplicationRegedit.RegistryRemoteApps.Clear();
            remoteApplicationRegedit.RegistryRemoteApps.AddRange(
                startMenuApplications.Select(app => app.CreateRemoteApplicationInfo()));
            remoteApplicationRegedit.Save();
        }
    }
}