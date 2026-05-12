namespace Application.Common.Models;

public class PlannerExecutionResponseModel
{
    public Guid WorkflowId { get; set; }

    public DateTime ExecutedAtUtc
        { get; set; }

    public List<PlannerExecutionStepModel>
        Steps { get; set; } = [];
}