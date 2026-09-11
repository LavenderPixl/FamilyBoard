using System.Text.Json.Serialization;
using Backend.DataAccess;

namespace Backend.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    [JsonIgnore]
    public string? HashedPassword { get; set; }
    [JsonIgnore]
    public string? Password { get; set; }
    public int Points { get; set; }
    public bool IsAdult { get; set; }
    public int FamilyId { get; set; }
    
    public static bool IsPasswordValid(string email, string password)
    {
        string? hashedPassword = UserDataAccess.GetHashedPasswordFromEmail(email);
        if (hashedPassword == null) return false;
        // Checks if the password matches our hashed
        if (!BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword)) return false;

        return true;
    }
}
