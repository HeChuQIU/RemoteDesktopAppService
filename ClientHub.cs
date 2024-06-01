﻿using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.SignalR;
using RemoteDesktopAppService.RemoteApplication;
using RemoteDesktopAppService.Shared;

namespace RemoteDesktopAppService;

public class ClientHub : Hub<IClientHub>
{


    public async Task<RemoteApplicationInfo[]> GetRemoteApplicationInfos()
    {
        var remoteApplicationRegedit = new RemoteApplicationRegedit();
        await Task.Run(remoteApplicationRegedit.Load);
        return remoteApplicationRegedit.RegistryRemoteApps.ToArray();
    }

    public async Task<LocalApplicationInfo[]> GetLocalApplicationInfos() => LocalApplicationInfo.LocalApplicationInfos.ToArray();

    public async Task SetLocalApplicationInfos(LocalApplicationInfo[] localApplicationInfos)
    {
        LocalApplicationInfo.LocalApplicationInfos.Clear();
        LocalApplicationInfo.LocalApplicationInfos.AddRange(localApplicationInfos);
    }

    public async Task LoadStartMenuApplications() =>
        LocalApplicationInfo.LoadStartMenuApplications();

    public async Task LoadApplications() => LocalApplicationInfo.LoadApplications();

    public async Task SaveApplications() => LocalApplicationInfo.SaveApplications();

    public async Task SetRemoteApplicationInfos(RemoteApplicationInfo[] remoteApplicationInfos)
    {
    }
}

public interface IClientHub
{
    Task<List<RemoteApplicationInfo>> GetRemoteApplicationInfos();
}