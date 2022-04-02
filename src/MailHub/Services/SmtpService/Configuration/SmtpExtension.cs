using MailHub.Services.SmtpService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmtpServer;
using SmtpServer.Storage;
using System;

namespace MailHub.Services.MailService.Configuration
{
    public static class SmtpExtension
    {
        public static void AddSmtpService(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection(nameof(SmtpOptions));
            services.Configure<SmtpOptions>(options);
           
            services.AddSingleton(x =>
            {
                var options = configuration.GetSection(nameof(SmtpOptions)).Get<SmtpOptions>();
                var builder = new SmtpServerOptionsBuilder()
                .ServerName(options.ServerName)
                .Port(25, 587);

                return new SmtpServer.SmtpServer(builder.Build(), x.GetRequiredService<IServiceProvider>());
            });

            services.AddTransient<IMessageStore, MessageStorage>();
            services.AddTransient<IMailboxFilter, MessageFilter>();

            services.AddHostedService<SmtpService>();
        }
    }
}
