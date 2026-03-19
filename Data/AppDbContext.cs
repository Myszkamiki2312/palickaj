using Microsoft.EntityFrameworkCore;
using ErpSupportDesk.Models;

namespace ErpSupportDesk.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ServiceTicket> ServiceTickets => Set<ServiceTicket>();
    public DbSet<IntegrationFlow> IntegrationFlows => Set<IntegrationFlow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>()
            .HasMany(client => client.Tickets)
            .WithOne(ticket => ticket.Client)
            .HasForeignKey(ticket => ticket.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Client>()
            .HasMany(client => client.Integrations)
            .WithOne(integration => integration.Client)
            .HasForeignKey(integration => integration.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ServiceTicket>()
            .HasIndex(ticket => ticket.Number)
            .IsUnique();

        modelBuilder.Entity<ServiceTicket>()
            .Property(ticket => ticket.PlannedHours)
            .HasPrecision(10, 2);

        modelBuilder.Entity<ServiceTicket>()
            .Property(ticket => ticket.SpentHours)
            .HasPrecision(10, 2);

        modelBuilder.Entity<IntegrationFlow>()
            .Property(integration => integration.SuccessRate)
            .HasPrecision(5, 2);
    }
}
