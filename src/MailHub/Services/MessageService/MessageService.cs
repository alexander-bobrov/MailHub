using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using MailHub.Services.MessageService.Models;
using System;
using Database.Entities;
using System.Threading.Tasks;
using System.Linq.Expressions;

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

        public async Task<Message[]> GetBasedOnAuthor(string authorEmail, string subject)
        {
            return await GetMessages(x => x.FromAddress == authorEmail && x.Subject == subject);
        }

        public async Task<Message[]> GetBasedOnRecipient(string recipientEmail, string subject)
        {
            return await GetMessages(x => x.ToAddress == recipientEmail && x.Subject == subject);
        }

        private async Task<Message[]> GetMessages(Expression<Func<MessageEntity, bool>> filter)
        {
            using var db = dbFactory.CreateDbContext();
            var sw = new Stopwatch();
            sw.Start();
            var messages = db.Messages.AsNoTracking().Where(filter)
                .OrderByDescending(o => o.CreatedAtUtc)
                .Select(m => new Message
                {
                    From = new Person { Name = m.FromName, Address = m.FromAddress },
                    To = new Person { Name = m.ToName, Address = m.ToAddress },
                    Text = m.Text,
                    Html = m.Html,
                    Subject = m.Subject,
                    Attachments = m.Attachments.Select(x => new Attachment
                    {
                        ContentId = x.ContentId,
                        Filename = x.Filename,
                        ContentType = x.ContentType,
                    }).ToArray()
                });
    
            sw.Stop();
            var result = await messages.ToArrayAsync();

            logger.LogInformation($"{result.Length} messages have been retrieved from DB for {sw.ElapsedMilliseconds}ms");
            return result;
        }

#if DEBUG
        public async Task<Message[]> GetAll()
        {
            using var db = dbFactory.CreateDbContext();

            var messages = db.Messages.AsNoTracking().Select(m => new Message
            {
                From = new Person { Name = m.FromName, Address = m.FromAddress },
                To = new Person { Name = m.ToName, Address = m.ToAddress },
                Text = m.Text,
                Html = m.Html,
                Subject = m.Subject,
            });

            return await messages.ToArrayAsync();
        }
#endif
    }
}
