using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IOpportunityInsightsService
{
    Task<OpportunityInsightsModel> GenerateInsightsAsync(
        Guid opportunityId);
}