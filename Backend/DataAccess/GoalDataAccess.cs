using Backend.Models;
using Dapper;

namespace Backend.DataAccess;

public static class GoalDataAccess
{
    public static Goal CreateGoal(string name, int cost, int userId)
    {
        string insertQuery = @"INSERT INTO goals(name, cost, user_id) VALUES(@name, @cost, @userid)
                                RETURNING id, name, progress, cost, user_id";
        using var conn = Database.Database.GetConn();

        Goal goal = conn.QuerySingle<Goal>(insertQuery, new { name, cost, userId });
        return goal;
    }

    public static bool DeleteGoal(int goalId)
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

    public static Goal? UpdateGoal(int goalId, string name, int cost, int userId)
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

        var updated = conn.Execute(updateQuery, new { userId, goalId });
        if (updated == 0) return false;
        return true;
    }

    public static Goal? MakeActive(int userId, int goalId)
    {
        string checkQuery = @"SELECT user_id FROM goals WHERE id = @goalId";
        string updateQuery = @"UPDATE users  SET current_goal_id = @goalId  WHERE id = @userId";
        string selectQuery = @"SELECT * FROM goals WHERE id = @goalId";
        using var conn = Database.Database.GetConn();

        int assignedUser = conn.ExecuteScalar<int>(checkQuery, new { userId });
        if (assignedUser != userId) return null; // If current owner of goal is not assignee

        var updated = conn.Execute(updateQuery, new { userId, goalId });
        if (updated == 0) return null;

        Goal updatedGoal = conn.QueryFirst<Goal>(selectQuery, new { goalId });

        return updatedGoal;
    }

    public static Goal? GetGoal(int id)
    {
        string selectQuery = @"SELECT * FROM goals WHERE id = @id";
        using var conn = Database.Database.GetConn();

        Goal? goal = conn.QueryFirstOrDefault<Goal>(selectQuery, new { id });
        if (goal == null) return null;
        return goal;
    }

    public static List<Goal>? GetUserGoals(int userId)
    {
        string selectQuery = @"SELECT * FROM goals WHERE user_id = @userId";
        using var conn = Database.Database.GetConn();

        List<Goal>? goals = conn.Query<Goal>(selectQuery, new { userId }).ToList();
        return goals;
    }

    public static bool? CheckIfCompleted(int goalId)
    {
        string selectQuery = @"SELECT * FROM goals WHERE id = @goalId";
        using var conn = Database.Database.GetConn();

        Goal? goal = conn.QueryFirstOrDefault<Goal>(selectQuery, new { goalId });
        if (goal == null) return null;
        if (goal.Progress < goal.Cost) return false;
        return true;
    }
}