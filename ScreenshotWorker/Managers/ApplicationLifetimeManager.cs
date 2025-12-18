using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ScreenshotWorker.Managers;

public sealed class ApplicationLifetimeManager(ILogger<ApplicationLifetimeManager> logger, IMessageBrokerManager messageBrokerManager) : IApplicationLifetimeManager
{
    private readonly IMessageBrokerManager _messageBrokerManager = messageBrokerManager;
    private readonly ILogger<ApplicationLifetimeManager> _logger = logger;
    private readonly CancellationTokenSource _cts = new();
    private bool _started = false;
    private bool _disposed = false;

    public CancellationToken CancellationToken => _cts.Token;

    public async Task StartApplicationAsync()
    {
        ThrowIfDisposed();

        if (_started)
            return;

        await InstallPlaywrightIfNeeded();
        await _messageBrokerManager.InitializeAsync();

        _started = true;
    }

    public Task StopApplicationAsync()
    {
        ThrowIfDisposed();

        if (_started)
        {
            _cts.Cancel();
            Dispose();
            _started = false;
        }

        return Task.CompletedTask;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ApplicationLifetimeManager), "The application lifetime manager has already been disposed.");
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _cts.Dispose();
    }

    private static string GetShell(string scriptFile) =>
        (Path.GetExtension(scriptFile).ToLowerInvariant(), RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) switch
        {
            (".cmd", true) => scriptFile,
            (".ps1", true) => "powershell.exe",
            (".sh", false) => "/bin/bash",
            _ => throw new NotSupportedException($"Unsupported script type or platform for: {scriptFile}")
        };

    private static string GetArguments(string scriptFile) =>
        (Path.GetExtension(scriptFile).ToLowerInvariant(), RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) switch
        {
            (".cmd", true) => "install",
            (".ps1", true) => $"-ExecutionPolicy Bypass -File \"{scriptFile}\" install",
            (".sh", false) => $"\"{scriptFile}\" install",
            _ => throw new NotSupportedException($"Unsupported script type or platform for: {scriptFile}")
        };

    private static bool IsChromiumInstalled()
    {
        var baseDir = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ms-playwright")
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache", "ms-playwright");

        return Directory.EnumerateDirectories(baseDir, "chromium-*", SearchOption.TopDirectoryOnly).Any();
    }


    private async Task InstallPlaywrightIfNeeded()
    {
        if (IsChromiumInstalled())
            return;

        var binDir = AppContext.BaseDirectory;

        var scriptFile = Directory.GetFiles(binDir, "playwright.*")
            .FirstOrDefault(f =>
                f.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".sh", StringComparison.OrdinalIgnoreCase));

        if (scriptFile is null)
            throw new FileNotFoundException("No Playwright install script found in output directory.");

        var shell = GetShell(scriptFile);
        var arguments = GetArguments(scriptFile);

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = shell,
                Arguments = arguments,
                WorkingDirectory = binDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (!string.IsNullOrWhiteSpace(error))
            _logger.LogCritical("Playwright cannot be setup, see error for details: {0}", error);

        if (process.ExitCode != 0)
            throw new Exception($"Playwright install failed with code {process.ExitCode}");
    }
}