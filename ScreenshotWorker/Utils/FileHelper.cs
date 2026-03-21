using System.Reflection;

namespace ScreenshotWorker.Utils;

public static class FileHelper
{
    public static string LoadEmbeddedFile(string filename)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{assembly.GetName().Name}.{filename.Replace('/', '.').Replace('\\', '.')}";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new Exception($"Resource {resourceName} not found");

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}
