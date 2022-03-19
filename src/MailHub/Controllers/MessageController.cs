using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MailHub.Services.MessageService;

namespace MailHub.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger<MessagesController> logger;
        private readonly MessageService messageService;

        public MessagesController(ILogger<MessagesController> logger, MessageService messageService)
        {
            this.logger = logger;
            this.messageService = messageService;
        }

        [HttpGet("get-messages-by-author")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAuthor(string address, string subject)
        {
            var messages = await messageService.GetBasedOnAuthorAsync(address, subject);
            return Ok(messages);
        }

        [HttpGet("get-messages-by-recipient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByRecipient(string address, string subject)
        {
            var messages = await messageService.GetBasedOnRecipientAsync(address, subject);
            return Ok(messages);
        }

        [HttpDelete("cleanup-mailbox")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Cleanup(string mailbox)
        {
            await messageService.DeleteMessagesAsync(mailbox);
            return Ok();
        }

#if DEBUG
        [HttpGet("get-all-messages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var messages = await messageService.GetAllMessagesAsync();
            return Ok(messages);
        }
#endif
    }
}
