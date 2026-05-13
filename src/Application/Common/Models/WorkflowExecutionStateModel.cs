namespace Application.Common.Models;

public class WorkflowExecutionStateModel
{
    public Guid WorkflowId { get; set; }

    public string UserPrompt { get; set; }
        = string.Empty;

    public DateTime StartedAtUtc
        { get; set; }

    public List<PlannerExecutionStepModel>
        Steps { get; set; } = [];

    public Dictionary<string, object>
        SharedMemory { get; set; } = [];
}