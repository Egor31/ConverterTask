namespace Backend.Api.Models
{
    /// <summary>
    /// Incoming conversion task model
    /// </summary>
    public class ConvertFileTask
    {
        /// <summary>
        /// Conversion task Id
        /// </summary>
        public int ConvertTaskId { get; set; }
        
        /// <summary>
        /// Html file to convert
        /// </summary>
        public required string HtmlFileData { get; set; }
        
        /// <summary>
        /// Pdf file result
        /// </summary>
        public byte[]? PdfFileData { get; set; }
    }
}
