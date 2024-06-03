using System.Xml;
using System.Xml.Serialization;
using IWshRuntimeLibrary;
using RemoteDesktopAppService.RemoteApplication;
using File = System.IO.File;

namespace RemoteDesktopAppService.Shared;

public record LocalApplicationInfo(
    string Name,
    string ExecutablePath,
    bool RemoteApp = false)
{
    public LocalApplicationInfo() : this("", "")
    {
    }

    static LocalApplicationInfo()
    {
        LoadApplications();
        if (LocalApplicationInfos.Count == 0)
            LoadStartMenuApplications();
    }

    public bool RemoteApp { get; set; } = RemoteApp;

    private static readonly string LocalApplicationInfoDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RemoteDesktopAppService",
        "LocalApplicationInfo.xml");

    public static readonly string StartMenuPath =
        Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + @"\Programs";

    public static readonly string StartMenuPathOfCurrentUser =
        Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Programs";

    public static List<LocalApplicationInfo> LocalApplicationInfos { get; } = [];


    public static void LoadStartMenuApplications()
    {
        string[] startMenuPaths = [StartMenuPathOfCurrentUser, StartMenuPath];
        LocalApplicationInfos.AddRange(
            startMenuPaths.Where(Directory.Exists)
                .SelectMany(startMenuPath => Directory.GetFiles(startMenuPath, "*.lnk", SearchOption.AllDirectories),
                    (startMenuPath, startMenuShortcutPath) => new { startMenuPath, startMenuShortcutPath })
                .Select(r => new
                {
                    r.startMenuPath,
                    r.startMenuShortcutPath,
                    ShortcutInfo = GetShortcut(r.startMenuShortcutPath)
                })
                .Where(r => r.ShortcutInfo.TargetPath.EndsWith(".exe") && File.Exists(r.ShortcutInfo.TargetPath))
                .Where(r => r.ShortcutInfo.Arguments.Length == 0)
                .Select(r => new
                {
                    Name = Path.GetFileNameWithoutExtension(r.startMenuShortcutPath),
                    ExecutablePath = r.ShortcutInfo.TargetPath
                })
                .Select(t => new LocalApplicationInfo(t.Name, t.ExecutablePath))
                .ExceptBy(LocalApplicationInfos.Select(t => t.ExecutablePath), t => t.ExecutablePath)
        );
    }

    public static void LoadApplications()
    {
        if (!File.Exists(LocalApplicationInfoDataPath)) return;
        using var reader = XmlReader.Create(LocalApplicationInfoDataPath);
        var serializer = new XmlSerializer(typeof(LocalApplicationInfo[]));
        if (!serializer.CanDeserialize(reader)) return;
        var localApplicationInfos = serializer.Deserialize(reader) as LocalApplicationInfo[] ?? [];
        LocalApplicationInfos.AddRange(
            localApplicationInfos.ExceptBy(LocalApplicationInfos.Select(t => t.ExecutablePath), t => t.ExecutablePath));
    }

    public static void SaveApplications()
    {
        if (!Directory.Exists(Path.GetDirectoryName(LocalApplicationInfoDataPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(LocalApplicationInfoDataPath) ??
                                      throw new InvalidOperationException());
        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            NewLineChars = "\n",
            NewLineHandling = NewLineHandling.Replace
        };
        using var writer = XmlWriter.Create(LocalApplicationInfoDataPath, settings);
        var serializer = new XmlSerializer(typeof(LocalApplicationInfo[]));
        serializer.Serialize(writer, LocalApplicationInfos.ToArray());
    }

    public static void SaveRemoteApplicationToRegistry()
    {
        var remoteApplicationInfos =
            LocalApplicationInfos.Where(t => t.RemoteApp).Select(t => t.CreateRemoteApplicationInfo());
        RemoteApplicationRegedit.RegistryRemoteApps.Clear();
        RemoteApplicationRegedit.RegistryRemoteApps.AddRange(remoteApplicationInfos);
        RemoteApplicationRegedit.Save();
    }

    public RemoteApplicationInfo CreateRemoteApplicationInfo()
    {
        return new RemoteApplicationInfo(Name, ExecutablePath, ExecutablePath, "", 0, ExecutablePath, 0, 0);
    }

    public static IWshShortcut GetShortcut(string shortcutFilename)
    {
        WshShell shell = new WshShell();
        IWshShortcut link = (IWshShortcut)shell.CreateShortcut(shortcutFilename);
        return link;
    }
}