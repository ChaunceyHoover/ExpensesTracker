using Dapper;
using Dapper.FluentMap;
using ExpensesApp.Models.Mapper;
using MySqlConnector;
using System.Data;

namespace ExpensesApp.Data {
    /// <summary>
    /// Interface for setting up dependency injection for SQL access in a project
    /// </summary>
    public interface ISqlDataAccess {
        Task<IEnumerable<T>> GetData<T, U>(string query, U parameters, string connId = "Default");
        Task SaveData<T>(string query, T parameters, string connId = "Default");
    }

    /// <summary>
    /// Implementation of dependency injection Sql Access 
    /// </summary>
    public class SqlDataAccess : ISqlDataAccess {
        private readonly IConfiguration _config;

        public SqlDataAccess(IConfiguration config) {
            _config = config;

            FluentMapper.Initialize(m => {
                m.AddMap(new PayeeMap());
                m.AddMap(new LocationMap());
                m.AddMap(new DynamicExpenseMap());
            });
        }

        public async Task<IEnumerable<T>> GetData<T, U>(string query, U parameters, string connectionID = "Default") {
            using IDbConnection dbConnection = new MySqlConnection(_config.GetConnectionString(connectionID));
            return await dbConnection.QueryAsync<T>(query, parameters);
        }

        public async Task SaveData<T>(string query, T parameters, string connectionID = "Default") {
            using IDbConnection dbConnection = new MySqlConnection(_config.GetConnectionString(connectionID));
            await dbConnection.ExecuteAsync(query, parameters);
        }
    }
}
