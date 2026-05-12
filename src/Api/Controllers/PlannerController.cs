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

    public PlannerController(
        IPlannerRuntime plannerRuntime)
    {
        _plannerRuntime =
            plannerRuntime;
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
}