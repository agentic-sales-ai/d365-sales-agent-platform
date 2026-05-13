using Application.Common.Interfaces;

namespace Infrastructure.Tools;

public class GenerateFollowUpEmailTool
    : IAgentTool
{
    private readonly IFollowUpEmailService
        _followUpEmailService;

    public GenerateFollowUpEmailTool(
        IFollowUpEmailService followUpEmailService)
    {
        _followUpEmailService =
            followUpEmailService;
    }

    public string Name =>
        "GenerateFollowUpEmail";

    public string Description =>
        "Generates AI follow-up email for an opportunity.";

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
            await _followUpEmailService
                .GenerateAsync(
                    opportunityId);

        return result;
    }
}