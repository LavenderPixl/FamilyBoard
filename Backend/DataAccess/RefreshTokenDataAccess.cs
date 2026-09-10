using Backend.Models;
using Dapper;

namespace Backend.DataAccess;

public class RefreshTokenDataAccess
{
    public static void CreateRefreshToken(string token, DateTime expiration, int userId)
    {
        string insertQuery = @"INSERT INTO refresh_tokens (token, expiration, user_id) VALUES (@token, @expiration, @userId)";
        var conn = Database.Database.GetConn();

        conn.Execute(insertQuery, new { token, expiration, userId });
    }

    public static RefreshToken? GetRefreshTokenFromToken(string token)
    {
        string selectQuery = @"SELECT * FROM refresh_tokens WHERE token = @token";
        var conn = Database.Database.GetConn();

        var refreshToken = conn.QueryFirstOrDefault<RefreshToken>(selectQuery, new { token });

        return refreshToken;
    }
}