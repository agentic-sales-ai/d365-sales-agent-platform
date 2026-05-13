using Application.Common.Interfaces;
using Application.Common.Models;

namespace Infrastructure.Repositories;

public class InMemoryWorkflowStateRepository
    : IWorkflowStateRepository
{
    private static readonly Dictionary<
        Guid,
        WorkflowExecutionStateModel>
        _storage = [];

    public Task SaveAsync(
        WorkflowExecutionStateModel state)
    {
        _storage[state.WorkflowId] = state;

        return Task.CompletedTask;
    }

    public Task<WorkflowExecutionStateModel?>
        GetAsync(Guid workflowId)
    {
        _storage.TryGetValue(
            workflowId,
            out var state);

        return Task.FromResult(state);
    }
}