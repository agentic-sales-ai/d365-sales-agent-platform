namespace Application.Common.Models;

public class WorkflowExecutionEventModel
{
    public Guid WorkflowId { get; set; }

    public DateTime TimestampUtc
        { get; set; }

    public string EventType { get; set; }
        = string.Empty;

    public string Message { get; set; }
        = string.Empty;

    public string? ToolName { get; set; }
}