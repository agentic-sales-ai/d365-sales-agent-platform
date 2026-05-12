namespace Application.Common.Models;

public class WorkflowExecutionMetadataModel
{
    public Guid WorkflowId { get; set; }

    public DateTime ExecutedAtUtc { get; set; }

    public long DurationMs { get; set; }

    public string Status { get; set; } = string.Empty;

    public List<string> ExecutedAgents
        { get; set; } = [];
}