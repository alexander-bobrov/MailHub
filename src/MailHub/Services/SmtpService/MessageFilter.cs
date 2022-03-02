using MailHub.Services.MailService.Configuration;
using Microsoft.Extensions.Options;
using SmtpServer;
using SmtpServer.Mail;
using SmtpServer.Storage;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MailHub.Services.SmtpService
{
    public class MessageFilter : IMailboxFilter
    {

        private readonly SmtpOptions options;

        public MessageFilter(IOptions<SmtpOptions> options)
        {
            this.options = options.Value;
        }
        public Task<MailboxFilterResult> CanAcceptFromAsync(ISessionContext context, IMailbox @from, int size, CancellationToken token)
        {
            if (options.AllowedDomains.Any(x => x.Equals(from.Host)))
            {
                return Task.FromResult(MailboxFilterResult.Yes);
            }

            return Task.FromResult(MailboxFilterResult.NoPermanently);
        }

        public Task<MailboxFilterResult> CanDeliverToAsync(ISessionContext context, IMailbox to, IMailbox @from, CancellationToken token)
        {
            return Task.FromResult(MailboxFilterResult.Yes);
        }
    }
}
