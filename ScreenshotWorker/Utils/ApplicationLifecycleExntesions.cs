using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScreenshotWorker.Managers;

namespace ScreenshotWorker.Utils;

public static class ApplicationLifecycleExntesions
{
    public static async Task StartApplicationAsync(this IHost app, string[] args)
    {
        var command = ParseCommand(args);
        using var screenshotWorkerApplication = app.Services.GetRequiredService<IApplicationLifetimeManager>();

        if (command == WorkerCommand.Setup)
        {
            await screenshotWorkerApplication.SetupApplicationAsync();
        }
        else
        {
            await screenshotWorkerApplication.StartApplicationAsync();
            await app.RunAsync(screenshotWorkerApplication.CancellationToken);
        }
    }
    private static WorkerCommand ParseCommand(string[] args)
    {
        if (args.Length != 1)
            throw new ArgumentException(
                "Command required. Supported commands: setup | start");

        return args[0].ToLowerInvariant() switch
        {
            "setup" => WorkerCommand.Setup,
            "start" => WorkerCommand.Start,
            _ => throw new ArgumentException(
                $"Unknown command '{args[0]}'. Supported commands: setup | start")
        };
    }

    private enum WorkerCommand
    {
        Setup,
        Start
    }
}
