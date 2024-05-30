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

    private readonly RegistryKey _baseKey =
        Registry.LocalMachine.OpenSubKey(RegistryPath, RegistryKeyPermissionCheck.ReadWriteSubTree) ??
        Registry.LocalMachine.CreateSubKey(RegistryPath, RegistryKeyPermissionCheck.ReadWriteSubTree);

    private readonly List<RemoteApplicationInfo> _registryRemoteApps = [];

    public List<RemoteApplicationInfo> RegistryRemoteApps => _registryRemoteApps;

    public void Load()
    {
        _registryRemoteApps.Clear();
        GetSubKeysHasHashKeyName()
            .Select(s => _baseKey.OpenSubKey(s) ?? throw new InvalidOperationException())
            .Select(subKey => new RemoteApplicationInfo(
                subKey.GetValue("Name") as string,
                subKey.GetValue("Path") as string,
                subKey.GetValue("VPath") as string,
                subKey.GetValue("RequiredCommandLine") as string,
                subKey.GetValue("CommandLineSetting") as int?,
                subKey.GetValue("IconPath") as string,
                subKey.GetValue("IconIndex") as int?,
                subKey.GetValue("ShowInTSWA") as int?
            )).ToList().ForEach(_registryRemoteApps.Add);
    }

    public void RemoveAllRemoteAppsInRegistry()
    {
        GetSubKeysHasHashKeyName().ToList().ForEach(_baseKey.DeleteSubKeyTree);
    }

    public void Save()
    {
        var toUpdateKeys = GetSubKeysHasHashKey()
            .ExceptBy(_registryRemoteApps.Select(info => info.ComputeHash().ToString()), key => key.GetValue(HashKeyName)?? throw new InvalidOperationException())
            .ToList();

        toUpdateKeys.Select(key => key.Name.Split(@"\")[^1]).ToList().ForEach(_baseKey.DeleteSubKeyTree);
        _registryRemoteApps.ExceptBy(toUpdateKeys.Select(key => key.GetValue(HashKeyName)), info => info.ComputeHash().ToString()).ToList()
            .ForEach(remoteApp =>
            {
                using var subKey = _baseKey.CreateSubKey(remoteApp.Name);
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

    private IEnumerable<string> GetSubKeysHasHashKeyName() =>
        _baseKey.GetSubKeyNames().Where(s => _baseKey.OpenSubKey(s)?.GetValueNames().Contains(HashKeyName) == true);

    private IEnumerable<RegistryKey> GetSubKeysHasHashKey() => GetSubKeysHasHashKeyName()
        .Select(s => _baseKey.OpenSubKey(s) ?? throw new InvalidOperationException());
}