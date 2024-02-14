using Dapper;
using Microsoft.Data.Sqlite;

namespace Backend.Api
{
    public static class StartupDatabaseManagement
    {
        public static async Task InitializeTables(WebApplication app)
        {
            var connectionString = app.Configuration.GetConnectionString("SqliteDbFileName") ?? 
                throw new ArgumentException("SqliteDbFileName not set in app configuration");
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var createTableSql = @"
                CREATE TABLE IF NOT EXISTS convert_tasks (
                    convert_task_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    filename TEXT,
                    html_file_content TEXT,
                    pdf_file_content BLOB
                )
            ";

            await connection.ExecuteAsync(createTableSql);
        }
    }
}
 