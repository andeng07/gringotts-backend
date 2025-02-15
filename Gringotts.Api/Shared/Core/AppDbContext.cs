using Gringotts.Api.Features.Client.Models;
using Gringotts.Api.Features.ClientAuthentication.Models;
using Gringotts.Api.Features.Interactions.Models;
using Gringotts.Api.Features.Reader.Models;
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
        
        public DbSet<InteractionLog> InteractionLogs { get; set; }
        public DbSet<ActiveSession> ActiveSessions { get; set; }
        public DbSet<Session> Sessions { get; set; }
        
        public DbSet<Location> Locations { get; set; }
        public DbSet<Reader> Readers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Client>().HasKey(client => client.Id);

            modelBuilder.Entity<ClientSecret>().HasKey(clientSecret => clientSecret.Id);
            modelBuilder.Entity<ClientSecret>()
                .HasOne<Client>()
                .WithOne()
                .HasForeignKey<ClientSecret>(clientSecret => clientSecret.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ClientSecret>()
                .HasIndex(clientSecret => clientSecret.Username)
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

            modelBuilder.Entity<InteractionLog>().HasKey(interactionLog => interactionLog.Id);
            modelBuilder.Entity<InteractionLog>()
                .HasOne<Reader>()
                .WithMany()
                .HasForeignKey(interactionLog => interactionLog.LogReaderId);
            modelBuilder.Entity<InteractionLog>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(interactionLog => interactionLog.LogUserId);
            
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
        }
    }
}