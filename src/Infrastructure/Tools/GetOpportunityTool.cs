using Application.Common.Interfaces;

namespace Infrastructure.Tools;

public class GetOpportunityTool
    : IAgentTool
{
    private readonly IDataverseService
        _dataverseService;

    public GetOpportunityTool(
        IDataverseService dataverseService)
    {
        _dataverseService = dataverseService;
    }

    public string Name =>
        "GetOpportunity";

    public string Description =>
        "Retrieves an opportunity from Dynamics 365 Sales.";

    public Dictionary<string, object>
        GetDefaultParameters()
    {
        return new Dictionary<string, object>
        {
            {
                "opportunityId",
                "a09c2889-a016-eb11-a813-002248029f77"
            }
        };
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
            await _dataverseService
                .GetOpportunityAsync(
                    opportunityId);

        return result!;
    }
}