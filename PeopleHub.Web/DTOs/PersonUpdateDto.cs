namespace PeopleHub.Web.Models;

public class PersonUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string? Email { get; set; }
}