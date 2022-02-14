using Database;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;
using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MailHub.Services.MailService
{
    public class MessageStorage : MessageStore
    {
        private readonly IDbContextFactory<MailHubContext> dbFactory;

        public MessageStorage(IDbContextFactory<MailHubContext> dbFactory)
        {
            this.dbFactory = dbFactory;
        }
        public override async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
        {
            await using var stream = new MemoryStream();

            var position = buffer.GetPosition(0);
            while (buffer.TryGet(ref position, out var memory))
            {
                await stream.WriteAsync(memory, cancellationToken);
            }

            stream.Position = 0;
            var message = await MimeMessage.LoadAsync(stream, cancellationToken);
           
            using (var db = dbFactory.CreateDbContext())
            {
                var from = message.From[0] as MailboxAddress;
                var to = message.To[0] as MailboxAddress;

                db.Messages.Add(new Database.Entities.MessageEntity
                {
                    FromName = from.Name,
                    FromAddress = from.Address,
                    ToName = to.Name,
                    ToAddress = to.Address,
                    Text = message.TextBody,
                    Html = message.HtmlBody,
                    Subject = message.Subject,
                    
                    CreatedAtUtc = DateTime.UtcNow

                });

                await db.SaveChangesAsync(cancellationToken);
            }

            return SmtpResponse.Ok;
        }

    }
}
