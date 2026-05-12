namespace Application.Common.Models;

public class ToolExecutionRequestModel
{
    public string ToolName { get; set; }
        = string.Empty;

    public Dictionary<string, object> Parameters
        { get; set; } = [];
}