using Application.Common.Interfaces;
using Application.Common.Models;

namespace Infrastructure.Services;

public class PlannerRuntime
    : IPlannerRuntime
{
    private readonly IAgentToolRegistry
        _toolRegistry;

    private readonly IPlannerAiService
        _plannerAiService;

    public PlannerRuntime(
        IAgentToolRegistry toolRegistry,
        IPlannerAiService plannerAiService)
    {
        _toolRegistry = toolRegistry;

        _plannerAiService =
            plannerAiService;
    }

    public async Task<PlannerExecutionResponseModel>
        ExecutePlanAsync(string userPrompt)
    {
        var workflowId = Guid.NewGuid();

        var executionSteps =
            new List<PlannerExecutionStepModel>();

        var plan =
            await _plannerAiService
                .GeneratePlanAsync(
                    userPrompt);

        foreach (var planStep in plan.Steps)
        {
            var tool =
                _toolRegistry.GetTool(
                    planStep.ToolName);

            object? result = null;

            if (tool.Name == "GetOpportunity")
            {
                result =
                    await tool.ExecuteAsync(
                        new Dictionary<string, object>
                        {
                            {
                                "opportunityId",
                                "a09c2889-a016-eb11-a813-002248029f77"
                            }
                        });
            }

            executionSteps.Add(
                new PlannerExecutionStepModel
                {
                    StepNumber =
                        planStep.StepNumber,

                    ToolName =
                        planStep.ToolName,

                    Status = "Completed",

                    Result = result
                });
        }

        return new PlannerExecutionResponseModel
        {
            WorkflowId = workflowId,

            ExecutedAtUtc =
                DateTime.UtcNow,

            Steps = executionSteps
        };
    }
}