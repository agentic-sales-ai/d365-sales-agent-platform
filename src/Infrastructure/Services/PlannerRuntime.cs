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

    private readonly IWorkflowStateService
        _workflowStateService;

    public PlannerRuntime(
        IAgentToolRegistry toolRegistry,
        IPlannerAiService plannerAiService,
        IWorkflowStateService workflowStateService)
    {
        _toolRegistry = toolRegistry;

        _plannerAiService =
            plannerAiService;

        _workflowStateService =
            workflowStateService;
    }

    public async Task<PlannerExecutionResponseModel>
        ExecutePlanAsync(string userPrompt)
    {
        var state =
            _workflowStateService
                .CreateState(userPrompt);

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
                planStep.Parameters;

            var result =
                await tool.ExecuteAsync(
                    parameters);

            var executionStep =
                new PlannerExecutionStepModel
                {
                    StepNumber =
                        planStep.StepNumber,

                    ToolName =
                        planStep.ToolName,

                    Status = "Completed",

                    Result = result
                };

            _workflowStateService
                .AddStepResult(
                    state,
                    executionStep);

            _workflowStateService
                .SetMemoryValue(
                    state,
                    planStep.ToolName,
                    result);
        }

        return new PlannerExecutionResponseModel
        {
            WorkflowId =
                state.WorkflowId,

            ExecutedAtUtc =
                state.StartedAtUtc,

            Steps =
                state.Steps
        };
    }
}