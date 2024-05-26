using Microsoft.AspNetCore.SignalR;
using RemoteAppLib;

namespace RemoteDesktopAppService;

public class RemoteAppHub : Hub
{
    public List<string> GetRemoteApps()
    {
        SystemRemoteApps systemRemoteApps = new SystemRemoteApps();
        return systemRemoteApps.GetAll().Cast<RemoteApp>().Select(app => app.Name).ToList();
    }
}