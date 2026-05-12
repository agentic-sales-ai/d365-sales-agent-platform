using Application.Common.Interfaces;
using Application.Common.Models;

namespace Infrastructure.Services;

public class OpportunityContextService
    : IOpportunityContextService
{
    private readonly IDataverseService
        _dataverseService;

    public OpportunityContextService(
        IDataverseService dataverseService)
    {
        _dataverseService = dataverseService;
    }

    public async Task<OpportunityContextModel>
        BuildContextAsync(Guid opportunityId)
    {
        var opportunity =
            await _dataverseService
                .GetOpportunityAsync(opportunityId);

        var activities =
            await _dataverseService
                .GetOpportunityActivitiesAsync(
                    opportunityId);

        return new OpportunityContextModel
        {
            Opportunity =
                opportunity ?? new OpportunityModel(),

            Activities = activities
        };
    }
}