using Microsoft.Win32;
using RemoteDesktopAppService.Shared;

namespace RemoteDesktopAppService.RemoteApplication;

public class RemoteApplicationRegedit(string prefix)
{
    public static readonly string RegistryPath =
        @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Terminal Server\TSAppAllowList\Applications";

    private readonly RegistryKey _baseKey =
        Registry.LocalMachine.OpenSubKey(RegistryPath) ?? throw new InvalidOperationException();

    private readonly List<RemoteApplicationInfo> _registryRemoteApps = [];

    public List<RemoteApplicationInfo> RegistryRemoteApps => _registryRemoteApps;

    public void Load()
    {
        _registryRemoteApps.Clear();
        _baseKey.GetSubKeyNames()
            .Where(s => s.StartsWith(prefix))
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

    public void Save()
    {
        var toUpdateKeysName = _baseKey.GetSubKeyNames().Where(s => s.StartsWith(prefix))
            .ExceptBy(_registryRemoteApps.Select(info => info.GetHashCode().ToString()), s => s[prefix.Length..])
            .ToList();
        toUpdateKeysName.ForEach(_baseKey.DeleteSubKeyTree);
        _registryRemoteApps.ExceptBy(toUpdateKeysName, info => $"{prefix}{info.GetHashCode().ToString()}").ToList()
            .ForEach(remoteApp =>
            {
                using var subKey = _baseKey.CreateSubKey($"{prefix}{remoteApp.GetHashCode()}");
                subKey.SetValue("Name", remoteApp.Name);
                subKey.SetValue("Path", remoteApp.Path);
                subKey.SetValue("VPath", remoteApp.VPath);
                subKey.SetValue("RequiredCommandLine", remoteApp.CommandLine);
                subKey.SetValue("CommandLineSetting", remoteApp.CommandLineOption);
                subKey.SetValue("IconPath", remoteApp.IconPath);
                subKey.SetValue("IconIndex", remoteApp.IconIndex);
                subKey.SetValue("ShowInTSWA", remoteApp.TSWA);
            });
    }
}