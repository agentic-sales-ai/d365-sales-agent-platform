using System.Diagnostics;
using Application.Common.Interfaces;
using Application.Common.Models;

namespace Infrastructure.Services;

public class AgentOrchestrator
    : IAgentOrchestrator
{
    private readonly IOpportunityInsightsService
        _insightsService;

    private readonly IFollowUpEmailService
        _followUpEmailService;

    private readonly IOpportunityRiskService
        _riskService;

    public AgentOrchestrator(
        IOpportunityInsightsService insightsService,
        IFollowUpEmailService followUpEmailService,
        IOpportunityRiskService riskService)
    {
        _insightsService = insightsService;

        _followUpEmailService =
            followUpEmailService;

        _riskService = riskService;
    }

    public async Task<OpportunityAgentResponseModel>
        ExecuteOpportunityWorkflowAsync(
            Guid opportunityId)
    {
        var stopwatch = Stopwatch.StartNew();

        var workflowId = Guid.NewGuid();

        var executedAgents = new List<string>();

        var insights =
            await _insightsService
                .GenerateInsightsAsync(
                    opportunityId);

        executedAgents.Add(
            "OpportunityInsightsAgent");

        var email =
            await _followUpEmailService
                .GenerateAsync(
                    opportunityId);

        executedAgents.Add(
            "FollowUpEmailAgent");

        var risk =
            await _riskService
                .AssessRiskAsync(
                    opportunityId);

        executedAgents.Add(
            "OpportunityRiskAgent");

        stopwatch.Stop();

        return new OpportunityAgentResponseModel
        {
            Metadata =
                new WorkflowExecutionMetadataModel
                {
                    WorkflowId = workflowId,

                    ExecutedAtUtc =
                        DateTime.UtcNow,

                    DurationMs =
                        stopwatch.ElapsedMilliseconds,

                    Status = "Completed",

                    ExecutedAgents =
                        executedAgents
                },

            Insights = insights,

            FollowUpEmail = email,

            RiskAssessment = risk
        };
    }
}