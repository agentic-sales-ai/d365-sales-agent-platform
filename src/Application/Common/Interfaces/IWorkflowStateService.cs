using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IWorkflowStateService
{
    WorkflowExecutionStateModel CreateState(
        string userPrompt);

    void AddStepResult(
        WorkflowExecutionStateModel state,
        PlannerExecutionStepModel step);

    void SetMemoryValue(
        WorkflowExecutionStateModel state,
        string key,
        object value);

    object? GetMemoryValue(
        WorkflowExecutionStateModel state,
        string key);
}