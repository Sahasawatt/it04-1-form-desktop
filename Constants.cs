namespace IT04Form;

/// <summary>
/// Single source of truth for the Occupation combo-box options and the
/// validation messages shown in red under each field (per the IT 04-1 mockup).
/// </summary>
public static class Constants
{
    public static readonly string[] Occupations =
    {
        "Software Engineer",
        "Teacher",
        "Doctor",
        "Nurse",
        "Accountant",
        "Civil Servant",
        "Business Owner",
        "Student",
        "Other",
    };

    public static readonly string[] Sexes = { "Male", "Female" };

    public static class Messages
    {
        public const string FirstName = "Please provide a First Name";
        public const string LastName = "Please provide a Last Name";
        public const string Email = "Please provide a valid Email";
        public const string Phone = "Please provide a valid Phone";
        public const string BirthDay = "Please provide a valid Birth Day";
        public const string Profile = "Please select a valid profile";
        public const string Occupation = "Please select an Occupation";
        public const string Sex = "Please select Sex";
        public const string SaveSuccess = "save data success";
    }

    /// <summary>Field keys used by Validation errors and by the form's error labels.</summary>
    public static class Fields
    {
        public const string FirstName = "FirstName";
        public const string LastName = "LastName";
        public const string Email = "Email";
        public const string Phone = "Phone";
        public const string BirthDay = "BirthDay";
        public const string Occupation = "Occupation";
        public const string Sex = "Sex";
        public const string ProfileBase64 = "ProfileBase64";
    }
}
