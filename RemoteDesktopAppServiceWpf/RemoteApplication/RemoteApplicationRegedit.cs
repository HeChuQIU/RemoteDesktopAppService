using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;
using RemoteDesktopAppService.Shared;

namespace RemoteDesktopAppService.RemoteApplication;

public class RemoteApplicationRegedit
{
    public static readonly string RegistryPath =
        @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Terminal Server\TSAppAllowList\Applications";

    public static readonly string HashKeyName = "RemoteDesktopAppServiceHash";

    private static readonly RegistryKey BaseKey =
        Registry.LocalMachine.OpenSubKey(RegistryPath, RegistryKeyPermissionCheck.ReadWriteSubTree) ??
        Registry.LocalMachine.CreateSubKey(RegistryPath, RegistryKeyPermissionCheck.ReadWriteSubTree);

    public static List<RemoteApplicationInfo> RegistryRemoteApps { get; } = [];

    static RemoteApplicationRegedit()
    {
        Load();
    }

    public static void Load()
    {
        RegistryRemoteApps.Clear();
        GetSubKeysHasHashKeyName()
            .Select(s => BaseKey.OpenSubKey(s) ?? throw new InvalidOperationException())
            .Select(subKey => new RemoteApplicationInfo(
                subKey.GetValue("Name") as string,
                subKey.GetValue("Path") as string,
                subKey.GetValue("VPath") as string,
                subKey.GetValue("RequiredCommandLine") as string,
                subKey.GetValue("CommandLineSetting") as int?,
                subKey.GetValue("IconPath") as string,
                subKey.GetValue("IconIndex") as int?,
                subKey.GetValue("ShowInTSWA") as int?
            )).ToList().ForEach(RegistryRemoteApps.Add);
    }

    public static void RemoveAllRemoteAppsInRegistry()
    {
        GetSubKeysHasHashKeyName().ToList().ForEach(BaseKey.DeleteSubKeyTree);
    }

    public static void Save()
    {
        var toUpdateKeys = GetSubKeysHasHashKey()
            .ExceptBy(RegistryRemoteApps.Select(info => info.ComputeHash().ToString()), key => key.GetValue(HashKeyName)?? throw new InvalidOperationException())
            .ToList();

        toUpdateKeys.Select(key => key.Name.Split(@"\")[^1]).ToList().ForEach(BaseKey.DeleteSubKeyTree);
        RegistryRemoteApps.ExceptBy(toUpdateKeys.Select(key => key.GetValue(HashKeyName)), info => info.ComputeHash().ToString()).ToList()
            .ForEach(remoteApp =>
            {
                using var subKey = BaseKey.CreateSubKey(remoteApp.Name);
                subKey.SetValue("Name", remoteApp.Name);
                subKey.SetValue("Path", remoteApp.Path);
                subKey.SetValue("VPath", remoteApp.VPath);
                subKey.SetValue("RequiredCommandLine", remoteApp.CommandLine);
                subKey.SetValue("CommandLineSetting", remoteApp.CommandLineOption);
                subKey.SetValue("IconPath", remoteApp.IconPath);
                subKey.SetValue("IconIndex", remoteApp.IconIndex);
                subKey.SetValue("ShowInTSWA", remoteApp.TSWA);
                subKey.SetValue(HashKeyName, remoteApp.ComputeHash().ToString());
            });
    }

    private static IEnumerable<string> GetSubKeysHasHashKeyName() =>
        BaseKey.GetSubKeyNames().Where(s => BaseKey.OpenSubKey(s)?.GetValueNames().Contains(HashKeyName) == true);

    private static IEnumerable<RegistryKey> GetSubKeysHasHashKey() => GetSubKeysHasHashKeyName()
        .Select(s => BaseKey.OpenSubKey(s) ?? throw new InvalidOperationException());
}