using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IAiToolOrchestrator
{
    Task<AiToolExecutionResponseModel>
        ExecuteAsync(
            AiToolExecutionRequestModel request);
}