using Application.Common.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

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
            IOpportunityRiskService,
            OpportunityRiskService>();
        
        return services;
    }
}