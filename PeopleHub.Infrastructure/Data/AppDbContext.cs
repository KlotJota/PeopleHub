using Microsoft.EntityFrameworkCore;
using PeopleHub.Domain.People;

namespace PeopleHub.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Person> People { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Person>()
            .HasIndex(p => p.Cpf)
            .IsUnique();

        modelBuilder.Entity<Person>()
            .HasIndex(p => p.Email)
            .IsUnique();
    }
}
