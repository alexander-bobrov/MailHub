using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Database.Configuration
{
    public static class DatabaseExtension
    {
        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection(nameof(DatabaseOptions)).Get<DatabaseOptions>();

            services.AddDbContext<MailHubContext>(x => x.UseSqlite(options.ConnectionString), optionsLifetime: ServiceLifetime.Singleton);
            services.AddDbContextFactory<MailHubContext>(x => x.UseSqlite(options.ConnectionString));
        }
    }
}