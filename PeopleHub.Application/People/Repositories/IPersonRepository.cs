using PeopleHub.Domain.People;

namespace PeopleHub.Application.People.Repositories;

public interface IPersonRepository
{
    Task<(IEnumerable<Person> Items, int TotalCount)> GetAllAsync(string? search = null, int page = 1, int pageSize = 10);
    Task<Person?> GetByIdAsync(int id);
    Task<Person?> GetByCpfAsync(string cpf);
    Task<Person?> GetByEmailAsync(string email);
    Task AddAsync(Person person);
    Task UpdateAsync(Person person);
    Task DeleteAsync(int id);
}
