using Application.Common.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Tools;
using Infrastructure.Repositories;
using Infrastructure.Queues;

namespace Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<
            IDataverseService,
            DataverseService>();

        services.AddScoped<
            IOpportunityInsightsService,
            OpportunityInsightsService>();

        services.AddScoped<
            IFollowUpEmailService,
            FollowUpEmailService>();

        services.AddScoped<
            IEmailDeliveryService,
            GraphEmailDeliveryService>();
        
        services.AddScoped<
            IAgentOrchestrator,
            AgentOrchestrator>();

        services.AddScoped<
            IAgentToolRegistry,
            AgentToolRegistry>();
        
        services.AddScoped<
            IOpportunityRiskService,
            OpportunityRiskService>();
        
        services.AddScoped<
            IOpportunityContextService,
            OpportunityContextService>();
        
        services.AddScoped<
            IAgentTool,
            GetOpportunityTool>();
        
        services.AddScoped<
            IAiToolOrchestrator,
            AiToolOrchestrator>();

        services.AddScoped<
            IPlannerRuntime,
            PlannerRuntime>();

        services.AddScoped<
            IPlannerAiService,
            PlannerAiService>();

        services.AddScoped<
            IWorkflowStateService,
            WorkflowStateService>();

        services.AddScoped<
            IAgentTool,
            GenerateOpportunityInsightsTool>();

        services.AddScoped<
            IAgentTool,
            AssessOpportunityRiskTool>();

        services.AddScoped<
            IAgentTool,
            GenerateFollowUpEmailTool>();

        services.AddScoped<
            IExecutionPolicyService,
            ExecutionPolicyService>();

        services.AddSingleton<
            IWorkflowStateRepository,
            InMemoryWorkflowStateRepository>();
        
        services.AddSingleton<
            IWorkflowExecutionQueue,
            InMemoryWorkflowExecutionQueue>();

        services.AddHostedService<
            WorkflowExecutionBackgroundService>();

        services.AddSingleton<
            IWorkflowTelemetryService,
            WorkflowTelemetryService>();

        services.AddScoped<
            IResilientExecutionService,
            ResilientExecutionService>();

        return services;
    }
}