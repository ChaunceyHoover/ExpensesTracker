using System.Collections.Generic;
using System.ComponentModel.Design;
using Dapper;
using ExpensesApp.Models;
using ExpensesApp.Models.Auxiliary;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace ExpensesApp {

    public static class Api {
        private static IConfiguration _config;
        private static ILogger _logger;

        public static void ConfigureApi(this WebApplication app) {
            _config = app.Configuration;
            _logger = app.Logger;

            var group = app.MapGroup("/api");

            group.MapGet("/exp/{id:int}/{start:datetime}/{end:datetime}", GetBalance);
            group.MapGet("/exp/{id:int}", (int id) => GetBalance(id, DateTime.MinValue, DateTime.MaxValue));
            group.MapGet("/split", SplitExpenses);
            group.MapGet("/loans", Loans);
        }

        private static async Task<IResult> GetBalance(int id, DateTime start, DateTime end) {
            _logger.LogInformation($"Hit [id:{id}] [start:{start.ToShortDateString()}] [end:{end.ToShortDateString()}]");

            // Get dynamic expenses
            var expensesSql = @"CALL GetBalances(@payee_id, @start_date, @end_date)";

            try {
                using (var conn = new MySqlConnection(_config.GetConnectionString("Default"))) {
                    var expensesResult = await conn.QueryAsync<Expenses, Payee, Expenses>(
                        expensesSql,
                        (exp, payee) => { exp.Payee = payee; return exp; },
                        new { payee_id = id, start_date = start.ToString("yyyy-MM-dd"), end_date = end.ToString("yyyy-MM-dd") },
                        splitOn: "payee_id"
                    );

                    return Results.Ok(expensesResult.FirstOrDefault());
                }
            } catch(Exception ex) {
                _logger.LogError($"{ex.Message}\n{ex.StackTrace}");
                return Results.Problem("Unable to query database");
            }
        }

        private static async Task<IResult> SplitExpenses(
            [FromQuery]int draw, [FromQuery]string search, [FromQuery]string column, [FromQuery]DtOrderDir order,
            [FromQuery]DateTime dateStart, [FromQuery]DateTime dateEnd, [FromQuery]int resultStart, [FromQuery]int resultCount) {

            return await DynamicExpenseSearch(true, draw, search, column, order, dateStart, dateEnd, resultStart, resultCount);
        }

        private static async Task<IResult> Loans(
            [FromQuery]int draw, [FromQuery]string search, [FromQuery]string column, [FromQuery]DtOrderDir order,
            [FromQuery]DateTime dateStart, [FromQuery]DateTime dateEnd, [FromQuery]int resultStart, [FromQuery]int resultCount) {

            return await DynamicExpenseSearch(false, draw, search, column, order, dateStart, dateEnd, resultStart, resultCount);
        }

        private static async Task<IResult> DynamicExpenseSearch(bool splitExpenses,
            int draw, string search, string column, DtOrderDir order,
            DateTime dateStart, DateTime dateEnd, int resultStart, int resultCount) {
            //_logger.LogInformation($"DEBUG: [draw:{draw}] [search:{search}] [col:{column}] [dir:{order}] [start:{dateStart.ToShortDateString()}] [end:{dateEnd.ToShortDateString()}] [start:{resultStart}] [length:{resultCount}]");

            try {
                // We never want to allow user input directly in the query when building a dynamic
                // SQL statement, so instead we'll manually check for what column the user is
                // sorting by
                string sortCol;

                // We used an enum for the direction to force the user to specify either "asc" or "desc"
                string sortDir = order.ToString().ToUpper();

                // These are just my preferences for how I want to sort the columns
                switch (column.ToLower()) {
                    case "payee":
                        // Payee first, then by most recent, and finally by the vendor name if needed
                        sortCol = $"payee_name {sortDir}, `date` DESC, vendor_name ASC";
                        break;
                    case "vendor":
                        // Vendor first, then by most recent, then amount from most to least if needed
                        sortCol = $"vendor_name {sortDir}, `date` DESC, amount DESC";
                        break;
                    case "amount":
                        // I doubt these will ever match, unless it's repeat one-off purchases, so date as backup
                        sortCol = $"amount {sortDir}, `date` DESC";
                        break;
                    default:
                        // Default to sort by date
                        sortCol = $"`date` {sortDir}, vendor_name ASC, amount DESC";
                        break;
                }

                using (var conn = new MySqlConnection(_config.GetConnectionString("Default"))) {
                    // DataTables requires both the count of the total results AND the filtered results.
                    // To not have to maintain 2 queries, we'll use the same one twice and just change
                    // the selection and the `LIMIT`.

                    // First, we create a query for counting ALL results
                    var totalCountSql = $"SELECT COUNT(de_id) FROM dynamic_expenses_view WHERE split = {splitExpenses}";

                    // Next, we create a query for getting filtered results. I chose a local function
                    // so I can use the same query for both getting paginated results AND counting the
                    // total data results. That way I only have to maintain one query instead of two.
                    Func<string, string, string> generateSql = (selection, limit) =>
$@"SELECT {selection}
FROM dynamic_expenses_view
WHERE
    split = {splitExpenses}
    AND `date` >= @start_range
    AND `date` <= @end_range
ORDER BY {sortCol}
{limit}";

                    // The first time we use this query, we only want the TOTAL filtered results. So we
                    // just count how many results we have and don't put a `LIMIT` at the end.
                    var filteredCountSql = generateSql("COUNT(de_id)", "");

                    // Next, we get the actual paginated results based off what the user submitted via
                    // the jQuery datatables plugin
                    var resultSql = generateSql("*", $"LIMIT {resultStart}, {resultCount}");

                    // Now that we have our queries, we just need to execute all of them.

                    // The total sum doesn't need any parameters since it's just counting all the data.
                    var totalResults = await conn.QueryAsync<int>(totalCountSql);

                    // The filtered results will be both need to pass the same date range parameters, so
                    // we'll define that now and then pass it to each query.
                    var filteredParameters = new {
                        start_range = dateStart.ToString("yyyy-MM-dd"),
                        end_range = dateEnd.ToString("yyyy-MM-dd")
                    };

                    // Query the TOTAL FILTERED results first in hopes that MySQL will cache those results
                    var filteredResults = await conn.QueryAsync<int>(filteredCountSql,
                        param: filteredParameters);

                    // Query the user specified FILTERED results and map the data to objects
                    var dynamicResults = await conn.QueryAsync<DynamicExpense, Payee, Vendor, DynamicExpense>(
                        resultSql,
                        (de, payee, vend) => {
                            de.Payee = payee;
                            de.Vendor = vend;
                            return de;
                        },
                        param: filteredParameters,
                        splitOn: "payee_id,vendor_id"
                    );

                    // Assuming we made it this far, return the values required by DataTables (draw, totals)
                    // and the results back to the user
                    return Results.Ok(new DtResult<DynamicExpense>() {
                        Draw = draw,
                        RecordsTotal = totalResults.FirstOrDefault(),
                        RecordsFiltered = filteredResults.FirstOrDefault(),
                        Data = dynamicResults
                    });
                }
            } catch (Exception ex) {
                _logger.LogError($"{ex.Message}\n{ex.StackTrace}");
                return Results.Ok(new DtResult<DynamicExpense>() {
                    Error = "Unable to query database"
                });
            }
        }
    }
}
