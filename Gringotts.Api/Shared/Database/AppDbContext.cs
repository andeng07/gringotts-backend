using Gringotts.Api.Shared.Database.Models.Readers;
using Gringotts.Api.Shared.Database.Models.Records;
using Gringotts.Api.Shared.Database.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Shared.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserSecret> UserSecrets { get; set; }
        public DbSet<UserAnalytics> UserAnalytics { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Reader> Readers { get; set; }
        public DbSet<ReaderSecret> ReaderSecrets { get; set; }
        public DbSet<ReaderAnalytics> ReaderAnalytics { get; set; }
        public DbSet<Location> Locations { get; set; }

        public DbSet<Record> Records { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(entity => entity.Id);

            modelBuilder.Entity<UserSecret>()
                .HasKey(entity => entity.Id);

            modelBuilder.Entity<UserSecret>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<UserSecret>(entity => entity.UserId);

            modelBuilder.Entity<UserAnalytics>()
                .HasKey(entity => entity.Id);

            modelBuilder.Entity<UserAnalytics>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<UserAnalytics>(entity => entity.UserId);

            modelBuilder.Entity<Role>()
                .HasKey(entity => entity.Id);

            modelBuilder.Entity<UserRole>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(entity => entity.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne<Role>()
                .WithMany()
                .HasForeignKey(entity => entity.RoleId);

            modelBuilder.Entity<Reader>()
                .HasKey(entity => entity.Id);

            modelBuilder.Entity<Reader>()
                .HasOne<Location>()
                .WithMany()
                .HasForeignKey(entity => entity.LocationId);        

            modelBuilder.Entity<ReaderSecret>()
                .HasKey(entity => entity.Id);

            modelBuilder.Entity<ReaderSecret>()
                .HasOne<Reader>()
                .WithOne()
                .HasForeignKey<ReaderSecret>(entity => entity.ReaderId);

            modelBuilder.Entity<ReaderAnalytics>()
                .HasKey(entity => entity.Id);

            modelBuilder.Entity<ReaderAnalytics>()
                .HasOne<Reader>()
                .WithOne()
                .HasForeignKey<ReaderAnalytics>(entity => entity.ReaderId);

            modelBuilder.Entity<Location>()
                .HasKey(entity => entity.Id);

            modelBuilder.Entity<Record>()
                .HasKey(entity => entity.Id);

            modelBuilder.Entity<Record>()
                .HasOne<Reader>()
                .WithMany()
                .HasForeignKey(entity => entity.ReaderId);

            modelBuilder.Entity<Record>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(entity => entity.UserId);

            modelBuilder.Entity<Record>()
                .Property(entity => entity.ActionType)
                .HasConversion<int>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}