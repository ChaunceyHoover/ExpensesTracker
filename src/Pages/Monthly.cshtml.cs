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

        public IList<DynamicExpense> DynamicExpenses { get; set; }
        public IList<StaticExpense> StaticExpenses { get; set; }

        public MonthlyModel([FromServices] MySqlConnection connection)
            => _conn = connection;

        public async Task OnGetAsync()
        {
            // Get dynamic expenses
            var dynamicSql = @"SELECT * FROM dynamic_expenses_view";
            var dynamicResults = await _conn.QueryAsync<DynamicExpense, Payee, Vendor, DynamicExpense>(dynamicSql, (de, payee, loc) => {
                de.Payee = payee;
                de.Location = loc;
                return de;
            }, splitOn: "payee_id,vendor_id");

            // Get static expenses
            var staticSql = @"SELECT * FROM static_expenses";
            var staticResults = await _conn.QueryAsync<StaticExpense>(staticSql);

            // Map to results for razor page to access
            DynamicExpenses = dynamicResults.ToList();
            StaticExpenses = staticResults.ToList();
        }
    }
}
