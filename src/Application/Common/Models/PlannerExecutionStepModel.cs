namespace Application.Common.Models;

public class PlannerExecutionStepModel
{
    public int StepNumber { get; set; }

    public string ToolName { get; set; }
        = string.Empty;

    public string Status { get; set; }
        = string.Empty;

    public object? Result { get; set; }
}