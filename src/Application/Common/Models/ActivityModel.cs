namespace Application.Common.Models;

public class ActivityModel
{
    public Guid Id { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string ActivityType { get; set; } = string.Empty;
}