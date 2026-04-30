using Microsoft.EntityFrameworkCore;
using PeopleHub.Web.Data;
using PeopleHub.Web.Models;
using PeopleHub.Web.Repositories;
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
            new Person { Id = 1, Name = "Ativo", Cpf = "1", Archived = false },
            new Person { Id = 2, Name = "Arquivado", Cpf = "2", Archived = true }
        });
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var repository = new PersonRepository(context);

        var result = await repository.GetAllAsync();

        Assert.Single(result);
        Assert.All(result, p => Assert.False(p.Archived));
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetArchivedToTrue()
    {
        var context = GetDbContext();
        var person = new Person { Id = 1, Name = "Jonathan", Cpf = "123", Archived = false };
        context.People.Add(person);
        await context.SaveChangesAsync();

        // limpa o rastreamento antes de iniciar a ação do teste pra evitar erro de "already being tracked".
        context.ChangeTracker.Clear();

        var repository = new PersonRepository(context);

        await repository.DeleteAsync(1);

        var verifyContext = GetDbContext();
        var updatedPerson = await verifyContext.People.AsNoTracking().FirstOrDefaultAsync(p => p.Id == 1);

        Assert.NotNull(updatedPerson);
        Assert.True(updatedPerson.Archived);
    }
}