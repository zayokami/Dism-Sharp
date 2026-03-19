namespace DismSharp.Core.CleanupRules;

public class DirectXShaderCacheRule : FileCleanupRule
{
    public override string Name => "DirectX 着色器缓存";
    public override string Description => "清理 DirectX 着色器编译缓存";

    public override Task<List<CleanupEntry>> ScanAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var entries = new List<CleanupEntry>();

            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var paths = new[]
            {
                Path.Combine(localAppData, "Microsoft", "DirectX Shader Cache"),
                Path.Combine(localAppData, "D3DSCache"),
                Path.Combine(localAppData, "NVIDIA", "DXCache"),
                Path.Combine(localAppData, "AMD", "DxCache"),
            };

            foreach (var path in paths)
            {
                if (Directory.Exists(path))
                    entries.AddRange(ScanDirectory(path));
            }

            return entries;
        }, cancellationToken);
    }
}
