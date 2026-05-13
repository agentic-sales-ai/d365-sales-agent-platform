using Application.Common.Interfaces;

namespace Infrastructure.Tools;

public class GenerateOpportunityInsightsTool
    : IAgentTool
{
    private readonly IOpportunityInsightsService
        _insightsService;

    public GenerateOpportunityInsightsTool(
        IOpportunityInsightsService insightsService)
    {
        _insightsService =
            insightsService;
    }

    public string Name =>
        "GenerateOpportunityInsights";

    public string Description =>
        "Generates AI insights for an opportunity.";

    public Dictionary<string, object>
        GetDefaultParameters()
    {
        return [];
    }

    public async Task<object> ExecuteAsync(
        Dictionary<string, object> parameters)
    {
        if (!parameters.ContainsKey(
                "Opportunity"))
        {
            throw new Exception(
                "Missing Opportunity");
        }

        var opportunity =
            parameters["Opportunity"];

        var idProperty =
            opportunity
                .GetType()
                .GetProperty(
                    "Id");

        if (idProperty == null)
        {
            throw new Exception(
                "Id missing");
        }

        var opportunityId =
            Guid.Parse(
                idProperty
                    .GetValue(
                        opportunity)!
                    .ToString()!);

        var result =
            await _insightsService
                .GenerateInsightsAsync(
                    opportunityId);

        return result;
    }
}