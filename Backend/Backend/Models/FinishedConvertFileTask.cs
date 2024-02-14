namespace Backend.Api.Models
{
    /// <summary>
    /// Transfers data which describes finished convertion task
    /// </summary>
    public class FinishedConvertFileTask
    {
        /// <summary>
        /// Conversion task Id
        /// </summary>
        public int ConvertTaskId { get; set; }

        /// <summary>
        /// Html file to convert
        /// </summary>
        public required string Filename { get; set; }
    }
}
