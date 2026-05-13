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

    private readonly IWorkflowTelemetryService
        _telemetryService;

    public PlannerRuntime(
        IAgentToolRegistry toolRegistry,
        IPlannerAiService plannerAiService,
        IWorkflowStateService workflowStateService,
        IExecutionPolicyService policyService,
        IWorkflowStateRepository repository,
        IWorkflowTelemetryService telemetryService)
    {
        _toolRegistry = toolRegistry;

        _plannerAiService =
            plannerAiService;

        _workflowStateService =
            workflowStateService;

        _policyService =
            policyService;

        _repository = repository;

        _telemetryService =
            telemetryService;
    }

    public async Task ExecuteWorkflowAsync(
        WorkflowExecutionStateModel state)
    {
        state.Status = "Running";

        await _repository.SaveAsync(state);

        await _telemetryService.TrackEventAsync(
            new WorkflowExecutionEventModel
            {
                WorkflowId =
                    state.WorkflowId,

                TimestampUtc =
                    DateTime.UtcNow,

                EventType =
                    "WorkflowStarted",

                Message =
                    "Workflow execution started."
            });

        var plan =
            await _plannerAiService
                .GeneratePlanAsync(
                    state.UserPrompt);

        foreach (var planStep in plan.Steps)
        {
            await _telemetryService.TrackEventAsync(
                new WorkflowExecutionEventModel
                {
                    WorkflowId =
                        state.WorkflowId,

                    TimestampUtc =
                        DateTime.UtcNow,

                    EventType =
                        "StepStarted",

                    ToolName =
                        planStep.ToolName,

                    Message =
                        $"Executing {planStep.ToolName}"
                });

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

                await _telemetryService
                    .TrackEventAsync(
                    new WorkflowExecutionEventModel
                    {
                        WorkflowId =
                            state.WorkflowId,

                        TimestampUtc =
                            DateTime.UtcNow,

                        EventType =
                            "StepBlocked",

                        ToolName =
                            planStep.ToolName,

                        Message =
                            policyResult.Reason
                    });

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

            await _telemetryService
                .TrackEventAsync(
                new WorkflowExecutionEventModel
                {
                    WorkflowId =
                        state.WorkflowId,

                    TimestampUtc =
                        DateTime.UtcNow,

                    EventType =
                        "StepCompleted",

                    ToolName =
                        planStep.ToolName,

                    Message =
                        $"{planStep.ToolName} completed successfully."
                });
        }

        state.Status = "Completed";

        await _repository.SaveAsync(state);

        await _telemetryService.TrackEventAsync(
            new WorkflowExecutionEventModel
            {
                WorkflowId =
                    state.WorkflowId,

                TimestampUtc =
                    DateTime.UtcNow,

                EventType =
                    "WorkflowCompleted",

                Message =
                    "Workflow execution completed."
            });
    }
}