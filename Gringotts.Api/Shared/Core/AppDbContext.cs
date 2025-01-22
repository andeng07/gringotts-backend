using Gringotts.Api.Features.Log.Models;
using Gringotts.Api.Features.LogReader.Models;
using Gringotts.Api.Features.LogUser.Models;
using Gringotts.Api.Features.ManagementAuthentication.Models;
using Gringotts.Api.Features.ManagementUser.Models;
using Gringotts.Api.Features.Statistics.Models;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Shared.Core
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<ManagementUser> ManagementUsers { get; set; }
        public DbSet<ManagementUserSecret> ManagementUserSecrets { get; set; }

        public DbSet<Log> Logs { get; set; }
        public DbSet<LogFallback> LogFallbacks { get; set; }
        public DbSet<Session> Sessions { get; set; }

        public DbSet<LogReader> Readers { get; set; }
        public DbSet<Location> Locations { get; set; }

        public DbSet<LogUser> LogUsers { get; set; }

        public DbSet<LogReaderAnalytics> LogReaderAnalyticsSet { get; set; }
        public DbSet<LogUserAnalytics> LogUserAnalyticsSet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ManagementUser>().HasKey(m => m.Id);

            modelBuilder.Entity<ManagementUserSecret>().HasKey(m => m.Id);
            modelBuilder.Entity<ManagementUserSecret>()
                .HasOne<ManagementUser>()
                .WithOne()
                .HasForeignKey<ManagementUserSecret>(m => m.Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LogUser>().HasKey(m => m.Id);

            modelBuilder.Entity<Location>().HasKey(m => m.Id);
            modelBuilder.Entity<LogReader>().HasKey(m => m.Id);
            modelBuilder.Entity<LogReader>()
                .HasOne<Location>()
                .WithMany()
                .HasForeignKey(x => x.LocationId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Log>().HasKey(m => m.Id);
            modelBuilder.Entity<Log>()
                .HasOne<LogReader>()
                .WithMany()
                .HasForeignKey(x => x.LogReaderId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Log>()
                .HasOne<LogUser>()
                .WithMany()
                .HasForeignKey(x => x.LogReaderId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<LogFallback>().HasKey(m => m.Id);
            modelBuilder.Entity<LogFallback>()
                .HasOne<LogReader>()
                .WithMany()
                .HasForeignKey(x => x.LogReaderId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Session>().HasKey(m => m.Id);
            modelBuilder.Entity<Session>()
                .HasOne<LogReader>()
                .WithMany()
                .HasForeignKey(x => x.LogReaderId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Session>()
                .HasOne<LogUser>()
                .WithMany()
                .HasForeignKey(x => x.LogUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LogReaderAnalytics>().HasKey(m => m.Id);
            modelBuilder.Entity<LogReaderAnalytics>()
                .HasOne<LogReader>()
                .WithOne()
                .HasForeignKey<LogReaderAnalytics>(x => x.ReaderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LogUserAnalytics>()
                .HasOne<LogUser>()
                .WithOne()
                .HasForeignKey<LogUserAnalytics>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}