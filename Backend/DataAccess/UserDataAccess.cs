using Backend.Models;
using Dapper;

namespace Backend.DataAccess;

public static class UserDataAccess
{
    public static bool CreateUser(string email, string username, string password)
    {
        string insertQuery = @"INSERT INTO users (email, username, hashed_password) VALUES (@email, @username, @hashedPassword)";
        string hashedPassword = hashPassword(password);
        var conn = Database.Database.GetConn();
        
        var isEmailTaken = conn.ExecuteScalar<bool>("SELECT COUNT(1) FROM users WHERE email=@email",
            new { email = email });

        if (isEmailTaken)
            return false;
        
        conn.Execute(insertQuery, new { email, username, hashedPassword });

        return true;
    }
    
    public static User? GetUser(int userId)
    {
        string selectQuery = @"SELECT * FROM users WHERE id = @id";
        var conn = Database.Database.GetConn();

        var user = conn.QueryFirstOrDefault<User>(selectQuery, new { id = userId });
        
        return user;
    }
    
    public static User? GetUserFromEmail(string email)
    {
        string SelectQuery = @"SELECT * FROM users WHERE email = @email";
        var conn = Database.Database.GetConn();

        var user = conn.QueryFirstOrDefault<User>(SelectQuery, new { email = email });
        
        return user;
    }

    public static string? GetHashedPasswordFromEmail(string email)
    {
        string hashedPasswordQuery = @"SElECT hashed_password FROM users WHERE email = @email";
        var conn = Database.Database.GetConn();

        return conn.QueryFirstOrDefault<string>(hashedPasswordQuery, new { email = email });
    }
    
    public static bool DeleteUser(int userId)
    {
        string deletionquery = @"DELETE FROM users WHERE id=@id";

        var conn = Database.Database.GetConn();

        try
        {
            conn.Execute(deletionquery, new { id = userId });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        return true;
    }
    
    public static bool ChangePassword(int userId, string email, string oldPassword, string newPassword)
    {
        if (!Models.User.IsPasswordValid(email, oldPassword)) return false;

        string newHash = hashPassword(newPassword);
        UpdateSingleColumnForUser("hashed_password", newHash, userId);
        
        return true;
    }

    public static bool AddPoints(int userId, int amount)
    {
        string updateQuery = @"UPDATE users SET points = points + @amount WHERE id = @id";
        var conn = Database.Database.GetConn();

        try
        {
            conn.Execute(updateQuery, new { id = userId, amount });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }

        return true;
    }

    public static bool UpdateAdultStatus(int userId, bool isAdult)
    {
        try
        {
            UpdateSingleColumnForUser("is_adult", isAdult, userId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        
        return true;
    }

    public static bool UpdateUsername(int userId, string username)
    {
        try
        {
            UpdateSingleColumnForUser("username", username, userId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }

        return true;
    }
    
    private static void UpdateSingleColumnForUser<T>(string column, T newValue, int userId)
    {
        string updateQuery = @$"UPDATE users SET {column} = @newValue WHERE id = @id";
        var conn = Database.Database.GetConn();
        
        conn.Execute(updateQuery, new { newValue = newValue, id = userId });
    }
    private static string hashPassword(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, 5);
    }
}