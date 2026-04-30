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
    public async Task<IEnumerable<Person>> GetAllAsync() =>
        await _context.People
            .AsNoTracking()
            .Where(p => p.Archived == false)
            .ToListAsync();

    public async Task<Person?> GetByIdAsync(int id) =>
        await _context.People
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Person?> GetByCpfAsync(string cpf) =>
        await _context.People
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Cpf == cpf);

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