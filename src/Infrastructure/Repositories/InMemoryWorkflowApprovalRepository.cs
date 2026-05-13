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

    public Task<WorkflowApprovalModel?>
        GetApprovedAsync(
            Guid workflowId,
            string toolName)
    {
        var approval =
            _storage.Values
                .FirstOrDefault(a =>
                    a.WorkflowId ==
                        workflowId
                    && a.ToolName ==
                        toolName
                    && a.Status ==
                        "Approved");

        return Task.FromResult(approval);
    }

    public Task<WorkflowApprovalModel?>
    GetPendingAsync(
        Guid workflowId)
{
    var approval =
        _storage.Values
            .FirstOrDefault(a =>
                a.WorkflowId ==
                    workflowId
                && a.Status ==
                    "Pending");

    return Task.FromResult(approval);
}
}