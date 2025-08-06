namespace WebApi.Models
{
    public class LogRequest
    {
        public string Message { get; set; } = string.Empty;
        public string Level { get; set; } = "info";
    }

}
