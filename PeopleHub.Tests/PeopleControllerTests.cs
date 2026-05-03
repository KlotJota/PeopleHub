using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;
using PeopleHub.Web.Controllers;
using PeopleHub.Web.Services;
using PeopleHub.Web.DTOs;
using PeopleHub.Web.Models;

namespace PeopleHub.Tests;

public class PeopleControllerTests
{
    private readonly Mock<IPersonService> _serviceMock;
    private readonly PeopleController _controller;

    public PeopleControllerTests()
    {
        _serviceMock = new Mock<IPersonService>();
        _controller = new PeopleController(_serviceMock.Object);
    }

    [Fact]
    public async Task Get_ReturnsOk_WithPaginatedItems()
    {
        // Arrange
        var mockItems = new List<Person> { new Person { Id = 1, Name = "Dev Jonathan" } };
        _serviceMock.Setup(s => s.GetAllActiveAsync(null, 1, 10))
                    .ReturnsAsync((mockItems, 1));

        // Act
        var result = await _controller.Get(null, 1, 10);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

        // Usando Reflection para acessar o objeto anônimo de forma segura entre Assemblies
        var items = okResult.Value?.GetType().GetProperty("items")?.GetValue(okResult.Value) as IEnumerable<Person>;
        var totalCount = (int?)okResult.Value?.GetType().GetProperty("totalCount")?.GetValue(okResult.Value);

        items.Should().NotBeNull();
        items.Should().HaveCount(1);
        totalCount.Should().Be(1);
        items!.First().Name.Should().Be("Dev Jonathan");
    }

    [Fact]
    public async Task Create_ReturnsConflict_WhenServiceReportsDuplicate()
    {
        // Arrange
        var dto = new PersonCreateDto { Name = "Duplicate" };
        _serviceMock.Setup(s => s.CreatePersonAsync(dto))
                    .ReturnsAsync((false, "Este CPF já está cadastrado.", null));

        // Act
        var result = await _controller.Create(dto);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenPersonDoesNotExist()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeletePersonAsync(99)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(99);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}