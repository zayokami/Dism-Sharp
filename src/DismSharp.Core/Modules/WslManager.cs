using System.Diagnostics;

namespace DismSharp.Core.Modules;

public record WslDistroInfo(string Name, string State, string Version, bool IsDefault);

public static class WslManager
{
    public static async Task<List<WslDistroInfo>> GetDistrosAsync()
    {
        return await Task.Run(() =>
        {
            var distros = new List<WslDistroInfo>();

            try
            {
                var psi = new ProcessStartInfo("wsl", "--list --verbose")
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

                    var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines.Skip(1))
                    {
                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 3)
                        {
                            var isDefault = line.Contains('*');
                            var name = isDefault ? parts[0].TrimStart('*') : parts[0];
                            var state = parts[1];
                            var version = parts[2];

                            distros.Add(new WslDistroInfo(name, state, version, isDefault));
                        }
                    }
                }
            }
            catch { }

            return distros;
        }).ConfigureAwait(false);
    }

    public static async Task StartDistroAsync(string name)
    {
        await Task.Run(() =>
        {
            var psi = new ProcessStartInfo("wsl", $"-d {name}")
            {
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            process?.WaitForExit();
        }).ConfigureAwait(false);
    }

    public static async Task ShutdownDistroAsync(string name)
    {
        await Task.Run(() =>
        {
            var psi = new ProcessStartInfo("wsl", $"--terminate {name}")
            {
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            process?.WaitForExit();
        }).ConfigureAwait(false);
    }

    public static async Task UninstallDistroAsync(string name)
    {
        await Task.Run(() =>
        {
            var psi = new ProcessStartInfo("wsl", $"--unregister {name}")
            {
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            process?.WaitForExit();
        }).ConfigureAwait(false);
    }

    public static async Task SetDefaultDistroAsync(string name)
    {
        await Task.Run(() =>
        {
            var psi = new ProcessStartInfo("wsl", $"--set-default {name}")
            {
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            process?.WaitForExit();
        }).ConfigureAwait(false);
    }
}
