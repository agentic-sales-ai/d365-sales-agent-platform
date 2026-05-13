using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IWorkflowStateRepository
{
    Task SaveAsync(
        WorkflowExecutionStateModel state);

    Task<WorkflowExecutionStateModel?>
        GetAsync(Guid workflowId);
}