using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MailHub.Services.CleanupService.Configuration
{
    public static class SmtpExtension
    {
        public static void AddBackgroundCleanup(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection(nameof(CleanupOptions));
            services.Configure<CleanupOptions>(options);

            services.AddHostedService<CleanupService>();
        }
    }
}
