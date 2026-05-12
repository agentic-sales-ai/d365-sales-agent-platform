namespace Application.Common.Models;

public class OpportunityInsightsModel
{
    public string Summary { get; set; } = string.Empty;

    public List<string> Risks { get; set; } = [];

    public List<string> NextActions { get; set; } = [];
}