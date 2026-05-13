namespace Application.Common.Models;

public class RetryPolicyModel
{
    public int MaxRetries { get; set; }
        = 3;

    public int DelayMilliseconds
        { get; set; } = 1000;
}