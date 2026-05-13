using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlannerController : ControllerBase
{
    private readonly IWorkflowStateService
        _workflowStateService;

    private readonly IWorkflowStateRepository
        _repository;

    private readonly IWorkflowExecutionQueue
        _queue;

    public PlannerController(
        IWorkflowStateService workflowStateService,
        IWorkflowStateRepository repository,
        IWorkflowExecutionQueue queue)
    {
        _workflowStateService =
            workflowStateService;

        _repository = repository;

        _queue = queue;
    }

    [HttpPost("execute")]
    public async Task<IActionResult> Execute(
        AiToolExecutionRequestModel request)
    {
        var workflow =
            _workflowStateService
                .CreateState(
                    request.UserPrompt);

        await _repository
            .SaveAsync(workflow);

        _queue.Enqueue(
            workflow.WorkflowId);

        return Ok(new
        {
            workflow.WorkflowId,

            workflow.Status
        });
    }

    [HttpGet("workflow/{workflowId}")]
    public async Task<IActionResult>
        GetWorkflow(Guid workflowId)
    {
        var workflow =
            await _repository
                .GetAsync(workflowId);

        if (workflow == null)
        {
            return NotFound();
        }

        return Ok(workflow);
    }
}