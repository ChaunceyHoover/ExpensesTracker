using Dapper;
using ExpensesApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace ExpensesApp.Pages
{
    public class MonthlyModel : PageModel
    {
        private MySqlConnection _conn;

        public IList<StaticExpense> StaticExpenses { get; set; }

        public MonthlyModel([FromServices] MySqlConnection connection)
            => _conn = connection;

        public async Task OnGetAsync()
        {
            // Get static expenses
            var staticSql = @"SELECT * FROM static_expenses";
            var staticResults = await _conn.QueryAsync<StaticExpense>(staticSql);

            // Map to results for razor page to access
            StaticExpenses = staticResults.ToList();
        }
    }
}