using Gringotts.Api.Features.Client.Models;
using Gringotts.Api.Features.ClientAuthentication.Models;
using Gringotts.Api.Features.Reader.Models;
using Gringotts.Api.Features.Sessions.Models;
using Gringotts.Api.Features.Statistics.Models;
using Gringotts.Api.Features.User.Models;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Shared.Core
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientSecret> ClientSecrets { get; set; }
        
        public DbSet<Department> Departments { get; set; }
        
        public DbSet<User> LogUsers { get; set; }
        
        public DbSet<SessionLog> SessionLogs { get; set; }
        public DbSet<ActiveSession> ActiveSessions { get; set; }
        public DbSet<Session> Sessions { get; set; }
        
        public DbSet<Location> Locations { get; set; }
        public DbSet<Reader> Readers { get; set; }

        public DbSet<ReaderAnalytics> LogReaderAnalyticsSet { get; set; }
        public DbSet<UserAnalytics> LogUserAnalyticsSet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Client>().HasKey(managementUser => managementUser.Id);

            modelBuilder.Entity<ClientSecret>().HasKey(managementUserSecret => managementUserSecret.Id);
            modelBuilder.Entity<ClientSecret>()
                .HasOne<Client>()
                .WithOne()
                .HasForeignKey<ClientSecret>(managementUserSecret => managementUserSecret.ManagementUserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ClientSecret>()
                .HasIndex(managementUserSecret => managementUserSecret.Username)
                .IsUnique();

            modelBuilder.Entity<Department>().HasKey(department => department.Id);

            modelBuilder.Entity<User>().HasKey(logUser => logUser.Id);
            modelBuilder.Entity<User>()
                .HasOne<Department>()
                .WithMany()
                .HasForeignKey(logUser => logUser.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Location>().HasKey(location => location.Id);
            
            modelBuilder.Entity<Reader>().HasKey(logReader => logReader.Id);
            modelBuilder.Entity<Reader>()
                .HasOne<Location>()
                .WithMany()
                .HasForeignKey(logReader => logReader.LocationId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SessionLog>().HasKey(sessionLog => sessionLog.Id);
            modelBuilder.Entity<SessionLog>()
                .HasOne<Reader>()
                .WithMany()
                .HasForeignKey(sessionLog => sessionLog.LogReaderId);
            modelBuilder.Entity<SessionLog>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(sessionLog => sessionLog.LogUserId);
            
            modelBuilder.Entity<Session>().HasKey(session => session.Id);
            modelBuilder.Entity<Session>()
                .HasOne<Reader>()
                .WithMany()
                .HasForeignKey(session => session.LogReaderId);
            modelBuilder.Entity<Session>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(session => session.LogUserId);

            modelBuilder.Entity<ActiveSession>().HasKey(activeSession => activeSession.Id);
            modelBuilder.Entity<ActiveSession>()
                .HasOne<Reader>()
                .WithMany()
                .HasForeignKey(activeSession => activeSession.LogReaderId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ActiveSession>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(activeSession => activeSession.LogUserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<ReaderAnalytics>().HasKey(logReaderAnalytics => logReaderAnalytics.Id);
            modelBuilder.Entity<ReaderAnalytics>()
                .HasOne<Reader>()
                .WithOne()
                .HasForeignKey<ReaderAnalytics>(logReaderAnalytics => logReaderAnalytics.ReaderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserAnalytics>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<UserAnalytics>(logUserAnalytics => logUserAnalytics.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}