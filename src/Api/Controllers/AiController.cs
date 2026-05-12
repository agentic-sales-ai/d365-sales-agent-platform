using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IAiToolOrchestrator
        _aiToolOrchestrator;

    public AiController(
        IAiToolOrchestrator aiToolOrchestrator)
    {
        _aiToolOrchestrator =
            aiToolOrchestrator;
    }

    [HttpPost("execute")]
    public async Task<IActionResult> Execute(
        AiToolExecutionRequestModel request)
    {
        try
        {
            var result =
                await _aiToolOrchestrator
                    .ExecuteAsync(request);

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