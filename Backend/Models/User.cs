using System.Text.Json.Serialization;
using Dapper;

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
    public Family Family { get; set; }
    
    public static bool IsPasswordValid(string email, string password)
    {
        var conn = Database.Database.GetConn();
        string hashedPasswordQuery = @"SElECT hashed_password FROM users WHERE email = @email";

        string? hashedPassword = conn.QueryFirstOrDefault<string>(hashedPasswordQuery, new { email = email });
        if (hashedPassword == null) return false;
        // Checks if the password matches our hashed
        if (!BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword)) return false;

        return true;
    }
}