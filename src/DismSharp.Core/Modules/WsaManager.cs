using System.Diagnostics;

namespace DismSharp.Core.Modules;

public record WsaAppInfo(string PackageName, string DisplayName, string Version, string InstallLocation);

public static class WsaManager
{
    public static async Task<List<WsaAppInfo>> GetInstalledAppsAsync()
    {
        return await Task.Run(() =>
        {
            var apps = new List<WsaAppInfo>();

            try
            {
                var psi = new ProcessStartInfo("powershell", """
                    -Command "Get-AppxPackage | Where-Object {$_.PackageFullName -like '*WSA*' -or $_.Dependencies.PackageFullName -like '*WSA*'} | Select-Object Name, PackageFullName, Version, InstallLocation | ConvertTo-Json"
                    """)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if (process != null)
                {
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var line in lines)
                        {
                            var trimmed = line.Trim();
                            if (trimmed.StartsWith("{"))
                            {
                                var name = ExtractJsonValue(trimmed, "Name");
                                var fullName = ExtractJsonValue(trimmed, "PackageFullName");
                                var version = ExtractJsonValue(trimmed, "Version");
                                var location = ExtractJsonValue(trimmed, "InstallLocation");

                                if (!string.IsNullOrEmpty(name))
                                    apps.Add(new WsaAppInfo(fullName, name, version, location));
                            }
                        }
                    }
                }
            }
            catch { }

            return apps;
        }).ConfigureAwait(false);
    }

    public static async Task<bool> IsWsaInstalledAsync()
    {
        return await Task.Run(() =>
        {
            try
            {
                var psi = new ProcessStartInfo("powershell", """
                    -Command "Get-AppxPackage -Name MicrosoftCorporationII.WindowsSubsystemForAndroid -ErrorAction SilentlyContinue | Select-Object -ExpandProperty PackageFullName"
                    """)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if (process != null)
                {
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return !string.IsNullOrWhiteSpace(output);
                }
            }
            catch { }

            return false;
        }).ConfigureAwait(false);
    }

    public static async Task UninstallAppAsync(string packageFullName)
    {
        await Task.Run(() =>
        {
            var psi = new ProcessStartInfo("powershell", $"""
                -Command "Remove-AppxPackage -Package '{packageFullName}'"
                """)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                Verb = "runas"
            };

            using var process = Process.Start(psi);
            process?.WaitForExit();
        }).ConfigureAwait(false);
    }

    private static string ExtractJsonValue(string json, string key)
    {
        var keyPattern = $"\"{key}\":";
        var keyIndex = json.IndexOf(keyPattern);
        if (keyIndex < 0) return "";

        var valueStart = json.IndexOf('"', keyIndex + keyPattern.Length);
        if (valueStart < 0) return "";

        var valueEnd = json.IndexOf('"', valueStart + 1);
        if (valueEnd < 0) return "";

        return json.Substring(valueStart + 1, valueEnd - valueStart - 1);
    }
}
