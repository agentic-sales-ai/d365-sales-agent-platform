using Application.Common.Interfaces;

namespace Infrastructure.Tools;

public class AssessOpportunityRiskTool
    : IAgentTool
{
    private readonly IOpportunityRiskService
        _riskService;

    public AssessOpportunityRiskTool(
        IOpportunityRiskService riskService)
    {
        _riskService =
            riskService;
    }

    public string Name =>
        "AssessOpportunityRisk";

    public string Description =>
        "Assesses opportunity risk.";

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
            await _riskService
                .AssessRiskAsync(
                    opportunityId);

        return result;
    }
}