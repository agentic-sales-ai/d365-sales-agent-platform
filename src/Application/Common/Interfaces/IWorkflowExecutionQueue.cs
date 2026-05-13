namespace Application.Common.Interfaces;

public interface IWorkflowExecutionQueue
{
    void Enqueue(Guid workflowId);

    bool TryDequeue(
        out Guid workflowId);
}