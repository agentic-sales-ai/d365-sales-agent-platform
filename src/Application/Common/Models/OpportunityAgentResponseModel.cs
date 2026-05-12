namespace Application.Common.Models;

public class OpportunityAgentResponseModel
{
    public WorkflowExecutionMetadataModel Metadata
        { get; set; } = new();

    public OpportunityInsightsModel Insights
        { get; set; } = new();

    public FollowUpEmailModel FollowUpEmail
        { get; set; } = new();

    public OpportunityRiskAssessmentModel RiskAssessment
        { get; set; } = new();
}