using Dapper;
using ExpensesApp.Data;
using ExpensesApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace ExpensesApp.Pages
{
    public class MonthlyModel : PageModel
    {
        private IConfiguration _config;
        private ISqlDataAccess _db;
        private MySqlConnection _connection;

        public IList<Payee> Payees { get; set; } = default;

        public MonthlyModel(IConfiguration config, ISqlDataAccess db, [FromServices] MySqlConnection connection) {
            _config = config;
            _db = db;
            _connection = connection;
        }

        public async Task OnGetAsync()
        {
            var sql = @"SELECT * FROM dynamic_expenses_view";

            var dynamics = await _connection.QueryAsync<DynamicExpense, Payee, Location, DynamicExpense>(sql, (de, payee, loc) => { de.Payee = payee; de.Location = loc; return de; }, splitOn: "payee_id,location_id");
            foreach (var dynamic in dynamics) {
                Console.WriteLine($"{dynamic.Id} [Name:{dynamic.Payee?.Name}] [Location:{dynamic.Location?.Name}] [Amount:${dynamic.Amount}]");
            }
        }
    }
}
