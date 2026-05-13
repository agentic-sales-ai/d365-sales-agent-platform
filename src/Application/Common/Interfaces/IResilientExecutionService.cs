namespace Application.Common.Interfaces;

public interface IResilientExecutionService
{
    Task<object> ExecuteAsync(
        Func<Task<object>> action,
        string toolName,
        Guid workflowId);
}