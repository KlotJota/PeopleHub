using PeopleHub.Web.DTOs;
using PeopleHub.Web.Models;

namespace PeopleHub.Web.Services;

public interface IPersonService
{
    Task<(IEnumerable<Person> Items, int TotalCount)> GetAllActiveAsync(string? search = null, int page = 1, int pageSize = 10);
    Task<Person?> GetPersonByIdAsync(int id);
    Task<PersonOperationResult> CreatePersonAsync(PersonCreateDto dto);
    Task<PersonOperationResult> UpdatePersonAsync(int id, PersonUpdateDto dto);
    Task<bool> DeletePersonAsync(int id);
}
