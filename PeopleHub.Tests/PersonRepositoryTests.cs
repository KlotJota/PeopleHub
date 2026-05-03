using Microsoft.EntityFrameworkCore;
using PeopleHub.Domain.People;
using PeopleHub.Infrastructure.Data;
using PeopleHub.Infrastructure.Repositories;
using Xunit;

namespace PeopleHub.Tests;

public class PersonRepositoryTests
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _dbName)
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ShouldNotReturnArchivedPeople()
    {
        var context = GetDbContext();
        context.People.AddRange(new List<Person>
        {
            new Person { Id = 1, Name = "Ativo", Cpf = "1", Email = "ativo@teste.com", Archived = false },
            new Person { Id = 2, Name = "Arquivado", Cpf = "2", Email = "arquivado@teste.com", Archived = true }
        });
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var repository = new PersonRepository(context);

        var (items, totalCount) = await repository.GetAllAsync();

        Assert.Single(items);
        Assert.Equal(1, totalCount);
        Assert.All(items, p => Assert.False(p.Archived));
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetArchivedToTrue()
    {
        var context = GetDbContext();
        var person = new Person { Id = 1, Name = "Jonathan", Cpf = "123", Email = "jonathan@teste.com", Archived = false };
        context.People.Add(person);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var repository = new PersonRepository(context);

        await repository.DeleteAsync(1);

        var verifyContext = GetDbContext();
        var updatedPerson = await verifyContext.People.AsNoTracking().FirstOrDefaultAsync(p => p.Id == 1);

        Assert.NotNull(updatedPerson);
        Assert.True(updatedPerson.Archived);
    }
}
