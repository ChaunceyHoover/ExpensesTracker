using System.Reflection.Metadata.Ecma335;
using Dapper;
using ExpensesApp.Models;
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
    }
}
