using Application.Common.Options;
using Microsoft.Extensions.Options;
using Application.Common.Interfaces;
using Application.Common.Models;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace Infrastructure.Services;

public class GraphEmailDeliveryService
    : IEmailDeliveryService
{
    private readonly GraphServiceClient _graphClient;

    public GraphEmailDeliveryService(
    IOptions<DataverseOptions> options)
{
    var settings = options.Value;

    var credential =
    new DeviceCodeCredential(
        new DeviceCodeCredentialOptions
        {
            TenantId = settings.TenantId,

            ClientId = settings.ClientId,

            DeviceCodeCallback =
                (callback, cancellationToken) =>
                {
                    Console.WriteLine(callback.Message);

                    return Task.CompletedTask;
                }
        });

    _graphClient =
        new GraphServiceClient(
            credential,
            new[]
            {
                "Mail.Send",
                "User.Read"
            });
}

    public async Task SendAsync(
        SendEmailRequestModel request)
    {
        var message = new Message
        {
            Subject = request.Subject,

            Body = new ItemBody
            {
                ContentType = BodyType.Text,
                Content = request.Body
            },

            ToRecipients =
            [
                new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = request.To
                    }
                }
            ]
        };

        await _graphClient
            .Me
            .SendMail
            .PostAsync(
                new Microsoft.Graph.Me.SendMail.SendMailPostRequestBody
                {
                    Message = message
                });
    }
}