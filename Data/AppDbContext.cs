using Microsoft.EntityFrameworkCore;
using MyApp.Models;

namespace MyApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Example DbSet for a User entity
        public DbSet<UserProfile> Users { get; set; }
        public DbSet<UserFiles> UserFiles { get; set; }
        public DbSet<UserInterestedProfiles> UserInterests { get; set; }
        public DbSet<MetadataOption> MetadataOptions { get; set; }

        // Add other DbSets as needed
        // public DbSet<Profile> Profiles { get; set; }
        // public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MetadataOption>(entity =>
            {
                entity.HasIndex(option => new { option.Category, option.Name, option.ParentId }).IsUnique();

                entity.HasOne(option => option.Parent)
                    .WithMany(option => option.Children)
                    .HasForeignKey(option => option.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure entity relationships and constraints here
            // modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        }
    }

}