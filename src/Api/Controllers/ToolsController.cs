using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToolsController : ControllerBase
{
    private readonly IAgentToolRegistry
        _toolRegistry;

    public ToolsController(
        IAgentToolRegistry toolRegistry)
    {
        _toolRegistry = toolRegistry;
    }

    [HttpGet]
    public IActionResult GetTools()
    {
        var tools =
            _toolRegistry
                .GetTools()
                .Select(t => new
                {
                    t.Name,
                    t.Description
                });

        return Ok(tools);
    }

    [HttpPost("execute")]
    public async Task<IActionResult> ExecuteTool(
        ToolExecutionRequestModel request)
    {
        try
        {
            var tool =
                _toolRegistry.GetTool(
                    request.ToolName);

            var result =
                await tool.ExecuteAsync(
                    request.Parameters);

            return Ok(new
            {
                Tool = tool.Name,

                Result = result
            });
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