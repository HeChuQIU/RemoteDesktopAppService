using System.Drawing;
using Microsoft.AspNetCore.SignalR;
using RemoteDesktopAppService.RemoteApplication;

namespace RemoteDesktopAppService;

public class ClientHub : Hub
{
    public async Task<string[]> GetRemoteApplicationNames()
    {
        return RemoteApplicationRegedit.RegistryRemoteApps.Select(info => info.Name).ToArray();
    }

    public async Task<string> GetIconBase64OfRemoteApplication(string name)
    {
        var remoteApp = RemoteApplicationRegedit.RegistryRemoteApps.FirstOrDefault(info => info.Name == name);
        return remoteApp == null ? string.Empty : BitmapToBase64(GetIconFromExe(remoteApp.Path));
    }

    public async Task<string[]> GetIconBase64OfRemoteApplications()
    {
        return RemoteApplicationRegedit.RegistryRemoteApps.Select(info => BitmapToBase64(GetIconFromExe(info.Path)))
            .ToArray();
    }

    Bitmap? GetIconFromExe(string filePath)
    {
        return Icon.ExtractAssociatedIcon(filePath)?.ToBitmap();
    }

    string BitmapToBase64(Bitmap? bitmap)
    {
        if (bitmap is null) return string.Empty;
        using var memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
        byte[] bitmapBytes = memoryStream.ToArray();
        return Convert.ToBase64String(bitmapBytes);
    }
}