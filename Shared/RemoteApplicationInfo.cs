using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RemoteDesktopAppService.Shared;

public record RemoteApplicationInfo
{
    public RemoteApplicationInfo(
        string? name,
        string? path,
        string? vPath,
        string? commandLine,
        int? commandLineOption,
        string? iconPath,
        int? iconIndex,
        int? tSWA)
    {
        Name = name ?? string.Empty;
        Path = path ?? string.Empty;
        VPath = vPath ?? string.Empty;
        CommandLine = commandLine ?? string.Empty;
        CommandLineOption = commandLineOption ?? 0;
        IconPath = iconPath ?? string.Empty;
        IconIndex = iconIndex ?? 0;
        TSWA = tSWA ?? 0;
    }



    public RemoteApplicationInfo()
    {
    }

    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string VPath { get; set; } = string.Empty;
    public string CommandLine { get; set; } = string.Empty;
    public int CommandLineOption { get; set; } = 0;
    public string IconPath { get; set; } = string.Empty;
    public int IconIndex { get; set; } = 0;
    public int TSWA { get; set; } = 0;

    public string ComputeHash()
    {
        var combined = Name + Path + VPath + CommandLine + CommandLineOption + IconPath + IconIndex + TSWA;

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(combined));

        var builder = new StringBuilder();
        foreach (var t in bytes)
        {
            builder.Append(t.ToString("x2"));
        }

        return builder.ToString();
    }

    public string ToJson() => JsonSerializer.Serialize(this);
    public static string ToJson(RemoteApplicationInfo remoteApplicationInfo) => remoteApplicationInfo.ToJson();

    public static RemoteApplicationInfo FromJson(string json) =>
        JsonSerializer.Deserialize<RemoteApplicationInfo>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new InvalidOperationException();
}