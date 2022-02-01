using System.Threading.Tasks;
using MailHub.Services.MessageService.Models;

namespace MailHub.Services.MessageService
{
    public interface IMessageService
    {
        Task<Message[]> GetBasedOnAuthor(string authorPart);

        Task<Message[]> GetBasedOnRecipient(string recipientPart);

        Task<Message[]> GetAll();
    }
}
