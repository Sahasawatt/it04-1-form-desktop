using System.Globalization;
using Microsoft.Data.Sqlite;

namespace IT04Form;

/// <summary>Local SQLite persistence for <see cref="Person"/> rows. No server — one file under LocalAppData.</summary>
public static class Db
{
    public static string DbPath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "IT04-1Form",
        "it04.db");

    private static string ConnectionString =>
        new SqliteConnectionStringBuilder { DataSource = DbPath }.ToString();

    /// <summary>Creates the LocalAppData folder and the persons table if either is missing.</summary>
    public static void Init()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(DbPath)!);

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
CREATE TABLE IF NOT EXISTS persons (
  id            INTEGER PRIMARY KEY AUTOINCREMENT,
  firstName     TEXT NOT NULL,
  lastName      TEXT NOT NULL,
  email         TEXT NOT NULL,
  phone         TEXT NOT NULL,
  birthDay      TEXT NOT NULL,
  occupation    TEXT NOT NULL,
  sex           TEXT NOT NULL,
  profileBase64 TEXT NOT NULL,
  createdAt     TEXT NOT NULL
);";
        command.ExecuteNonQuery();
    }

    /// <summary>Inserts an already-validated input and returns the DB-generated id.</summary>
    public static int Insert(PersonInput input)
    {
        if (input.BirthDay is null)
            throw new ArgumentException("BirthDay must not be null.", nameof(input));

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using (var insertCommand = connection.CreateCommand())
        {
            insertCommand.CommandText = @"
INSERT INTO persons (firstName, lastName, email, phone, birthDay, occupation, sex, profileBase64, createdAt)
VALUES ($firstName, $lastName, $email, $phone, $birthDay, $occupation, $sex, $profileBase64, $createdAt);";
            insertCommand.Parameters.AddWithValue("$firstName", input.FirstName.Trim());
            insertCommand.Parameters.AddWithValue("$lastName", input.LastName.Trim());
            insertCommand.Parameters.AddWithValue("$email", input.Email.Trim());
            insertCommand.Parameters.AddWithValue("$phone", input.Phone.Trim());
            // InvariantCulture (not shown in the literal spec expression) so the
            // write side can't disagree with GetAll's InvariantCulture ParseExact
            // on a non-invariant OS culture (e.g. th-TH formats "yyyy" as the
            // Buddhist Era, Gregorian year + 543, unless a culture is forced).
            insertCommand.Parameters.AddWithValue(
                "$birthDay",
                input.BirthDay!.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            insertCommand.Parameters.AddWithValue("$occupation", input.Occupation.Trim());
            insertCommand.Parameters.AddWithValue("$sex", input.Sex.Trim());
            insertCommand.Parameters.AddWithValue("$profileBase64", input.ProfileBase64.Trim());
            insertCommand.Parameters.AddWithValue(
                "$createdAt",
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            insertCommand.ExecuteNonQuery();
        }

        using var idCommand = connection.CreateCommand();
        idCommand.CommandText = "SELECT last_insert_rowid();";
        return Convert.ToInt32(idCommand.ExecuteScalar());
    }

    /// <summary>Returns every row, newest first.</summary>
    public static List<Person> GetAll()
    {
        var result = new List<Person>();

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT id, firstName, lastName, email, phone, birthDay, occupation, sex, profileBase64, createdAt " +
            "FROM persons ORDER BY id DESC;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Person
            {
                Id = reader.GetInt32(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Email = reader.GetString(3),
                Phone = reader.GetString(4),
                BirthDay = DateTime.ParseExact(reader.GetString(5), "yyyy-MM-dd", CultureInfo.InvariantCulture),
                Occupation = reader.GetString(6),
                Sex = reader.GetString(7),
                ProfileBase64 = reader.GetString(8),
                CreatedAt = DateTime.ParseExact(reader.GetString(9), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            });
        }

        return result;
    }
}
