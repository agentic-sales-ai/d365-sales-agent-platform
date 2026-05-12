using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IOpportunityRiskService
{
    Task<OpportunityRiskAssessmentModel>
        AssessRiskAsync(Guid opportunityId);
}