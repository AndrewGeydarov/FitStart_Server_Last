namespace FitStart_Server.ReturnModels
{
    public class ReturnFileResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? FilePath { get; set; }
        public Stream? Stream { get; set; }
        public string? ContentType { get; set; }
    }
}
