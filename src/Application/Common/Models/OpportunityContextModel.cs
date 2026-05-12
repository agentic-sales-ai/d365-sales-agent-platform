namespace Application.Common.Models;

public class OpportunityContextModel
{
    public OpportunityModel Opportunity
        { get; set; } = new();

    public List<ActivityModel> Activities
        { get; set; } = [];
}