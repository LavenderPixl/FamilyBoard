using System.Text.Json.Serialization;

namespace Backend.Models;

public class User
{
    public int Id;
    public string Username;
    public string Email;
    [JsonIgnore]
    public string? HashedPassword;
    [JsonIgnore]
    public string? Password;
    public int Points;
    public bool SsAdult;
    public Family Family;
}