using Npgsql;

namespace Backend.Database;

public class Database
{
    public static string ConnectionString { private get; set; }

    public static NpgsqlConnection GetConn()
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(ConnectionString);
        var dataSource = dataSourceBuilder.Build();
        return dataSource.OpenConnection();
    }
    
}