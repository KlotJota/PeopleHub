using System.ComponentModel.DataAnnotations;

namespace PeopleHub.Web.Models;

public class Person
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3)]
    /// <example>Jonathan Garcia</example>
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "CPF is required")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF must contain 11 digits")]
    public string Cpf { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }

    [EmailAddress(ErrorMessage = "Invalid e-mail")]
    public string? Email { get; set; }

    public bool Archived { get; set; } = false;
}