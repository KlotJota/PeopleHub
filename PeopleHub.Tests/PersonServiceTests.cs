using Moq;
using Xunit;
using PeopleHub.Web.Services;
using PeopleHub.Web.Repositories;
using PeopleHub.Web.Models;
using PeopleHub.Web.DTOs;

namespace PeopleHub.Tests;

public class PersonServiceTests
{
    [Fact]
    public async Task CreatePerson_DeveFalhar_SeMenorDe18Anos()
    {
        // Arrange
        var repoMock = new Mock<IPersonRepository>();
        var service = new PersonService(repoMock.Object);
        var dto = new PersonCreateDto { BirthDate = DateTime.Now.AddYears(-17), Cpf = "000", Email = "teste@teste.com" };

        // Act
        var result = await service.CreatePersonAsync(dto);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("no mínimo 18 anos", result.Message);
    }

    [Fact]
    public async Task CreatePerson_DeveFalhar_SeCpfJaExiste()
    {
        // Arrange
        var repoMock = new Mock<IPersonRepository>();
        repoMock.Setup(r => r.GetByCpfAsync("123")).ReturnsAsync(new Person { Id = 1, Cpf = "123" });

        var service = new PersonService(repoMock.Object);
        var dto = new PersonCreateDto { Cpf = "123", BirthDate = DateTime.Now.AddYears(-20) };

        // Act
        var result = await service.CreatePersonAsync(dto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Este CPF já está cadastrado.", result.Message);
    }
}