using System.Threading.Channels;
using Backend.Api.Models;
using Backend.Api.Repositories;

namespace Backend.Api.Services
{
    public class FileConverterService(FileConverterRepository fileConverterRepository, ChannelWriter<ConvertFileTask> channelWriter)
    {
        public async Task<int> ConvertHtmlFile(ConvertFileRequest convertFileRequest)
        {
            var convertFileTaskId = await fileConverterRepository.SaveHtmlFileForProcessing(convertFileRequest);

            await channelWriter.WriteAsync(
                new ConvertFileTask()
                {
                    ConvertTaskId = convertFileTaskId, 
                    HtmlFileData = convertFileRequest.HtmlFileData
                });
            
            return convertFileTaskId;
        }

        public async Task<IEnumerable<FinishedConvertFileTask>> GetAllFinishedTasksAsync()
        {
            var allFinishedTasks = await fileConverterRepository.GetAllFinishedConvertFileTasksAsync();

            return allFinishedTasks;
        }

        public async Task<byte[]?> GetPdfFileByFinishedTaskId(int finishedTaskId)
        {
            var pdfFile = await fileConverterRepository.GetPdfFileByFinishedTaskId(finishedTaskId);

            return pdfFile;
        }

        public async Task DeleteConvertTaskByID(int deletingConvertTaskId)
        {
            await fileConverterRepository.DeleteConvertTaskByID(deletingConvertTaskId);
        }
    }
}
