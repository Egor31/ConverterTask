using System.Threading.Channels;
using Backend.Api.Models;
using Backend.Api.Repositories;
using Microsoft.AspNetCore.SignalR;
using PuppeteerSharp;

namespace Backend.Api.Services;

public class FileConverterHostedService(
    ChannelReader<ConvertFileTask> channelReader,
    ChannelWriter<ConvertFileTask> channelWriter,
    IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var serviceScope = serviceProvider.CreateScope();
        var fileConverterRepository =
            serviceScope.ServiceProvider.GetRequiredService<FileConverterRepository>();
        var signalRHubContext = serviceScope.ServiceProvider.GetRequiredService<IHubContext<ConvertTaskHub>>();

        await WriteToChannelNotCompletedTasks(fileConverterRepository, cancellationToken);

        await foreach (var convertFileTask in channelReader.ReadAllAsync(cancellationToken))
        {
            try
            {
                var htmlFileString = ConvertBase64ToString(convertFileTask.HtmlFileData);

                convertFileTask.PdfFileData = await ConvertHtmlToPdf(htmlFileString);

                await fileConverterRepository.SavePdfFile(convertFileTask);

                await signalRHubContext.Clients.All.SendAsync("taskCompleted", convertFileTask.ConvertTaskId);

                Console.WriteLine($"task running on thread: {Environment.CurrentManagedThreadId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    private static string ConvertBase64ToString(string base64String)
    {
        var byteData = Convert.FromBase64String(base64String);
        var htmlFileString = System.Text.Encoding.UTF8.GetString(byteData);

        return htmlFileString;
    }

    private static async Task<byte[]> ConvertHtmlToPdf(string htmlFileString)
    {
        var launchBrowserOptions = new LaunchOptions { Headless = true };
        using var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(launchBrowserOptions);
        await using var page = await browser.NewPageAsync();
        
        await page.SetContentAsync(htmlFileString);
        
        var pdfFileData = await page.PdfDataAsync();
        await page.CloseAsync();

        return pdfFileData;
    }

    private async Task WriteToChannelNotCompletedTasks(
        FileConverterRepository fileConverterRepository, 
        CancellationToken cancellationToken)
    {
        var notCompletedConvertTasks = await fileConverterRepository.GetNotExecutedConvertTasks();

        foreach (var notCompletedTask in notCompletedConvertTasks)
        {
            await channelWriter.WriteAsync(notCompletedTask, cancellationToken);
        }
    }
}