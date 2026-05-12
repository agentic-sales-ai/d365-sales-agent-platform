namespace Application.Common.Models;

public class OpportunityAgentResponseModel
{
    public OpportunityInsightsModel Insights
        { get; set; } = new();

    public FollowUpEmailModel FollowUpEmail
        { get; set; } = new();
}