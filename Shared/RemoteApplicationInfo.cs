using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteDesktopAppService.Shared;

public record RemoteApplicationInfo
{
    public RemoteApplicationInfo(
        string? Name,
        string? Path,
        string? VPath,
        string? CommandLine,
        int? CommandLineOption,
        string? IconPath,
        int? IconIndex,
        int? TSWA)
    {
        this.Name = Name ?? string.Empty;
        this.Path = Path ?? string.Empty;
        this.VPath = VPath ?? string.Empty;
        this.CommandLine = CommandLine ?? string.Empty;
        this.CommandLineOption = CommandLineOption ?? 0;
        this.IconPath = IconPath ?? string.Empty;
        this.IconIndex = IconIndex ?? 0;
        this.TSWA = TSWA ?? 0;
    }

    public string Name { get; }
    public string Path { get; }
    public string VPath { get; }
    public string CommandLine { get; }
    public int CommandLineOption { get; }
    public string IconPath { get; }
    public int IconIndex { get; }
    public int TSWA { get; }
}