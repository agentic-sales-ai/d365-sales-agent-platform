namespace Application.Common.Options;

public class OpenAIOptions
{
    public const string SectionName = "OpenAI";

    public string Endpoint { get; set; } = string.Empty;

    public string Deployment { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;
}