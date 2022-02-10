using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MailHub.Services.MessageService.Models;

namespace MailHub.Services.MessageService
{
    public class MessageService
    {
        private readonly IDbContextFactory<MailHubContext> dbFactory;
        private readonly ILogger<MessageService> logger;

        public MessageService(IDbContextFactory<MailHubContext> dbFactory, ILogger<MessageService> logger)
        {
            this.dbFactory = dbFactory;
            this.logger = logger;
        }

        public async Task<Message[]> GetAll()
        {
            using var db = dbFactory.CreateDbContext();

            var messages = db.Messages.AsNoTracking().Select(m => new Message
            {
                From = new Person { Name = m.FromName, Address = m.FromAddress},
                To = new Person { Name = m.ToName, Address = m.ToAddress },
                Text = m.Text,
                Html = m.Html,
                Subject = m.Subject,
            });

            return await messages.ToArrayAsync();
        }
        public async Task<Message[]> GetBasedOnAuthor(string authorEmail, string subject)
        {
            using var db = dbFactory.CreateDbContext();
            //todo slow but OK for now
            var sw = new Stopwatch();
            sw.Start();
            var messages = db.Messages.AsNoTracking().Where(x => x.FromAddress == authorEmail && x.Subject == subject)
                .OrderByDescending(o => o.CreatedAtUtc)
                .Select(m => new Message
                {
                    From = new Person { Name = m.FromName, Address = m.FromAddress },
                    To = new Person { Name = m.ToName, Address = m.ToAddress },
                    Text = m.Text,
                    Html = m.Html,
                    Subject = m.Subject,
                });
            var result = await messages.ToArrayAsync();
            sw.Stop();

            logger.LogInformation($"{result.Length} messages have been retrieved from DB for {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<Message[]> GetBasedOnRecipient(string recipientEmail, string subject)
        {
            using var db = dbFactory.CreateDbContext();
            //todo slow but OK for now
            var sw = new Stopwatch();
            sw.Start();
            var messages = db.Messages.AsNoTracking().Where(x => x.ToAddress == recipientEmail && x.Subject == subject)
                .OrderByDescending(o => o.CreatedAtUtc)
                .Select(m => new Message
                {
                    From = new Person { Name = m.FromName, Address = m.FromAddress },
                    To = new Person { Name = m.ToName, Address = m.ToAddress },
                    Text = m.Text,
                    Html = m.Html,
                    Subject = m.Subject,
                });
            var result = await messages.ToArrayAsync();
            sw.Stop();

            logger.LogInformation($"{result.Length} messages have been retrieved from DB for {sw.ElapsedMilliseconds}ms");
            return result;
        }

    }
}
