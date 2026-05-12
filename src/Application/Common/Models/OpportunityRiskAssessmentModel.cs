namespace Application.Common.Models;

public class OpportunityRiskAssessmentModel
{
    public int RiskScore { get; set; }

    public string RiskLevel { get; set; } = string.Empty;

    public List<string> Reasons { get; set; } = [];
}