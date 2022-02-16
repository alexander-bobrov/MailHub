
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmtpServer;
using SmtpServer.ComponentModel;
using System;
using System.Threading;
using System.Threading.Tasks;
using MailHub.Services.MailService.Configuration;
using MailHub.Services.SmtpService;

namespace MailHub.Services.MailService
{
    public class SmtpService : IHostedService, IDisposable
    {
        private SmtpServer.SmtpServer server;
        private readonly SmtpOptions options;
        private readonly ILogger<SmtpService> logger;
        private readonly IDbContextFactory<MailHubContext> dbFactory;

        public void Dispose(){}

        public SmtpService(ILogger<SmtpService> logger, IOptions<SmtpOptions> options, IDbContextFactory<MailHubContext> dbFactory)
        {
            this.options = options.Value;
            this.logger = logger;
            this.dbFactory = dbFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var options = new SmtpServerOptionsBuilder()
            .ServerName(this.options.ServerName)
            .Port(25, 587)
            .Build();

            
            var sp = new SmtpServer.ComponentModel.ServiceProvider();
            sp.Add(new MessageFilter(this.options.AllowedDomains));
            sp.Add(new MessageStorage(dbFactory));

            server = new SmtpServer.SmtpServer(options, sp);

            //idk why but it's a blocking call so
            var task = server.StartAsync(cancellationToken);
            if (task.IsFaulted)
            {
                logger.LogCritical(task.Exception.ToString());
                throw task.Exception;
            }
            logger.LogInformation("Smtp server has been started and ready to recieve e-mails");

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
