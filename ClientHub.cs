using Microsoft.AspNetCore.SignalR;
using RemoteDesktopAppService.RemoteApplication;
using RemoteDesktopAppService.Shared;
using RemoteDesktopAppService.SystemApplication;

namespace RemoteDesktopAppService;

public class ClientHub : Hub<IClientHub>
{
    public async Task<RemoteApplicationInfo[]> GetRemoteApplicationInfos()
    {
        var remoteApplicationRegedit = new RemoteApplicationRegedit();
        await Task.Run(remoteApplicationRegedit.Load);
        return remoteApplicationRegedit.RegistryRemoteApps.ToArray();
    }

    public async Task<StartMenuApplication[]> GetStartMenuApplications() =>
        StartMenuApplication.GetStartMenuApplications().ToArray();

    public async Task SetRemoteApplicationInfos(RemoteApplicationInfo[] remoteApplicationInfos)
    {
    }
}

public interface IClientHub
{
    Task<List<RemoteApplicationInfo>> GetRemoteApplicationInfos();
}