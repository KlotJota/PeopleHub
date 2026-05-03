namespace PeopleHub.Web.DTOs;
using System.ComponentModel.DataAnnotations;

public class PersonCreateDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "CPF is required")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF must contain 11 digits")]
    public string Cpf { get; set; } = string.Empty;

    [Required(ErrorMessage = "BirthDate is required")]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid e-mail")]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;
}
