using Backend.Api.Models;
using Backend.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers
{
    [Route("[controller]/[action]")]
    public class ConverterController(FileConverterService fileConverterService) : ControllerBase
    {
        [HttpPost]
        public async Task<int> ConvertFile([FromBody] ConvertFileRequest convertFileRequest)
        {
            await Task.Delay(2000);
            return await fileConverterService.ConvertHtmlFile(convertFileRequest);
        }

        [HttpGet]
        public async Task<IEnumerable<FinishedConvertFileTask>> GetAllFinishedTasks()
        {
            return await fileConverterService.GetAllFinishedTasksAsync();
        }

        [HttpGet]
        public async Task<IActionResult> GetPdfFileByFinishedTaskId(int finishedTaskId)
        {
            var pdfData = await fileConverterService.GetPdfFileByFinishedTaskId(finishedTaskId);

            if (pdfData == null)
            {
                return NotFound("No PDF file found for the specified finishedTaskId.");
            }

            return File(pdfData, "application/pdf", "FileName.pdf");
        }

        [HttpGet]
        public async Task DeleteTaskById(int deletingTaskId)
        {
            await fileConverterService.DeleteConvertTaskByID(deletingTaskId);
        }
    }
}
