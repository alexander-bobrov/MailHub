using Database.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MailHub.Services.CleanupService.Configuration;
using MailHub.Services.MailService.Configuration;
using MailHub.Configuration;

namespace MailHub
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabase(_configuration);          
            services.UseBackgroundCleanup(_configuration);
            services.UseSmtpService(_configuration);
            services.UseMessageService(_configuration);

            services.AddControllers();
            services.AddSwagger();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MailHub v1"));
            
            app.UseHttpsRedirection();

            app.UseRouting();         
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}