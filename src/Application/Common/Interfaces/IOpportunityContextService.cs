using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IOpportunityContextService
{
    Task<OpportunityContextModel>
        BuildContextAsync(Guid opportunityId);
}