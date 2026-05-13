using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IWorkflowTelemetryService
{
    Task TrackEventAsync(
        WorkflowExecutionEventModel executionEvent);

    Task<List<WorkflowExecutionEventModel>>
        GetEventsAsync(Guid workflowId);
}