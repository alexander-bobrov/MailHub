
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using MailHub.Services.MailService.Configuration;

namespace MailHub.Services.MailService
{
    public class SmtpService : IHostedService, IDisposable
    {
        private SmtpServer.SmtpServer server;
        private readonly SmtpOptions options;
        private readonly ILogger<SmtpService> logger;

        public void Dispose(){}

        public SmtpService(SmtpServer.SmtpServer server, ILogger<SmtpService> logger, IOptions<SmtpOptions> options)
        {
            this.options = options.Value;
            this.server = server;
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var task = server.StartAsync(cancellationToken);
            if (task.IsFaulted)
            {
                logger.LogCritical(task.Exception.ToString());
                throw task.Exception;
            }
            logger.LogInformation("Smtp server has been started and ready to recieve e-mails");
            logger.LogInformation($"Settings are: {this.options}");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            server.Shutdown();
            logger.LogInformation("Smtp server has been stopped");
            return Task.CompletedTask;
        }
    }
}
