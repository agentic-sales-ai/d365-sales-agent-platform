using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlannerController : ControllerBase
{
    private readonly IPlannerRuntime
        _plannerRuntime;

    private readonly IWorkflowStateRepository
        _repository;

    public PlannerController(
        IPlannerRuntime plannerRuntime,
        IWorkflowStateRepository repository)
    {
        _plannerRuntime =
            plannerRuntime;

        _repository = repository;
    }

    [HttpPost("execute")]
    public async Task<IActionResult> Execute(
        AiToolExecutionRequestModel request)
    {
        try
        {
            var result =
                await _plannerRuntime
                    .ExecutePlanAsync(
                        request.UserPrompt);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = ex.Message
            });
        }
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