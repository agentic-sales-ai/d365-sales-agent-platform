using Application.Common.Interfaces;
using Application.Common.Models;

namespace Infrastructure.Services;

public class ResilientExecutionService
    : IResilientExecutionService
{
    private readonly IWorkflowTelemetryService
        _telemetryService;

    private readonly RetryPolicyModel
        _retryPolicy = new();

    public ResilientExecutionService(
        IWorkflowTelemetryService telemetryService)
    {
        _telemetryService =
            telemetryService;
    }

    public async Task<object> ExecuteAsync(
        Func<Task<object>> action,
        string toolName,
        Guid workflowId)
    {
        var attempt = 0;

        while (true)
        {
            try
            {
                attempt++;

                return await action();
            }
            catch (Exception ex)
            {
                await _telemetryService
                    .TrackEventAsync(
                    new WorkflowExecutionEventModel
                    {
                        WorkflowId =
                            workflowId,

                        TimestampUtc =
                            DateTime.UtcNow,

                        EventType =
                            "StepFailed",

                        ToolName =
                            toolName,

                        Message =
                            $"Attempt {attempt} failed: {ex.Message}"
                    });

                if (attempt >=
                    _retryPolicy.MaxRetries)
                {
                    throw;
                }

                await _telemetryService
                    .TrackEventAsync(
                    new WorkflowExecutionEventModel
                    {
                        WorkflowId =
                            workflowId,

                        TimestampUtc =
                            DateTime.UtcNow,

                        EventType =
                            "RetryScheduled",

                        ToolName =
                            toolName,

                        Message =
                            $"Retrying {toolName}. Attempt {attempt + 1}"
                    });

                await Task.Delay(
                    _retryPolicy
                        .DelayMilliseconds);
            }
        }
    }
}