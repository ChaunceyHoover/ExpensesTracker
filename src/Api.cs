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
        }
    }
}
