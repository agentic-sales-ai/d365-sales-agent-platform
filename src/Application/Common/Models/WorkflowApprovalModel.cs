namespace Application.Common.Models;

public class WorkflowApprovalModel
{
    public Guid ApprovalId { get; set; }

    public Guid WorkflowId { get; set; }

    public string ToolName { get; set; }
        = string.Empty;

    public string Status { get; set; }
        = "Pending";

    public DateTime CreatedAtUtc
        { get; set; }

    public DateTime? ResolvedAtUtc
        { get; set; }

    public string? DecisionNotes
        { get; set; }
}