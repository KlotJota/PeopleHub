using PeopleHub.Web.Models;      // Para achar a classe Person
using PeopleHub.Web.DTOs;        // Para achar PersonCreateDto e PersonUpdateDto
using PeopleHub.Web.Repositories;

namespace PeopleHub.Web.Services;

public class PersonService : IPersonService
{
    private readonly IPersonRepository _repository;

    public PersonService(IPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<(IEnumerable<Person> Items, int TotalCount)> GetAllActiveAsync(string? search = null, int page = 1, int pageSize = 10)
    {
        return await _repository.GetAllAsync(search, page, pageSize);
    }

    public async Task<Person?> GetPersonByIdAsync(int id) => await _repository.GetByIdAsync(id);

    public async Task<Person?> GetPersonByCpfAsync(string cpf) => await _repository.GetByCpfAsync(cpf);

    public async Task<(bool Success, string Message, Person? Person)> CreatePersonAsync(PersonCreateDto dto)
    {
        var validation = await ValidatePersonDataAsync(null, dto.Email, dto.Cpf, dto.BirthDate);
        if (!validation.Success) return (false, validation.Message, null);

        var person = new Person
        {
            Name = dto.Name,
            Cpf = dto.Cpf,
            BirthDate = dto.BirthDate,
            Email = dto.Email,
            Archived = false
        };

        await _repository.AddAsync(person);
        return (true, "Success", person);
    }

    public async Task<(bool Success, string Message)> UpdatePersonAsync(int id, PersonUpdateDto dto)
    {
        var person = await _repository.GetByIdAsync(id);
        if (person == null) return (false, "Pessoa não encontrada.");

        var validation = await ValidatePersonDataAsync(id, dto.Email, dto.Cpf, dto.BirthDate);
        if (!validation.Success) return (false, validation.Message);

        person.Name = dto.Name;
        person.Cpf = dto.Cpf;
        person.BirthDate = dto.BirthDate;
        person.Email = dto.Email;

        await _repository.UpdateAsync(person);
        return (true, "Success");
    }

    public async Task<bool> DeletePersonAsync(int id)
    {
        var person = await _repository.GetByIdAsync(id);
        if (person == null) return false;

        person.Archived = true;
        await _repository.UpdateAsync(person);
        return true;
    }

    private async Task<(bool Success, string Message)> ValidatePersonDataAsync(int? id, string? email, string cpf, DateTime birthDate)
    {
        if (birthDate > DateTime.Now.AddYears(-18))
            return (false, "A pessoa deve ter no mínimo 18 anos.");

        if (!string.IsNullOrEmpty(email))
        {
            var personWithEmail = await _repository.GetByEmailAsync(email);
            if (personWithEmail != null && personWithEmail.Id != id)
                return (false, "Este e-mail já está cadastrado.");
        }

        var personWithCpf = await _repository.GetByCpfAsync(cpf);
        if (personWithCpf != null && personWithCpf.Id != id)
            return (false, "Este CPF já está cadastrado.");

        return (true, "Success");
    }
}