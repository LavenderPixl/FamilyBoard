using System.Security.Cryptography;
using Backend.Models;
using Dapper;

namespace Backend.DataAccess;

public class FamilyDataAccess
{
    public static Family? CreateFamily(string familyName)
    {
        string insertQuery = @"INSERT INTO families (name) VALUES (@familyName) RETURNING id, name";
        using var conn = Database.Database.GetConn();

        Family family = conn.QuerySingle<Family>(insertQuery, new { familyName });
        return family;
    }
    
    public static bool JoinFamily(int userId, int familyId)
    {
        string updateQuery = @"UPDATE users SET family_id = @familyId WHERE id = @userId";
        using var conn = Database.Database.GetConn();
        
        conn.Execute(updateQuery, new { familyId, userId });
        return true;
    }

    private static string GenerateCode()
    {
        string code = RandomNumberGenerator.GetString("0123456789", 8);
        return code;
    }

    public static bool CreateFamilyInvite(FamilyCode familyCode)
    {
        string insertQuery = @"INSERT INTO family_codes (code, expiration, created_by, family_id) 
                               VALUES (@code, @expiration, @created_by, @family_id)";
        using var conn = Database.Database.GetConn();

        int loop = 0;
        while (loop < 3)
        {
            string generatedCode = GenerateCode();
            var codeExists = conn.ExecuteScalar<bool>
                ("SELECT COUNT(1) FROM family_codes WHERE code = @Code", new {Code = generatedCode});
            if (!codeExists)
            {
                familyCode.Code = generatedCode;
                conn.Execute(insertQuery, new
                {
                    familyCode, familyCode.Expiration, familyCode.CreatedBy, familyCode.FamilyId
                });
                return true;
            }
            loop++;
        }
        return false;
    }
    
    // public static bool DeleteFamily(int familyId)
    // {
    //     
    // }
    //
    // public static Family? GetFamily(int familyId)
    // {
    //     
    // }
    //
    // public static List<User> GetFamilyMembers(int familyId)
    // {
    //     
    // }
    //

    //
    // public static bool LeaveFamily(int familyId, int userId)
    // {
    //     
    // }
    //

}
