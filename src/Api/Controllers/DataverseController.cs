using Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataverseController : ControllerBase
{
    private readonly IDataverseService _dataverseService;

    private readonly IOpportunityInsightsService
        _opportunityInsightsService;

    public DataverseController(
        IDataverseService dataverseService,
        IOpportunityInsightsService opportunityInsightsService)
    {
        _dataverseService = dataverseService;

        _opportunityInsightsService =
            opportunityInsightsService;
    }

    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        try
        {
            await _dataverseService
                .GetOpportunityActivitiesAsync(Guid.Empty);

            return Ok(new
            {
                Status = "Connected to Dataverse"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Status = "Connection failed",
                Error = ex.Message
            });
        }
    }

    [HttpGet("opportunity/{id}")]
    public async Task<IActionResult> GetOpportunity(Guid id)
    {
        try
        {
            var result =
                await _dataverseService.GetOpportunityAsync(id);

            if (result == null)
            {
                return NotFound();
            }

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

    [HttpGet("opportunity/{id}/insights")]
    public async Task<IActionResult> GetInsights(Guid id)
    {
        try
        {
            var result =
                await _opportunityInsightsService
                    .GenerateInsightsAsync(id);

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