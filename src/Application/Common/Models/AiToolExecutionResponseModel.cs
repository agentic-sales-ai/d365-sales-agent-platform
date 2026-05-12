namespace Application.Common.Models;

public class AiToolExecutionResponseModel
{
    public string SelectedTool { get; set; }
        = string.Empty;

    public object? ToolResult { get; set; }

    public string Reasoning { get; set; }
        = string.Empty;
}