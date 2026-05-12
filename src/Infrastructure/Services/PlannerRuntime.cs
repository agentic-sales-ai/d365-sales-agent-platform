using Application.Common.Interfaces;
using Application.Common.Models;

namespace Infrastructure.Services;

public class PlannerRuntime
    : IPlannerRuntime
{
    private readonly IAgentToolRegistry
        _toolRegistry;

    private readonly IOpportunityInsightsService
        _insightsService;

    private readonly IFollowUpEmailService
        _followUpEmailService;

    private readonly IOpportunityRiskService
        _riskService;

    public PlannerRuntime(
        IAgentToolRegistry toolRegistry,
        IOpportunityInsightsService insightsService,
        IFollowUpEmailService followUpEmailService,
        IOpportunityRiskService riskService)
    {
        _toolRegistry = toolRegistry;

        _insightsService = insightsService;

        _followUpEmailService =
            followUpEmailService;

        _riskService = riskService;
    }

    public async Task<PlannerExecutionResponseModel>
        ExecutePlanAsync(string userPrompt)
    {
        var workflowId = Guid.NewGuid();

        var steps =
            new List<PlannerExecutionStepModel>();

        var opportunityId =
            Guid.Parse(
                "a09c2889-a016-eb11-a813-002248029f77");

        var getOpportunityTool =
            _toolRegistry.GetTool(
                "GetOpportunity");

        var opportunityResult =
            await getOpportunityTool.ExecuteAsync(
                new Dictionary<string, object>
                {
                    {
                        "opportunityId",
                        opportunityId
                    }
                });

        steps.Add(
            new PlannerExecutionStepModel
            {
                StepNumber = 1,

                ToolName = "GetOpportunity",

                Status = "Completed",

                Result = opportunityResult
            });

        var insights =
            await _insightsService
                .GenerateInsightsAsync(
                    opportunityId);

        steps.Add(
            new PlannerExecutionStepModel
            {
                StepNumber = 2,

                ToolName = "GenerateInsights",

                Status = "Completed",

                Result = insights
            });

        var risk =
            await _riskService
                .AssessRiskAsync(
                    opportunityId);

        steps.Add(
            new PlannerExecutionStepModel
            {
                StepNumber = 3,

                ToolName = "AssessRisk",

                Status = "Completed",

                Result = risk
            });

        var email =
            await _followUpEmailService
                .GenerateAsync(
                    opportunityId);

        steps.Add(
            new PlannerExecutionStepModel
            {
                StepNumber = 4,

                ToolName = "GenerateFollowUpEmail",

                Status = "Completed",

                Result = email
            });

        return new PlannerExecutionResponseModel
        {
            WorkflowId = workflowId,

            ExecutedAtUtc =
                DateTime.UtcNow,

            Steps = steps
        };
    }
}