using Microsoft.AspNetCore.SignalR;
using RemoteDesktopAppService.RemoteApplication;
using RemoteDesktopAppService.Shared;

namespace RemoteDesktopAppService;

public class ClientHub : Hub<IClientHub>
{
    public async Task<string[]> GetRemoteApplicationInfos()
    {
        var remoteApplicationRegedit = new RemoteApplicationRegedit();
        await Task.Run(remoteApplicationRegedit.Load);
        return remoteApplicationRegedit.RegistryRemoteApps.Select(RemoteApplicationInfo.ToJson).ToArray();
    }

    public void Test()
    {
        string json = """
                      {
                        "name": "7-Zip File Manager",
                        "path": "C:\\Program Files\\7-Zip\\7zFM.exe",
                        "vPath": "C:\\Program Files\\7-Zip\\7zFM.exe",
                        "commandLine": "",
                        "commandLineOption": 0,
                        "iconPath": "C:\\Program Files\\7-Zip\\7zFM.exe",
                        "iconIndex": 0,
                        "tSWA": 0
                      }
                      """;
        RemoteApplicationInfo testInfo = new("7-Zip File Manager", "C:\\Program Files\\7-Zip\\7zFM.exe", "C:\\Program Files\\7-Zip\\7zFM.exe", "", 0, "C:\\Program Files\\7-Zip\\7zFM.exe", 0, 0);
        // json = testInfo.ToJson();
        var remoteApp = RemoteApplicationInfo.FromJson(json);
    }
}

public interface IClientHub
{
    Task<List<RemoteApplicationInfo>> GetRemoteApplicationInfos();
}
