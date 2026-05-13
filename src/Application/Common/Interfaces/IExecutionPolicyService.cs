using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IExecutionPolicyService
{
    Task<ExecutionPolicyResultModel>
        ValidateAsync(
            PlannerStepDefinitionModel step);
}