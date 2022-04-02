using Database.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MailHub.Services.MailService.Configuration;
using MailHub.Services.CleanupService.Configuration;

namespace Tests
{
    public class TestStartup 
    {
        private readonly IConfiguration _configuration;

        public TestStartup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabase(_configuration);
            services.AddSmtpService(_configuration);
            services.AddMessageService(_configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) { }

    }
}