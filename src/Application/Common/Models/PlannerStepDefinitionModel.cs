namespace Application.Common.Models;

public class PlannerStepDefinitionModel
{
    public int StepNumber { get; set; }

    public string ToolName { get; set; }
        = string.Empty;

    public Dictionary<string, object>
        Parameters { get; set; } = [];

    public List<string>
        DependsOnMemoryKeys { get; set; } = [];

    public string? ConditionMemoryKey
    {
        get;
        set;
    }

    public string? ConditionEquals
    {
        get;
        set;
    }
}