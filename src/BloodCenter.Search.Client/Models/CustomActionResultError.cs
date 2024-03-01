namespace BloodCenter.Search.Client.Models
{
    public class CustomActionResultError
    {
        public string Message { get; set; }
        public string? PropertyMessage { get; set; }

        public CustomActionResultError(string message, string? propertyMessage = null)
        {
            Message = message;
            PropertyMessage = propertyMessage;
        }
    }
}
