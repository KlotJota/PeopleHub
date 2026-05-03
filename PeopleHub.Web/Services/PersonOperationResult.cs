using PeopleHub.Web.Models;

namespace PeopleHub.Web.Services;

public enum PersonOperationStatus
{
    Success,
    ValidationError,
    Conflict,
    NotFound
}

public sealed record PersonOperationResult(
    PersonOperationStatus Status,
    string Message,
    Person? Person = null)
{
    public bool Success => Status == PersonOperationStatus.Success;

    public static PersonOperationResult Created(Person person) =>
        new(PersonOperationStatus.Success, "Success", person);

    public static PersonOperationResult Updated() =>
        new(PersonOperationStatus.Success, "Success");

    public static PersonOperationResult ValidationError(string message) =>
        new(PersonOperationStatus.ValidationError, message);

    public static PersonOperationResult Conflict(string message) =>
        new(PersonOperationStatus.Conflict, message);

    public static PersonOperationResult NotFound(string message) =>
        new(PersonOperationStatus.NotFound, message);
}
