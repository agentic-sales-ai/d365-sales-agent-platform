using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class OpportunityInsightsService
    : IOpportunityInsightsService
{
    private readonly OpenAIOptions _options;

    private readonly HttpClient _httpClient;

    private readonly IOpportunityContextService
        _contextService;

    public OpportunityInsightsService(
        IOpportunityContextService contextService,
        IOptions<OpenAIOptions> options)
    {
        _contextService = contextService;

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
        var context =
            await _contextService
                .BuildContextAsync(opportunityId);

        var opportunity = context.Opportunity;

        var activitiesText = string.Join(
            "\n",
            context.Activities.Select(a =>
                $"- {a.ActivityType}: {a.Subject}"));

        var prompt =
$"""
You are an enterprise sales AI assistant.

Analyze the following Dynamics 365 opportunity.

Opportunity Name:
{opportunity.Name}

Customer:
{opportunity.CustomerName}

Estimated Value:
{opportunity.EstimatedValue}

Status:
{opportunity.Status}

Activities:
{activitiesText}

Return ONLY valid JSON.

Required fields:
- summary
- risks
- nextActions

Example format:
summary = string
risks = array of strings
nextActions = array of strings

Do not include markdown.
Do not include explanations outside JSON.
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

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content.ReadAsStringAsync();

            throw new Exception(
                $"OpenAI API Error: {error}");
        }

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

        if (string.IsNullOrWhiteSpace(content))
        {
            return new OpportunityInsightsModel();
        }

        var insights =
            JsonSerializer.Deserialize<OpportunityInsightsModel>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        return insights ?? new OpportunityInsightsModel();
    }
}