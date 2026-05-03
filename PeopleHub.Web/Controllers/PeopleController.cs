using Microsoft.AspNetCore.Mvc;
using PeopleHub.Application.People.DTOs;
using PeopleHub.Application.People.Results;
using PeopleHub.Application.People.Services;

namespace PeopleHub.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController : Controller
{
    private readonly IPersonService _personService;

    public PeopleController(IPersonService personService)
    {
        _personService = personService;
    }

    [HttpGet("/People")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (items, totalCount) = await _personService.GetAllActiveAsync(search, page, pageSize);

        return Ok(new { items, totalCount });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var person = await _personService.GetPersonByIdAsync(id);
        if (person == null) return NotFound();

        return Ok(person);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PersonCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _personService.CreatePersonAsync(dto);
        if (!result.Success) return ToActionResult(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Person!.Id }, result.Person);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PersonUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _personService.UpdatePersonAsync(id, dto);
        if (!result.Success) return ToActionResult(result);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _personService.DeletePersonAsync(id);
        if (!success) return NotFound();

        return NoContent();
    }

    private IActionResult ToActionResult(PersonOperationResult result) =>
        result.Status switch
        {
            PersonOperationStatus.Conflict => Conflict(result.Message),
            PersonOperationStatus.NotFound => NotFound(result.Message),
            PersonOperationStatus.ValidationError => BadRequest(result.Message),
            _ => BadRequest(result.Message)
        };
}
