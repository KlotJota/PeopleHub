using Moq;
using PeopleHub.Web.DTOs;
using PeopleHub.Web.Models;
using PeopleHub.Web.Repositories;
using PeopleHub.Web.Services;
using Xunit;

namespace PeopleHub.Tests;

public class PersonServiceTests
{
    [Fact]
    public async Task CreatePerson_DeveFalhar_SeMenorDe18Anos()
    {
        var repoMock = new Mock<IPersonRepository>();
        var service = new PersonService(repoMock.Object);
        var dto = new PersonCreateDto { BirthDate = DateTime.Today.AddYears(-17), Cpf = "000", Email = "teste@teste.com" };

        var result = await service.CreatePersonAsync(dto);

        Assert.False(result.Success);
        Assert.Equal(PersonOperationStatus.ValidationError, result.Status);
        Assert.Contains("mínimo 18 anos", result.Message);
    }

    [Fact]
    public async Task CreatePerson_DeveFalhar_SeCpfJaExiste()
    {
        var repoMock = new Mock<IPersonRepository>();
        repoMock.Setup(r => r.GetByCpfAsync("123")).ReturnsAsync(new Person { Id = 1, Cpf = "123" });

        var service = new PersonService(repoMock.Object);
        var dto = new PersonCreateDto { Cpf = "123", BirthDate = DateTime.Today.AddYears(-20) };

        var result = await service.CreatePersonAsync(dto);

        Assert.False(result.Success);
        Assert.Equal(PersonOperationStatus.Conflict, result.Status);
        Assert.Equal("Este CPF já está cadastrado.", result.Message);
    }
}
