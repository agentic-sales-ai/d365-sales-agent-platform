using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class PlannerAiService
    : IPlannerAiService
{
    private readonly OpenAIOptions _options;

    private readonly HttpClient _httpClient;

    private readonly IAgentToolRegistry
        _toolRegistry;

    public PlannerAiService(
        IOptions<OpenAIOptions> options,
        IAgentToolRegistry toolRegistry)
    {
        _options = options.Value;

        _toolRegistry = toolRegistry;

        _httpClient = new HttpClient();

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _options.ApiKey);
    }

    public async Task<PlannerPlanModel>
        GeneratePlanAsync(
            string userPrompt)
    {
        var tools =
            _toolRegistry
                .GetTools()
                .Select(t =>
$"""
Tool: {t.Name}

Description:
{t.Description}

DefaultParameters:
{JsonSerializer.Serialize(
    t.GetDefaultParameters())}
""");

        var toolsText =
            string.Join(
                "\n",
                tools);

        var prompt =
$"""
You are an enterprise AI workflow planner.

Available tools:

{toolsText}

User request:

{userPrompt}

Generate a workflow execution plan.

Return ONLY valid JSON.

Required structure:

steps = array

Each step contains:

- stepNumber
- toolName
- parameters
- dependsOnMemoryKeys

Rules:

Only use available tools.

Do not invent tools.

Use tool descriptions.

If a tool consumes output
from a previous tool:

NEVER place memory data
inside parameters.

Instead use:

dependsOnMemoryKeys

Example:

GetOpportunity

produces:

Opportunity

GenerateOpportunityInsights
must contain:

dependsOnMemoryKeys:

[
   "Opportunity"
]

Parameters stay empty
for memory-driven tools.

Never duplicate memory values
inside parameters.

Do not include explanations.

Return JSON only.
""";

        var requestBody =
            new
            {
                model =
                    _options.Model,

                messages =
                    new[]
                    {
                        new
                        {
                            role =
                                "user",

                            content =
                                prompt
                        }
                    }
            };

        var response =
            await _httpClient
                .PostAsJsonAsync(
                    "https://api.openai.com/v1/chat/completions",
                    requestBody);

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response
                    .Content
                    .ReadAsStringAsync();

            throw new Exception(
                $"OpenAI API Error: {error}");
        }

        var json =
            await response
                .Content
                .ReadAsStringAsync();

        using var document =
            JsonDocument.Parse(
                json);

        var content =
            document.RootElement
                .GetProperty(
                    "choices")[0]
                .GetProperty(
                    "message")
                .GetProperty(
                    "content")
                .GetString();

        if (
            string.IsNullOrWhiteSpace(
                content))
        {
            return new PlannerPlanModel();
        }

        content =
            content
                .Replace(
                    "```json",
                    "")
                .Replace(
                    "```",
                    "")
                .Trim();

        Console.WriteLine(
@"===== GENERATED PLAN =====");

        Console.WriteLine(
            content);

        Console.WriteLine(
@"==========================");

        var result =
            JsonSerializer
                .Deserialize<
                    PlannerPlanModel>(
                    content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive =
                            true
                    });

        return result
            ?? new PlannerPlanModel();
    }
}