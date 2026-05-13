using Application.Common.Interfaces;
using Application.Common.Models;

namespace Infrastructure.Repositories;

public class InMemoryWorkflowApprovalRepository
    : IWorkflowApprovalRepository
{
    private static readonly Dictionary<
        Guid,
        WorkflowApprovalModel>
        _storage = [];

    public Task CreateAsync(
        WorkflowApprovalModel approval)
    {
        _storage[approval.ApprovalId] =
            approval;

        return Task.CompletedTask;
    }

    public Task<WorkflowApprovalModel?>
        GetAsync(Guid approvalId)
    {
        _storage.TryGetValue(
            approvalId,
            out var approval);

        return Task.FromResult(approval);
    }

    public Task UpdateAsync(
        WorkflowApprovalModel approval)
    {
        _storage[approval.ApprovalId] =
            approval;

        return Task.CompletedTask;
    }
}