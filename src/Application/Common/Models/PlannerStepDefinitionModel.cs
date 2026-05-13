namespace Application.Common.Models;

public class PlannerStepDefinitionModel
{
    public int StepNumber { get; set; }

    public string ToolName { get; set; }
        = string.Empty;

    public Dictionary<string, object>
        Parameters { get; set; } = [];
}