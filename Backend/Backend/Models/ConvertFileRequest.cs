namespace Backend.Api.Models;

public class ConvertFileRequest
{
    public required string FileName { get; set; }
    
    public required string HtmlFileData { get; set; }
}