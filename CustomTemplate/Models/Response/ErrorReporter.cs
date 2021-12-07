namespace CustomTemplate.Models.Response
{
    public class ErrorReporter
    {
        public string? Message { get; set; } 
        public int StatusCode { get; set; }
        public Dictionary<object, object> Details { get; set; } = new();
        public DateTime TimeStamp { get; } = DateTime.Now;
    }

    public static class ErrorReporterProvider
    {
        public static ErrorReporter Set(string? message, int statusCode, Dictionary<object, object> details)
        {
            return new ErrorReporter()
            {
                Message = message,
                StatusCode = statusCode,
                Details = details
            };
        }
    }
}