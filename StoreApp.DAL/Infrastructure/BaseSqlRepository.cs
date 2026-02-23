using Microsoft.Data.SqlClient;

namespace StoreApp.DAL.Infrastructure;

public abstract class BaseSqlRepository
{
    private readonly string _connectionString;

    public BaseSqlRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    public SqlConnection OpenConnection()
    {
        var con = new SqlConnection(_connectionString);
        con.Open();
        return con;
    }
}
