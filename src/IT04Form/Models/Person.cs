namespace IT04Form;

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
