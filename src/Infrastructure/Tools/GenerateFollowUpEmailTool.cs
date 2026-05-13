using Application.Common.Interfaces;

namespace Infrastructure.Tools;

public class GenerateFollowUpEmailTool
    : IAgentTool
{
    private readonly IFollowUpEmailService
        _emailService;

    public GenerateFollowUpEmailTool(
        IFollowUpEmailService emailService)
    {
        _emailService = emailService;
    }

    public string Name =>
        "GenerateFollowUpEmail";

    public string Description =>
        "Generates AI follow-up email for opportunity.";

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
            await _emailService
                .GenerateAsync(
                    opportunityId);

        return result;
    }
}