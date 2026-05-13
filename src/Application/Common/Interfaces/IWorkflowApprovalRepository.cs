using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IWorkflowApprovalRepository
{
    Task CreateAsync(
        WorkflowApprovalModel approval);

    Task<WorkflowApprovalModel?>
        GetAsync(Guid approvalId);

    Task UpdateAsync(
        WorkflowApprovalModel approval);

    Task<WorkflowApprovalModel?>
        GetApprovedAsync(
            Guid workflowId,
            string toolName);

    Task<WorkflowApprovalModel?>
        GetPendingAsync(
            Guid workflowId);
}