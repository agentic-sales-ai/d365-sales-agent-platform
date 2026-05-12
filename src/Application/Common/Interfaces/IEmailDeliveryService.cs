using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IEmailDeliveryService
{
    Task SendAsync(
        SendEmailRequestModel request);
}