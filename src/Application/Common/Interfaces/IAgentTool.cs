namespace Application.Common.Interfaces;

public interface IAgentTool
{
    string Name { get; }

    string Description { get; }

    Task<object> ExecuteAsync(
        Dictionary<string, object> parameters);
}