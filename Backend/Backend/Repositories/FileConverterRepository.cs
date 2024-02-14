using System.Data;
using Backend.Api.Models;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Backend.Api.Repositories
{
    public class FileConverterRepository
    {
        private readonly IDbConnection _dbConnection;
        
        public FileConverterRepository(IConfiguration appConfiguration) 
        {
            var sqliteConnectionString = appConfiguration.GetConnectionString("SqliteDbFileName") 
                ?? throw new ArgumentException("Sqlite connection string not found in configuration");
            
            _dbConnection = new SqliteConnection(sqliteConnectionString);
        }

        public async Task<int> SaveHtmlFileForProcessing(ConvertFileRequest convertFileRequest)
        {
            var saveHtmlFileForProcessingQuery = @"
                INSERT INTO convert_tasks (filename, html_file_content) 
                VALUES (@FileName, @HtmlFileData);
                SELECT last_insert_rowid();
            ";
            
            var convertFileTaskId = 
                await _dbConnection.ExecuteScalarAsync<int>(saveHtmlFileForProcessingQuery, convertFileRequest);

            return convertFileTaskId;
        }

        public async Task SavePdfFile(ConvertFileTask finishedConvertFileTask)
        {
            var savePdfFileQuery = @"
                UPDATE convert_tasks 
                SET pdf_file_content = @PdfFileData
                WHERE convert_task_id = @ConvertTaskId;
            ";
            
            await _dbConnection.ExecuteScalarAsync<int>(savePdfFileQuery, finishedConvertFileTask);
        }

        public async Task<IEnumerable<FinishedConvertFileTask>> GetAllFinishedConvertFileTasksAsync()
        {
            var getFinishedConvertFileTasksQuery = @"
                SELECT 
                    convert_task_id as ConvertTaskId, 
                    filename
                FROM convert_tasks
                WHERE pdf_file_content IS NOT NULL;
            ";

            var finishedConvertFileTasks = 
                await _dbConnection.QueryAsync<FinishedConvertFileTask>(getFinishedConvertFileTasksQuery);

            return finishedConvertFileTasks;
        }

        public async Task<byte[]?> GetPdfFileByFinishedTaskId(int finishedTaskId)
        {
            var getPdfByFinishedTaskIdQuery = @"
                SELECT pdf_file_content 
                FROM convert_tasks 
                WHERE convert_task_id = @finishedTaskId";

            var pdfFile = await _dbConnection.QueryFirstOrDefaultAsync<byte[]>(
                getPdfByFinishedTaskIdQuery, 
                new { finishedTaskId });

            return pdfFile;
        }

        public async Task DeleteConvertTaskByID(int deletingConvertTaskId)
        {
            var deletingConvertTaskByIdQuery = @"
                DELETE FROM convert_tasks 
                WHERE convert_task_id = @deletingConvertTaskId
            ";

            await _dbConnection.ExecuteAsync(deletingConvertTaskByIdQuery, new { deletingConvertTaskId });
        }

        public async Task<IEnumerable<ConvertFileTask>> GetNotExecutedConvertTasks()
        {
            var getNotExecutedConvertTasksQuery = @"
                SELECT 
                      convert_task_id   AS ConvertTaskId 
                    , html_file_content AS HtmlFileData 
                FROM convert_tasks 
                WHERE pdf_file_content IS NULL"
            ;

            var notExecutedConvertTasks = await _dbConnection.QueryAsync<ConvertFileTask>(getNotExecutedConvertTasksQuery);

            return notExecutedConvertTasks;
        }
    }
}
