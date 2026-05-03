using Microsoft.EntityFrameworkCore;
using PeopleHub.Web.Data;
using PeopleHub.Web.Models;

namespace PeopleHub.Web.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly AppDbContext _context;

    public PersonRepository(AppDbContext context)
    {
        _context = context;
    }

    // AsNoTracking evita conflitos de cache
    public async Task<(IEnumerable<Person> Items, int TotalCount)> GetAllAsync(string? search = null, int page = 1, int pageSize = 10)
    {
        var query = _context.People
            .AsNoTracking()
            .Where(p => !p.Archived);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search) ||
                                     p.Cpf.Contains(search) ||
                                     (p.Email != null && p.Email.Contains(search)));
        }

        var totalCount = await query.CountAsync(); // Precisamos do total para o front saber quantas páginas existem
        var items = await query
            .OrderBy(p => p.Name) // Importante: Skip/Take exige ordenação
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Person?> GetByIdAsync(int id) =>
        await _context.People
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Person?> GetByCpfAsync(string cpf) =>
        await _context.People
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Cpf == cpf);

    public async Task<Person?> GetByEmailAsync(string email) =>
        await _context.People
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Email == email);

    public async Task AddAsync(Person person)
    {
        await _context.People.AddAsync(person);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Person person)
    {
        _context.Entry(person).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var person = await GetByIdAsync(id);
        if (person != null)
        {
            person.Archived = true;
            _context.Entry(person).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}