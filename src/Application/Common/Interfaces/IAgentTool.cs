namespace Application.Common.Interfaces;

public interface IAgentTool
{
    string Name { get; }

    string Description { get; }

    Dictionary<string, object>
        GetDefaultParameters();

    Task<object> ExecuteAsync(
        Dictionary<string, object> parameters);
}