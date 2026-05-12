namespace Application.Common.Models;

public class OpportunityModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal EstimatedValue { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}