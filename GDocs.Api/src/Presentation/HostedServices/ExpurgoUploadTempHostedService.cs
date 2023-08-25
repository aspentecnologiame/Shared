using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.HostedServices
{
    public class ExpurgoUploadTempHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly string _uploadTempPath;

        public ExpurgoUploadTempHostedService(
            ILogger<ExpurgoUploadTempHostedService> logger,
            Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment
        )
        {
            _logger = logger;

            if (string.IsNullOrWhiteSpace(hostingEnvironment.WebRootPath))
            {
                hostingEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            _uploadTempPath = Path.Combine(hostingEnvironment.WebRootPath, "uploads", "temp");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!Directory.Exists(_uploadTempPath))
                    return Task.CompletedTask;

                _logger.LogWarning($"[Expurgo upload temp] Iniciado.");

                var info = new DirectoryInfo(_uploadTempPath);
                var files = info.GetFiles().Where(p => p.CreationTime < DateTime.Now.AddDays(-1)).ToArray();
                foreach (FileInfo file in files)
                {
                    file.Delete();
                    _logger.LogInformation($"[Expurgo upload temp] Arquivo {file.FullName} deletado.");
                }

                _logger.LogWarning($"[Expurgo upload temp] Finalizado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Expurgo upload temp] {ex.Message}");
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
