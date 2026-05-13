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
        _riskService = riskService;
    }

    public string Name =>
        "AssessOpportunityRisk";

    public string Description =>
        "Assesses sales opportunity risk.";

    public Dictionary<string, object>
        GetDefaultParameters()
    {
        return [];
    }

    public async Task<object> ExecuteAsync(
        Dictionary<string, object> parameters)
    {
        if (!parameters.ContainsKey(
                "opportunityId"))
        {
            throw new Exception(
                "Missing opportunityId");
        }

        var opportunityId =
            Guid.Parse(
                parameters["opportunityId"]
                    .ToString()!);

        var result =
            await _riskService
                .AssessRiskAsync(
                    opportunityId);

        return result;
    }
}