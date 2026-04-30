using PeopleHub.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using PeopleHub.Web.Models;
using PeopleHub.Web.Repositories;

namespace PeopleHub.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController : Controller
{
    private readonly IPersonRepository _repository;

    public PeopleController(IPersonRepository repository)
    {
        _repository = repository;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _repository.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var person = await _repository.GetByIdAsync(id);
        if (person == null) return NotFound();
        return Ok(person);
    }

    [HttpGet("cpf/{cpf}")]
    public async Task<IActionResult> GetByCpf(string cpf)
    {
        var person = await _repository.GetByCpfAsync(cpf);
        if (person == null) return NotFound();
        return Ok(person);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PersonCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existing = await _repository.GetByCpfAsync(dto.Cpf);
        if (existing != null) return BadRequest("CPF already registered.");

        var person = new Person
        {
            Name = dto.Name,
            Cpf = dto.Cpf,
            BirthDate = dto.BirthDate,
            Email = dto.Email,
            Archived = false
        };

        await _repository.AddAsync(person);
        return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PersonUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Busca a pessoa original (sem rastreamento para evitar conflito)
        var person = await _repository.GetByIdAsync(id);
        if (person == null) return NotFound();

        // Atualiza apenas os campos permitidos do DTO para a Entidade
        person.Name = dto.Name;
        person.Cpf = dto.Cpf;
        person.BirthDate = dto.BirthDate;
        person.Email = dto.Email;
        // person.Archived não é tocado aqui, preservando o estado original

        await _repository.UpdateAsync(person);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var person = await _repository.GetByIdAsync(id);
        if (person == null) return NotFound();

        person.Archived = true;
        await _repository.UpdateAsync(person);

        return NoContent();
    }
}