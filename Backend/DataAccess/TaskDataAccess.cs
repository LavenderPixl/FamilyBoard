using Dapper;

namespace Backend.DataAccess;

public class TaskDataAccess
{
    public static Models.Task? CreateTask(string name, int reward, DateOnly expireDate, int familyId, int? userId)
    {
        var expireAsDateTime = expireDate.ToDateTime(TimeOnly.MinValue);
        string insertQuery = @"INSERT INTO tasks (name, reward, expire_date, family_id, user_id) 
                                    VALUES (@name, @reward, @expireDate, @familyId, @userId)
                                    RETURNING id";
        string selectQuery = @"SELECT tasks.id, name, reward, expire_date, completed, tasks.family_id, user_id, username FROM tasks 
                                    LEFT JOIN users u on tasks.user_id = u.id
                                    WHERE tasks.id = @id";
        using var conn = Database.Database.GetConn();

        int? taskId = conn.QueryFirstOrDefault<int>(insertQuery, new { name, reward, expireDate = expireAsDateTime, familyId, userId });
        if (taskId == null) return null;

        Models.Task? task = conn.QueryFirstOrDefault<Models.Task>(selectQuery, new { id = taskId });
        return task;
    }

    public static Models.Task? GetTask(int id)
    {
        string selectQuery = @"SELECT tasks.id, name, reward, completed, expire_date, tasks.family_id, user_id, username, goal_id
                                    FROM tasks 
                                    LEFT JOIN users u on tasks.user_id = u.id
                                    WHERE tasks.id = @id";
        using var conn = Database.Database.GetConn();

        Models.Task? task = conn.QueryFirstOrDefault<Models.Task>(selectQuery, new { id });
        return task;
    }

    public static List<Models.Task> GetTasksForFamily(int familyId)
    {
        string selectQuery = @"SELECT tasks.id, name, reward, completed, expire_date, tasks.family_id, user_id, username, goal_id 
                                    FROM tasks
                                    LEFT JOIN public.users u on tasks.user_id = u.id
                                    WHERE tasks.family_id = @familyId";
        using var conn = Database.Database.GetConn();

        var task = conn.Query<Models.Task>(selectQuery, new { familyId }).ToList();
        return task;
    }
    
    public static List<Models.Task> GetTasksForUser(int userId)
    {
        string selectQuery = @"SELECT tasks.id, name, reward, completed, expire_date, tasks.family_id, user_id, username, goal_id
                                    FROM tasks
                                    LEFT JOIN public.users u on tasks.user_id = u.id
                                    WHERE tasks.user_id = @userId";
        using var conn = Database.Database.GetConn();

        var tasks = conn.Query<Models.Task>(selectQuery, new { userId }).ToList();
        return tasks;
    }

    public static Models.Task? UpdateTask(int id, string name, int reward, DateOnly expireDate, int? userId)
    {
        var expireAsDateTime = expireDate.ToDateTime(TimeOnly.MinValue);
        string updateQuery = @"UPDATE tasks SET name = @name, reward = @reward, expire_date = @expireDate, user_id = @userId WHERE id = @id";
        string selectQuery = @"SELECT tasks.id, name, reward, completed, expire_date, tasks.family_id, user_id, username FROM tasks 
                                    LEFT JOIN users u on tasks.user_id = u.id
                                    WHERE tasks.id = @id";
        using var conn = Database.Database.GetConn();

        conn.Execute(updateQuery, new {id, name, reward, expireDate = expireAsDateTime, userId});
        var updatedTask = conn.QueryFirstOrDefault<Models.Task>(selectQuery, new {id});
        return updatedTask;
    }

    public static bool UpdateCompletedStatus(int id, int reward, bool completed, int userId, int? goalId)
    {
        string updateUserQuery = @"UPDATE users SET points = points + @reward WHERE id = @userID";
        string updateTaskQuery = @"UPDATE tasks SET completed = @completed, goal_id = @goalId WHERE id = @id";
        string updateGoalQuery = @"UPDATE goals SET progress = progress + @reward WHERE id = @goalId";
        using var conn = Database.Database.GetConn();

        using (var tran = conn.BeginTransaction())
        {
            try
            {
                conn.Execute(updateUserQuery, new { reward, userId });
                conn.Execute(updateTaskQuery, new { id, completed, goalId });
                conn.Execute(updateGoalQuery, new { reward, goalId });
                tran.Commit();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                tran.Rollback();
                return false;
            }
        }
    }

    public static bool DeleteTask(int id)
    {
        string deletionQuery = @"DELETE FROM tasks WHERE id = @id";

        using var conn = Database.Database.GetConn();

        try
        {
            conn.Execute(deletionQuery, new { id });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }

        return true;
    }
}