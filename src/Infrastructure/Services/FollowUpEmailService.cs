using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class FollowUpEmailService
    : IFollowUpEmailService
{
    private readonly IDataverseService _dataverseService;

    private readonly OpenAIOptions _options;

    private readonly HttpClient _httpClient;

    public FollowUpEmailService(
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

    public async Task<FollowUpEmailModel>
        GenerateAsync(Guid opportunityId)
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
You are an enterprise sales assistant.

Generate a professional customer follow-up email.

Opportunity:
{opportunity?.Name}

Customer:
{opportunity?.CustomerName}

Value:
{opportunity?.EstimatedValue}

Status:
{opportunity?.Status}

Recent Activities:
{activitiesText}

Return ONLY valid JSON.

Required fields:
- subject
- body

The tone should be:
- professional
- concise
- relationship-oriented
- sales-focused

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
            return new FollowUpEmailModel();
        }

        var result =
            JsonSerializer.Deserialize<FollowUpEmailModel>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        return result ?? new FollowUpEmailModel();
    }
}