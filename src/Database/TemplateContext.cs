using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public sealed class MailHubContext : DbContext
    {
        public MailHubContext(DbContextOptions<MailHubContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        //dotnet ef migrations add --startup-project MailHub --project Database Initial
        public DbSet<MessageEntity> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageEntity>()
            .HasMany(e => e.Attachments)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
        }

    }

   


}