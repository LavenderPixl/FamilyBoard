using Npgsql;

namespace Backend.Database;

public class Database
{
    public static string ConnectionString { private get; set; }
    private static NpgsqlDataSource DataSource;

    public static void Initialize()
    {
        DataSource = new NpgsqlDataSourceBuilder(ConnectionString).Build();
    }

    public static NpgsqlConnection GetConn()
    {
        return DataSource.OpenConnection();
    }
}