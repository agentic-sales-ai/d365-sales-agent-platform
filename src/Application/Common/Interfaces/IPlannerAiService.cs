using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IPlannerAiService
{
    Task<PlannerPlanModel>
        GeneratePlanAsync(
            string userPrompt);
}