using Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services;

public class WorkflowExecutionBackgroundService
    : BackgroundService
{
    private readonly IServiceProvider
        _serviceProvider;

    public WorkflowExecutionBackgroundService(
        IServiceProvider serviceProvider)
    {
        _serviceProvider =
            serviceProvider;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope =
                _serviceProvider.CreateScope();

            var queue =
                scope.ServiceProvider
                    .GetRequiredService<
                        IWorkflowExecutionQueue>();

            var repository =
                scope.ServiceProvider
                    .GetRequiredService<
                        IWorkflowStateRepository>();

            var plannerRuntime =
                scope.ServiceProvider
                    .GetRequiredService<
                        IPlannerRuntime>();

            if (queue.TryDequeue(
                    out var workflowId))
            {
                var workflow =
                    await repository
                        .GetAsync(workflowId);

                if (workflow != null)
                {
                    await plannerRuntime
                        .ExecuteWorkflowAsync(
                            workflow);
                }
            }

            await Task.Delay(
                1000,
                stoppingToken);
        }
    }
}