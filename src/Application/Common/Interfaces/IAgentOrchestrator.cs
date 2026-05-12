using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IAgentOrchestrator
{
    Task<OpportunityAgentResponseModel>
        ExecuteOpportunityWorkflowAsync(
            Guid opportunityId);
}