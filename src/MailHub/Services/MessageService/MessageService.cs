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

        public async Task DeleteMessagesAsync(string mailbox)
        {
            using var db = dbFactory.CreateDbContext();
            var messages = db.Messages.Where(x => x.ToAddress == mailbox);
            db.RemoveRange(messages);
            await db.SaveChangesAsync();
        }
        public async Task<Message[]> GetBasedOnAuthorAsync(string authorEmail, string subject)
        {
            return await GetMessagesAsync(x => x.FromAddress == authorEmail && x.Subject == subject);
        }

        public async Task<Message[]> GetBasedOnRecipientAsync(string recipientEmail, string subject)
        {
            return await GetMessagesAsync(x => x.ToAddress == recipientEmail && x.Subject == subject);
        }

        private async Task<Message[]> GetMessagesAsync(Expression<Func<MessageEntity, bool>> filter)
        {
            using var db = dbFactory.CreateDbContext();
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
    
            return await messages.ToArrayAsync();
        }

#if DEBUG
        public async Task<Message[]> GetAllMessagesAsync()
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
