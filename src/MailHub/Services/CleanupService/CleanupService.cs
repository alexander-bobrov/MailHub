using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailHub.Services.CleanupService.Configuration;

namespace MailHub.Services.CleanupService
{
    public class CleanupService : IHostedService, IDisposable
    {
        private readonly IDbContextFactory<MailHubContext> dbFactory;
        private readonly ILogger<CleanupService> logger;
        private readonly CleanupOptions options;
        private Timer timer = null;

        public CleanupService(ILogger<CleanupService> logger, IOptions<CleanupOptions> options, IDbContextFactory<MailHubContext> dbFactory)
        {
            this.dbFactory = dbFactory;
            this.logger = logger;
            this.options = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Background expired account cleaning has beed started");
            timer = new Timer(Cleanup, null, TimeSpan.Zero, options.CleanupInterval);

            return Task.CompletedTask;
        }

        private async void Cleanup(object state)
        {
            using (var db = dbFactory.CreateDbContext())
            {

                var messagesToDelete = db.Messages.Where(x => x.CreatedAtUtc.AddHours(options.RetentionPeriodInHours) > DateTime.UtcNow);
                foreach (var message in messagesToDelete)
                {
                    db.Messages.Remove(message);
                }

                await db.SaveChangesAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);

            logger.LogInformation("Background expired account cleaning has beed stopped");
            return Task.CompletedTask;
        }

        
        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
