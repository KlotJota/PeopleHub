using PeopleHub.Web.Models;

namespace PeopleHub.Web.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.People.Any()) return;

        var people = new Person[]
        {
            new Person { Name = "Jonathan Garcia", Cpf = "12345678901", BirthDate = new DateTime(2002, 8, 30), Email = "jonathan@peoplehub.com" },
            new Person { Name = "Admin Hub", Cpf = "98765432100", BirthDate = new DateTime(1985, 5, 20), Email = "admin@peoplehub.com" }
        };

        context.People.AddRange(people);
        context.SaveChanges();
    }
}