using Microsoft.Xrm.Sdk;
using Application.Common.Models;
using System.Linq;
using Application.Common.Interfaces;
using Application.Common.Options;
using Microsoft.Extensions.Options;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Query;

namespace Infrastructure.Services;

public class DataverseService : IDataverseService
{
    private readonly ServiceClient _serviceClient;

    public DataverseService(IOptions<DataverseOptions> options)
    {
        var settings = options.Value;

        var connectionString =
            $@"
AuthType=ClientSecret;
Url={settings.Url};
ClientId={settings.ClientId};
ClientSecret={settings.ClientSecret};
TenantId={settings.TenantId};
";

        _serviceClient = new ServiceClient(connectionString);
    }

    public async Task<OpportunityModel?> GetOpportunityAsync(
        Guid opportunityId)
    {
        var entity = await _serviceClient.RetrieveAsync(
            "opportunity",
            opportunityId,
            new ColumnSet(true));

        if (entity == null)
        {
            return null;
        }

        return new OpportunityModel
{
    Id = entity.Id,
    Name = entity.GetAttributeValue<string>("name") ?? string.Empty,

    EstimatedValue =
        entity.GetAttributeValue<Money>("estimatedvalue")?.Value ?? 0,

    CustomerName =
        entity.GetAttributeValue<EntityReference>("customerid")?.Name
        ?? string.Empty,

    Status =
        entity.FormattedValues.Contains("statecode")
            ? entity.FormattedValues["statecode"]
            : string.Empty
};
    }

    public async Task<List<ActivityModel>> GetOpportunityActivitiesAsync(
        Guid opportunityId)
    {
        var query = new QueryExpression("activitypointer")
        {
            ColumnSet = new ColumnSet(true)
        };

        query.Criteria.AddCondition(
            "regardingobjectid",
            ConditionOperator.Equal,
            opportunityId);

        var result = await _serviceClient.RetrieveMultipleAsync(query);

        return result.Entities.Select(e => new ActivityModel
        {
            Id = e.Id,
            Subject = e.GetAttributeValue<string>("subject") ?? string.Empty,
            ActivityType = e.GetAttributeValue<string>("activitytypecode") ?? string.Empty
        }).ToList();
    }
}