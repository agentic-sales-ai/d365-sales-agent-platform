using Application.Common.Interfaces;
using Application.Common.Models;

namespace Infrastructure.Services;

public class WorkflowStateService
    : IWorkflowStateService
{
    public WorkflowExecutionStateModel
        CreateState(string userPrompt)
    {
        return new WorkflowExecutionStateModel
        {
            WorkflowId = Guid.NewGuid(),

            UserPrompt = userPrompt,

            StartedAtUtc =
                DateTime.UtcNow
        };
    }

    public void AddStepResult(
        WorkflowExecutionStateModel state,
        PlannerExecutionStepModel step)
    {
        state.Steps.Add(step);
    }

    public void SetMemoryValue(
        WorkflowExecutionStateModel state,
        string key,
        object value)
    {
        state.SharedMemory[key] = value;
    }

    public object? GetMemoryValue(
        WorkflowExecutionStateModel state,
        string key)
    {
        if (state.SharedMemory.ContainsKey(key))
        {
            return state.SharedMemory[key];
        }

        return null;
    }
}