using Application.Common.Interfaces;
using Application.Common.Models;

namespace Infrastructure.Services;

public class ExecutionPolicyService
    : IExecutionPolicyService
{
    public Task<ExecutionPolicyResultModel>
        ValidateAsync(
            PlannerStepDefinitionModel step)
    {
        if (string.IsNullOrWhiteSpace(
                step.ToolName))
        {
            return Task.FromResult(
                new ExecutionPolicyResultModel
                {
                    IsAllowed = false,

                    Reason =
                        "Tool name missing."
                });
        }

        var blockedTools =
            new List<string>
            {
                "DeleteOpportunity",
                "BulkEmailCustomers"
            };

        if (blockedTools.Contains(
                step.ToolName))
        {
            return Task.FromResult(
                new ExecutionPolicyResultModel
                {
                    IsAllowed = false,

                    Reason =
                        $"Tool {step.ToolName} is blocked by policy."
                });
        }

        var approvalRequiredTools =
            new List<string>
            {
                "GenerateFollowUpEmail"
            };

        if (approvalRequiredTools
            .Contains(step.ToolName))
        {
            return Task.FromResult(
                new ExecutionPolicyResultModel
                {
                    IsAllowed = false,

                    Reason =
                        $"Approval required for {step.ToolName}."
                });
        }

        return Task.FromResult(
            new ExecutionPolicyResultModel
            {
                IsAllowed = true,

                Reason = "Allowed"
            });
    }
}