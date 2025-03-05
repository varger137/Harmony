using Microsoft.EntityFrameworkCore;
using TaskCraft.Entities;

namespace TaskCraft.DataBase
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Channel> Channels { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Channel>(entity =>
            {
                entity.Property(p => p.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()"); 

                entity.Property(p => p.OwnerId)
                    .IsRequired();
            });

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade); 


            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict); 


            modelBuilder.Entity<Channel>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.Channels)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict); 


            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Channel)
                .WithMany(p => p.Chats)
                .HasForeignKey(c => c.ChannelId)
                .OnDelete(DeleteBehavior.Cascade); 


        }
    }
}