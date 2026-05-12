using System.Text.Json;
using Application.Common.Interfaces;
using Application.Common.Models;

namespace Infrastructure.Services;

public class AiToolOrchestrator
    : IAiToolOrchestrator
{
    private readonly IAgentToolRegistry
        _toolRegistry;

    public AiToolOrchestrator(
        IAgentToolRegistry toolRegistry)
    {
        _toolRegistry = toolRegistry;
    }

    public async Task<AiToolExecutionResponseModel>
        ExecuteAsync(
            AiToolExecutionRequestModel request)
    {
        var prompt =
            request.UserPrompt
                .ToLowerInvariant();

        if (prompt.Contains("opportunity"))
        {
            var tool =
                _toolRegistry.GetTool(
                    "GetOpportunity");

            var parameters =
                new Dictionary<string, object>
                {
                    {
                        "opportunityId",
                        "a09c2889-a016-eb11-a813-002248029f77"
                    }
                };

            var result =
                await tool.ExecuteAsync(
                    parameters);

            return new AiToolExecutionResponseModel
            {
                SelectedTool =
                    tool.Name,

                ToolResult =
                    result,

                Reasoning =
                    "The user asked about an opportunity, so the GetOpportunity tool was selected."
            };
        }

        return new AiToolExecutionResponseModel
        {
            SelectedTool = "None",

            Reasoning =
                "No matching tool found."
        };
    }
}