using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class CleanUpImageService : BackgroundService
{
    private readonly ILogger<CleanUpImageService> _logger;
    private readonly string _imagesDirectoryPath;

    public CleanUpImageService(ILogger<CleanUpImageService> logger)
    {
        _logger = logger;
        _imagesDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "imagenes");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);

            try
            {
                DeleteFilesInDirectory(_imagesDirectoryPath);
                 _logger.LogError("Se eliminaron los archivos en la carpeta de imágenes");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar los archivos en la carpeta de imágenes: {ex.Message}");
            }
        }
    }

    private void DeleteFilesInDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            var files = Directory.GetFiles(directoryPath);

            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                    _logger.LogInformation($"Archivo eliminado: {file}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"No se pudo eliminar el archivo {file}: {ex.Message}");
                }
            }
        }
        else
        {
            _logger.LogWarning($"La carpeta de imágenes no existe: {directoryPath}");
        }
    }
}
