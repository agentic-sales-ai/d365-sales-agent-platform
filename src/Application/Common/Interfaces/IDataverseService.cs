using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IDataverseService
{
    Task<OpportunityModel?> GetOpportunityAsync(Guid opportunityId);

    Task<List<ActivityModel>> GetOpportunityActivitiesAsync(
        Guid opportunityId);
}