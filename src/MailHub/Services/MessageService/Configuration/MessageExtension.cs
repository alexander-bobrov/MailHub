using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MailHub.Services.CleanupService.Configuration
{
    public static class MessageExtension
    {
        public static void UseMessageService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<MessageService.MessageService>();
        }
    }
}
