using System.Collections.Concurrent;
using Application.Common.Interfaces;

namespace Infrastructure.Queues;

public class InMemoryWorkflowExecutionQueue
    : IWorkflowExecutionQueue
{
    private readonly ConcurrentQueue<Guid>
        _queue = new();

    public void Enqueue(Guid workflowId)
    {
        _queue.Enqueue(workflowId);
    }

    public bool TryDequeue(
        out Guid workflowId)
    {
        return _queue.TryDequeue(
            out workflowId);
    }
}