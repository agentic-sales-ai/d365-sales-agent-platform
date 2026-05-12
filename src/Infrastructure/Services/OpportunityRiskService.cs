using Application.Common.Interfaces;
using Application.Common.Models;

namespace Infrastructure.Services;

public class OpportunityRiskService
    : IOpportunityRiskService
{
    private readonly IDataverseService
        _dataverseService;

    public OpportunityRiskService(
        IDataverseService dataverseService)
    {
        _dataverseService = dataverseService;
    }

    public async Task<OpportunityRiskAssessmentModel>
        AssessRiskAsync(Guid opportunityId)
    {
        var opportunity =
            await _dataverseService
                .GetOpportunityAsync(opportunityId);

        var activities =
            await _dataverseService
                .GetOpportunityActivitiesAsync(
                    opportunityId);

        var score = 0;

        var reasons = new List<string>();

        if (opportunity == null)
        {
            return new OpportunityRiskAssessmentModel
            {
                RiskScore = 100,
                RiskLevel = "Critical",
                Reasons =
                [
                    "Opportunity not found"
                ]
            };
        }

        if (opportunity.EstimatedValue > 10000)
        {
            score += 20;

            reasons.Add(
                "High-value opportunity");
        }

        if (activities.Count < 3)
        {
            score += 30;

            reasons.Add(
                "Low customer engagement");
        }

        if (opportunity.Status.Equals(
                "Open",
                StringComparison.OrdinalIgnoreCase))
        {
            score += 20;

            reasons.Add(
                "Opportunity still open");
        }

        string level;

        if (score >= 70)
        {
            level = "High";
        }
        else if (score >= 40)
        {
            level = "Medium";
        }
        else
        {
            level = "Low";
        }

        return new OpportunityRiskAssessmentModel
        {
            RiskScore = score,

            RiskLevel = level,

            Reasons = reasons
        };
    }
}