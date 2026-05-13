using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IPlannerRuntime
{
    Task ExecuteWorkflowAsync(
        WorkflowExecutionStateModel state);
}