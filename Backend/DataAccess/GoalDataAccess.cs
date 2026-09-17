using Backend.Models;
using Dapper;

namespace Backend.DataAccess;

public static class GoalDataAccess
{
    public static Goal? CreateGoal(string name, int cost, int userId)
    {
        string insertQuery = @"INSERT INTO goals(name, cost, user_id) VALUES(@name, @cost, @userid)";
        using var conn = Database.Database.GetConn();

        Goal goal = conn.QuerySingle<Goal>(insertQuery, new { name, cost, userId});
        return goal;
    }

    public static bool? DeleteGoal(int goalId)
    {
        string deleteQuery = @"DELETE FROM goals WHERE id = @goalId";
        using var conn = Database.Database.GetConn();

        try
        {
            conn.Execute(deleteQuery, new { goalId });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }

        return true;
    }
    
    public static Goal? EditGoal(string name, int cost, int userId, int goalId)
    {
        string updateQuery = "UPDATE goals SET name = @name, cost = @cost, user_id = @userId WHERE id = @goalId";
        string selectQuery = "SELECT * FROM goals WHERE id = @goalId";
        
        using var conn = Database.Database.GetConn();

        try
        {
            conn.Execute(updateQuery, new { name, cost, userId, goalId });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }

        var updatedGoal = conn.QueryFirstOrDefault<Goal>(selectQuery, new { goalId });
        return updatedGoal;
    }

    public static bool AssignGoal(int userId, int goalId)
    {
        string updateQuery = @"UPDATE goals  SET user_id = @userId  WHERE id = @goalId";
        using var conn = Database.Database.GetConn();
        
        return conn.Execute(updateQuery, new { userId, goalId }) == 1;
    }

    public static Goal MakeActive(int userId, int goalId)
    {
        string updateQuery = @"UPDATE users  SET current_goal = @goalId  WHERE id = @userId";
        using var conn = Database.Database.GetConn();
        
        return conn.Execute(updateQuery, new { userId, goalId }) == 1;
    }
}