namespace Application.Common.Models;

public class ExecutionPolicyResultModel
{
    public bool IsAllowed { get; set; }

    public string Reason { get; set; }
        = string.Empty;
}