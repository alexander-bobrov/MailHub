using Microsoft.Extensions.Logging;
using SmtpServer;
using SmtpServer.Mail;
using SmtpServer.Storage;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MailHub.Services.SmtpService
{
    public class MessageFilter : IMailboxFilter
    {
        private readonly string[] allowedDomains;
        private readonly ILogger<MessageFilter> logger;

        public MessageFilter(string[] allowedDomains, ILogger<MessageFilter> logger)
        {
            this.allowedDomains = allowedDomains;
            this.logger = logger;
        }
        public Task<MailboxFilterResult> CanAcceptFromAsync(ISessionContext context, IMailbox @from, int size, CancellationToken token)
        {
            if (allowedDomains.Any(x => x.Equals(from.Host)))
            {              
                return Task.FromResult(MailboxFilterResult.Yes);
            }

            logger.LogInformation($"Message from {from.User} has been declined");
            return Task.FromResult(MailboxFilterResult.NoPermanently);
        }

        public Task<MailboxFilterResult> CanDeliverToAsync(ISessionContext context, IMailbox to, IMailbox @from, CancellationToken token)
        {
            return Task.FromResult(MailboxFilterResult.Yes);
        }

        public IMailboxFilter CreateInstance(ISessionContext context)
        {
            return new MessageFilter(allowedDomains);
        }
    }
}
