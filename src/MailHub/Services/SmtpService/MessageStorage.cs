using Database;
using Database.Entities;
using HtmlAgilityPack;
using MailHub.Utils;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Serilog;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;
using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

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

            string text = null;
            if (message.TextBody is null && message.HtmlBody is not null)
            {
                var raw = HtmlToText.ConvertHtml(message.HtmlBody);
                var formatted = raw.
                    Replace("\u00a0", string.Empty).
                    Replace("\r", string.Empty);
                text = formatted;
            }

            var attachmentsMime = message.BodyParts.Where(x => x.ContentDisposition != null && x.ContentDisposition.FileName != null).ToArray();

            foreach(var a in attachmentsMime)
            {
                Log.Information(a.ToString());
            }

            using (var db = dbFactory.CreateDbContext())
            {
                var from = message.From[0] as MailboxAddress;
                var to = message.To[0] as MailboxAddress;

                var attachments = attachmentsMime.Select(x => new AttachmentEntity
                {       
                    ContentId = x.ContentId,
                    Filename = x.ContentType?.Name,
                    ContentType = x.ContentType?.Format
                }).ToArray();

                db.Messages.Add(new MessageEntity
                {
                    FromName = from.Name,
                    FromAddress = from.Address,
                    ToName = to.Name,
                    ToAddress = to.Address,
                    Text = text,
                    Html = message.HtmlBody,
                    Subject = message.Subject,
                    Attachments = attachments,

                    CreatedAtUtc = DateTime.UtcNow

                });

                await db.SaveChangesAsync(cancellationToken);
            }

            return SmtpResponse.Ok;
        }

    }
}
