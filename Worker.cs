using Microsoft.Win32;

namespace RemoteDesktopAppService
{
    public class Worker(ILogger<Worker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    List<string> installedSoftwareList = GetInstalledSoftwareList();
                    string installedSoftwareListString = string.Join("\n", installedSoftwareList);
                    logger.LogInformation("Installed software: {installedSoftwareListString}", installedSoftwareListString);
                }
                await Task.Delay(100000, stoppingToken);
            }
        }

        List<string> GetInstalledSoftwareList()
        {
            List<string> gInstalledSoftware = new();

            string displayName;

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", false))
            {
                foreach (String keyName in key.GetSubKeyNames())
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (string.IsNullOrEmpty(displayName))
                        continue;

                    gInstalledSoftware.Add(displayName.ToLower());
                }
            }

            //using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", false))
            using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", false);
                foreach (String keyName in key.GetSubKeyNames())
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (string.IsNullOrEmpty(displayName))
                        continue;

                    gInstalledSoftware.Add(displayName.ToLower());
                }
            }

            using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", false);
                foreach (String keyName in key.GetSubKeyNames())
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (string.IsNullOrEmpty(displayName))
                        continue;

                    gInstalledSoftware.Add(displayName.ToLower());
                }
            }

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall", false))
            {
                foreach (String keyName in key.GetSubKeyNames())
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (string.IsNullOrEmpty(displayName))
                        continue;

                    gInstalledSoftware.Add(displayName.ToLower());
                }
            }

            return gInstalledSoftware;
        }
    }
}
