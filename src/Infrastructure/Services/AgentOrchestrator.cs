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

    public AgentOrchestrator(
        IOpportunityInsightsService insightsService,
        IFollowUpEmailService followUpEmailService)
    {
        _insightsService = insightsService;

        _followUpEmailService =
            followUpEmailService;
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

        return new OpportunityAgentResponseModel
        {
            Insights = insights,

            FollowUpEmail = email
        };
    }
}