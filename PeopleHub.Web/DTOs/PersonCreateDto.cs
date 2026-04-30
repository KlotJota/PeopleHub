namespace PeopleHub.Web.DTOs;

public class PersonCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string? Email { get; set; }
}