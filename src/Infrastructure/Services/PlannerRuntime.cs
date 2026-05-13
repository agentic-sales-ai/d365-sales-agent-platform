using Microsoft.Extensions.DependencyInjection;
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

    private readonly IResilientExecutionService
        _resilientExecutionService;

    private readonly IServiceProvider
    _serviceProvider;

    public PlannerRuntime(
        IAgentToolRegistry toolRegistry,
        IPlannerAiService plannerAiService,
        IWorkflowStateService workflowStateService,
        IExecutionPolicyService policyService,
        IWorkflowStateRepository repository,
        IWorkflowTelemetryService telemetryService,
        IResilientExecutionService
            resilientExecutionService,
        IServiceProvider serviceProvider)
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

        _resilientExecutionService =
            resilientExecutionService;

        _serviceProvider =
            serviceProvider;
    }

    public async Task ExecuteWorkflowAsync(
        WorkflowExecutionStateModel state)
    {
        try
        {
            state.Status = "Running";

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
                await _telemetryService
                    .TrackEventAsync(
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
    if (policyResult.Reason
        .Contains("Approval required"))
    {
        var approval =
            new WorkflowApprovalModel
            {
                ApprovalId =
                    Guid.NewGuid(),

                WorkflowId =
                    state.WorkflowId,

                ToolName =
                    planStep.ToolName,

                CreatedAtUtc =
                    DateTime.UtcNow
            };

        var approvalRepository =
            _serviceProvider
                .GetRequiredService<
                    IWorkflowApprovalRepository>();

        await approvalRepository
            .CreateAsync(approval);

        state.Status =
            "WaitingForApproval";

        await _repository
            .SaveAsync(state);

        await _telemetryService
            .TrackEventAsync(
            new WorkflowExecutionEventModel
            {
                WorkflowId =
                    state.WorkflowId,

                TimestampUtc =
                    DateTime.UtcNow,

                EventType =
                    "WorkflowSuspended",

                ToolName =
                    planStep.ToolName,

                Message =
                    $"Approval required for {planStep.ToolName}"
            });

        return;
    }

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

                try
                {
                    var result =
                        await _resilientExecutionService
                            .ExecuteAsync(
                            async () =>
                            {
                                return await tool
                                    .ExecuteAsync(
                                        parameters);
                            },
                            planStep.ToolName,
                            state.WorkflowId);

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

                    await _repository
                        .SaveAsync(state);

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
                catch (Exception ex)
                {
                    var failedStep =
                        new PlannerExecutionStepModel
                        {
                            StepNumber =
                                planStep.StepNumber,

                            ToolName =
                                planStep.ToolName,

                            Status = "Failed",

                            Result =
                                ex.Message
                        };

                    _workflowStateService
                        .AddStepResult(
                            state,
                            failedStep);

                    await _repository
                        .SaveAsync(state);

                    await _telemetryService
                        .TrackEventAsync(
                        new WorkflowExecutionEventModel
                        {
                            WorkflowId =
                                state.WorkflowId,

                            TimestampUtc =
                                DateTime.UtcNow,

                            EventType =
                                "StepFailed",

                            ToolName =
                                planStep.ToolName,

                            Message =
                                ex.Message
                        });

                    throw;
                }
            }

            state.Status = "Completed";

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
                        "WorkflowCompleted",

                    Message =
                        "Workflow execution completed."
                });
        }
        catch (Exception ex)
        {
            state.Status = "Failed";

            state.FailureReason =
                ex.Message;

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
                        "WorkflowFailed",

                    Message =
                        ex.Message
                });
        }
    }
}