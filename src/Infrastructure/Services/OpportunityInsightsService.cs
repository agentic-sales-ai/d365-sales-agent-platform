using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class OpportunityInsightsService
    : IOpportunityInsightsService
{
    private readonly IDataverseService _dataverseService;

    private readonly OpenAIOptions _options;

    private readonly HttpClient _httpClient;

    public OpportunityInsightsService(
        IDataverseService dataverseService,
        IOptions<OpenAIOptions> options)
    {
        _dataverseService = dataverseService;

        _options = options.Value;

        _httpClient = new HttpClient();

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _options.ApiKey);
    }

    public async Task<OpportunityInsightsModel>
        GenerateInsightsAsync(Guid opportunityId)
    {
        var opportunity =
            await _dataverseService.GetOpportunityAsync(
                opportunityId);

        var activities =
            await _dataverseService
                .GetOpportunityActivitiesAsync(
                    opportunityId);

        var activitiesText = string.Join(
            "\n",
            activities.Select(a =>
                $"- {a.ActivityType}: {a.Subject}"));

        var prompt =
$"""
You are an enterprise sales AI assistant.

Analyze the following Dynamics 365 opportunity.

Opportunity Name:
{opportunity?.Name}

Customer:
{opportunity?.CustomerName}

Estimated Value:
{opportunity?.EstimatedValue}

Status:
{opportunity?.Status}

Activities:
{activitiesText}

Provide:
1. Executive summary
2. Key risks
3. Recommended next actions

Keep response concise and business-focused.
""";

        var requestBody = new
        {
            model = _options.Model,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = prompt
                }
            }
        };

        var response =
            await _httpClient.PostAsJsonAsync(
                "https://api.openai.com/v1/chat/completions",
                requestBody);

        response.EnsureSuccessStatusCode();

        var json =
            await response.Content.ReadAsStringAsync();

        using var document =
            JsonDocument.Parse(json);

        var content =
            document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

        return new OpportunityInsightsModel
        {
            Summary = content ?? string.Empty
        };
    }
}