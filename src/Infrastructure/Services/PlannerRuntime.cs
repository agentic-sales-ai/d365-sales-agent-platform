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

            var parameters =
                tool.GetDefaultParameters();

            var result =
                await tool.ExecuteAsync(
                    parameters);

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