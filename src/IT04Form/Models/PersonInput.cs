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
