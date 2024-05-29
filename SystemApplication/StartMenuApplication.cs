using IWshRuntimeLibrary;
using RemoteDesktopAppService.Shared;
using File = System.IO.File;

namespace RemoteDesktopAppService.SystemApplication;

public record StartMenuApplication(string Name, string ExecutablePath, string ShortcutPathAfterStartMenu)
{
    public static readonly string StartMenuPath =
        Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + @"\Programs";

    public static readonly string StartMenuPathOfCurrentUser =
        Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Programs";

    public static List<StartMenuApplication> GetStartMenuApplications()
    {
        string[] startMenuPaths = [StartMenuPath, StartMenuPathOfCurrentUser];
        return startMenuPaths.Where(Directory.Exists)
            .SelectMany(startMenuPath => Directory.GetFiles(startMenuPath, "*.lnk", SearchOption.AllDirectories),
                (startMenuPath, startMenuShortcutPath) => new { startMenuPath, startMenuShortcutPath })
            .Select(r => new
            {
                r.startMenuPath,
                r.startMenuShortcutPath,
                ExecutablePath = GetShortcutTargetFile(r.startMenuShortcutPath)
            })
            .Where(r => r.ExecutablePath.EndsWith(".exe") && File.Exists(r.ExecutablePath))
            .Select(r => new
            {
                Name = Path.GetFileNameWithoutExtension(r.startMenuShortcutPath),
                r.ExecutablePath,
                startMenuShortcutPathAfterStartMenu = r.startMenuShortcutPath[r.startMenuPath.Length..]
            })
            .Select(t => new StartMenuApplication(t.Name, t.ExecutablePath,
                t.startMenuShortcutPathAfterStartMenu))
            .ToList();
    }

    public RemoteApplicationInfo CreateRemoteApplicationInfo()
    {
        return new RemoteApplicationInfo(Name, ExecutablePath, ShortcutPathAfterStartMenu, "", 0, ExecutablePath, 0, 0);
    }

    public static string GetShortcutTargetFile(string shortcutFilename)
    {
        WshShell shell = new WshShell();
        IWshShortcut link = (IWshShortcut)shell.CreateShortcut(shortcutFilename);
        return link.TargetPath;
    }
}