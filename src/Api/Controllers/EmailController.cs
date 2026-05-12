using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IEmailDeliveryService
        _emailDeliveryService;

    public EmailController(
        IEmailDeliveryService emailDeliveryService)
    {
        _emailDeliveryService =
            emailDeliveryService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send(
        SendEmailRequestModel request)
    {
        try
        {
            await _emailDeliveryService
                .SendAsync(request);

            return Ok(new
            {
                Status = "Email sent successfully"
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