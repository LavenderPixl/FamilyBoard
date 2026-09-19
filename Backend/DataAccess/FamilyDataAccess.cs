using System.Security.Cryptography;
using Backend.Models;
using Dapper;

namespace Backend.DataAccess;

public static class FamilyDataAccess
{
    public static Family? CreateFamily(string familyName)
    {
        string insertQuery = @"INSERT INTO families (name) VALUES (@familyName) RETURNING id, name";
        using var conn = Database.Database.GetConn();

        Family family = conn.QuerySingle<Family>(insertQuery, new { familyName });
        return family;
    }

    public static bool DeleteFamily(int familyId)
    {
        string deleteQuery = @"DELETE FROM families WHERE id = @familyId";
        using var conn = Database.Database.GetConn();
        
        return conn.Execute(deleteQuery, new { familyId }) != 0;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="familyId"></param>
    /// <returns>Boolean</returns>
    public static bool JoinFamily(int userId, int? familyId)
    {
        string updateQuery = @"UPDATE users SET family_id = @familyId WHERE id = @userId";
        using var conn = Database.Database.GetConn();

        return conn.Execute(updateQuery, new { familyId, userId }) == 1;
    }

    public static bool LeaveFamily(int userId, int familyId)
    {
        string updateQuery = @"UPDATE users SET family_id = NULL WHERE id = @userId ";
        using var conn = Database.Database.GetConn();
        
        return conn.Execute(updateQuery, new { userId, familyId }) == 1;
    }


    public static Family? GetFamily(int familyId)
    {
        string selectQuery = @"SELECT * FROM  families WHERE id = @familyId";
        using var conn = Database.Database.GetConn();
     
        Family? family = conn.QueryFirstOrDefault<Family>(selectQuery, new { familyId });
        return family;
    }

    /// <summary>
    /// Gets which family the code belongs to, and checks if it's valid, by expiration date.
    /// </summary>
    /// <param name="code">Invitation code</param>
    /// <returns>Family ID</returns>
    public static int? GetFamilyByCode(string familyCode)
    {
        string selectQuery = @"SELECT family_id FROM family_codes WHERE code = @familyCode
                               AND expiration >= CURRENT_DATE";
        using var conn = Database.Database.GetConn();
        
        int? familyId = conn.QuerySingleOrDefault<int?>(selectQuery, new { familyCode });

        return familyId;
    }
    
    public static List<User> GetFamilyMembers(int familyId)
    {
        string selectQuery = @"SELECT * FROM users WHERE family_id = @FamilyId";
        using var conn = Database.Database.GetConn();
        
        List<User> members = conn.Query<User>(selectQuery, new { familyId }).ToList();
        return members;
    }

    // For invitation code
    private static string GenerateCode()
    {
        string code = RandomNumberGenerator.GetString("0123456789", 8);
        return code;
    }
    

    public static bool CreateFamilyInvite(FamilyCode familyCode)
    {
        string insertQuery = @"INSERT INTO family_codes (code, expiration, created_by, family_id) 
                               VALUES (@Code, @Expiration, @CreatedBy, @FamilyId)";
        using var conn = Database.Database.GetConn();

        int loop = 0;
        while (loop < 3) // Max tries, 3
        {
            string generatedCode = GenerateCode();
            var codeExists = conn.ExecuteScalar<bool>
                ("SELECT COUNT(1) FROM family_codes WHERE code = @Code", new {Code = generatedCode});
            if (!codeExists)
            {
                familyCode.Code = generatedCode;
                conn.Execute(insertQuery, familyCode);
                return true;
            }
            loop++;
        }
        return false;
    }
}
