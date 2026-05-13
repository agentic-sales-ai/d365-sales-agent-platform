using Application.Common.Interfaces;
using Application.Common.Models;

namespace Infrastructure.Services;

public class WorkflowTelemetryService
    : IWorkflowTelemetryService
{
    private static readonly List<
        WorkflowExecutionEventModel>
        _events = [];

    public Task TrackEventAsync(
        WorkflowExecutionEventModel executionEvent)
    {
        _events.Add(executionEvent);

        return Task.CompletedTask;
    }

    public Task<List<WorkflowExecutionEventModel>>
        GetEventsAsync(Guid workflowId)
    {
        var result =
            _events
                .Where(e =>
                    e.WorkflowId ==
                    workflowId)
                .OrderBy(e =>
                    e.TimestampUtc)
                .ToList();

        return Task.FromResult(result);
    }
}