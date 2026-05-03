using PeopleHub.Application.People.DTOs;
using PeopleHub.Application.People.Results;
using PeopleHub.Domain.People;

namespace PeopleHub.Application.People.Services;

public interface IPersonService
{
    Task<(IEnumerable<Person> Items, int TotalCount)> GetAllActiveAsync(string? search = null, int page = 1, int pageSize = 10);
    Task<Person?> GetPersonByIdAsync(int id);
    Task<PersonOperationResult> CreatePersonAsync(PersonCreateDto dto);
    Task<PersonOperationResult> UpdatePersonAsync(int id, PersonUpdateDto dto);
    Task<bool> DeletePersonAsync(int id);
}
