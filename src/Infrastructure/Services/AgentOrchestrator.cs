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
        var insights =
            await _insightsService
                .GenerateInsightsAsync(
                    opportunityId);

        var email =
            await _followUpEmailService
                .GenerateAsync(
                    opportunityId);

        var risk =
            await _riskService
                .AssessRiskAsync(
                    opportunityId);

        return new OpportunityAgentResponseModel
        {
            Insights = insights,

            FollowUpEmail = email,

            RiskAssessment = risk
        };
    }
}