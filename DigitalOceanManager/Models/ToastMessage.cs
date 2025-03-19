namespace DigitalOceanManager.Models;

public class ToastMessage
{
    public string Message { get; }
    public string Type { get; }
    public string Title { get; }
    public int Duration { get; }
    public DateTime Timestamp { get; }

    public ToastMessage(string message, string type, string title, int duration, DateTime timestamp)
    {
        Message = message;
        Type = type;
        Title = title;
        Duration = duration;
        Timestamp = timestamp;
    }
}