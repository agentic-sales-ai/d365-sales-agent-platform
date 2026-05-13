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

    private readonly IExecutionPolicyService
        _policyService;

    private readonly IWorkflowStateRepository
        _repository;

    public PlannerRuntime(
        IAgentToolRegistry toolRegistry,
        IPlannerAiService plannerAiService,
        IWorkflowStateService workflowStateService,
        IExecutionPolicyService policyService,
        IWorkflowStateRepository repository)
    {
        _toolRegistry = toolRegistry;

        _plannerAiService =
            plannerAiService;

        _workflowStateService =
            workflowStateService;

        _policyService =
            policyService;

        _repository = repository;
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
            var policyResult =
                await _policyService
                    .ValidateAsync(planStep);

            if (!policyResult.IsAllowed)
            {
                var blockedStep =
                    new PlannerExecutionStepModel
                    {
                        StepNumber =
                            planStep.StepNumber,

                        ToolName =
                            planStep.ToolName,

                        Status = "Blocked",

                        Result =
                            policyResult.Reason
                    };

                _workflowStateService
                    .AddStepResult(
                        state,
                        blockedStep);

                continue;
            }

            var tool =
                _toolRegistry.GetTool(
                    planStep.ToolName);

            var parameters =
                new Dictionary<string, object>(
                    planStep.Parameters);

            foreach (var memoryKey
                in planStep.DependsOnMemoryKeys)
            {
                var memoryValue =
                    _workflowStateService
                        .GetMemoryValue(
                            state,
                            memoryKey);

                if (memoryValue != null)
                {
                    parameters[memoryKey] =
                        memoryValue;
                }
            }

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

            await _repository.SaveAsync(state);
        }

        await _repository.SaveAsync(state);

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