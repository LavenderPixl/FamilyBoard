using Npgsql;

namespace Backend.Database;

public class Database
{
    private static string ConnectionString = "Host=localhost:5432;Username=postgres;Password=password;Database=familyBoard";

    public static NpgsqlConnection GetConn()
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(ConnectionString);
        var dataSource = dataSourceBuilder.Build();
        return dataSource.OpenConnection();
    }
    
}