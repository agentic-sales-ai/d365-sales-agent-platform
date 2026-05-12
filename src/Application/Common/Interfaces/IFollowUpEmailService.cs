using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IFollowUpEmailService
{
    Task<FollowUpEmailModel> GenerateAsync(
        Guid opportunityId);
}