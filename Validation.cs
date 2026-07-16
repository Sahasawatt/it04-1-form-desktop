using System.Text.RegularExpressions;

namespace IT04Form;

/// <summary>Field-keyed validation for a <see cref="PersonInput"/>.</summary>
public static class Validation
{
    private static readonly Regex EmailRegex =
        new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled);

    private static readonly Regex PhoneRegex =
        new(@"^\+?\d{9,15}$", RegexOptions.Compiled);

    /// <summary>Returns field-keyed error messages (keys = Constants.Fields.*, values = Constants.Messages.*).
    /// An empty dictionary means the input is valid.</summary>
    public static Dictionary<string, string> Validate(PersonInput input)
    {
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(input.FirstName))
            errors[Constants.Fields.FirstName] = Constants.Messages.FirstName;

        if (string.IsNullOrWhiteSpace(input.LastName))
            errors[Constants.Fields.LastName] = Constants.Messages.LastName;

        if (!IsValidEmail(input.Email))
            errors[Constants.Fields.Email] = Constants.Messages.Email;

        if (!IsValidPhone(input.Phone))
            errors[Constants.Fields.Phone] = Constants.Messages.Phone;

        if (!IsValidBirthDay(input.BirthDay))
            errors[Constants.Fields.BirthDay] = Constants.Messages.BirthDay;

        if (!Constants.Occupations.Contains(input.Occupation))
            errors[Constants.Fields.Occupation] = Constants.Messages.Occupation;

        if (!Constants.Sexes.Contains(input.Sex))
            errors[Constants.Fields.Sex] = Constants.Messages.Sex;

        // "data:image/..." prefix check already implies non-empty (an empty
        // string can never start with a non-empty prefix).
        var profile = input.ProfileBase64 ?? "";
        if (!profile.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase))
            errors[Constants.Fields.ProfileBase64] = Constants.Messages.Profile;

        return errors;
    }

    public static bool IsValidEmail(string email) => EmailRegex.IsMatch(email ?? "");

    public static bool IsValidPhone(string phone)
    {
        var cleaned = (phone ?? "").Replace(" ", "").Replace("-", "");
        return PhoneRegex.IsMatch(cleaned);
    }

    public static bool IsValidBirthDay(DateTime? birthDay)
    {
        if (birthDay is null) return false;
        var date = birthDay.Value.Date;
        return date >= new DateTime(1900, 1, 1) && date <= DateTime.Today;
    }
}
