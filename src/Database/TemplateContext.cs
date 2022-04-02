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