using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteDesktopAppService.Shared
{
    public record InstalledSoftwareInfo(string? DisplayIcon, string? DisplayName, string? DisplayVersion,
        string? InstallLocation, string? Publisher, string? QuietUninstallString, string? UninstallString);
}
