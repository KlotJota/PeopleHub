using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PeopleHub.Application.People.DTOs;
using PeopleHub.Application.People.Results;
using PeopleHub.Application.People.Services;
using PeopleHub.Domain.People;
using PeopleHub.Web.Controllers;
using Xunit;

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
        var mockItems = new List<Person> { new Person { Id = 1, Name = "Dev Jonathan" } };
        _serviceMock.Setup(s => s.GetAllActiveAsync(null, 1, 10))
            .ReturnsAsync((mockItems, 1));

        var result = await _controller.Get(null, 1, 10);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

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
        var dto = new PersonCreateDto { Name = "Duplicate" };
        _serviceMock.Setup(s => s.CreatePersonAsync(dto))
            .ReturnsAsync(PersonOperationResult.Conflict("Este CPF já está cadastrado."));

        var result = await _controller.Create(dto);

        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenPersonDoesNotExist()
    {
        _serviceMock.Setup(s => s.DeletePersonAsync(99)).ReturnsAsync(false);

        var result = await _controller.Delete(99);

        result.Should().BeOfType<NotFoundResult>();
    }
}
