using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MailHub.Services.CleanupService.Configuration
{
    public static class MessageExtension
    {
        public static void AddMessageService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<MessageService.MessageService>();
        }
    }
}
