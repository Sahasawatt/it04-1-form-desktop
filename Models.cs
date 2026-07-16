namespace IT04Form;

/// <summary>Raw values collected from the form, before validation.</summary>
/// <param name="BirthDay">null when the user has not picked a date yet.</param>
/// <param name="ProfileBase64">data URL, e.g. "data:image/png;base64,iVBOR...".</param>
public sealed record PersonInput(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    DateTime? BirthDay,
    string Occupation,
    string Sex,
    string ProfileBase64);

/// <summary>A row of the "persons" table.</summary>
public sealed class Person
{
    public int Id { get; init; }
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Email { get; init; } = "";
    public string Phone { get; init; } = "";
    public DateTime BirthDay { get; init; }
    public string Occupation { get; init; } = "";
    public string Sex { get; init; } = "";
    public string ProfileBase64 { get; init; } = "";
    public DateTime CreatedAt { get; init; }
}
