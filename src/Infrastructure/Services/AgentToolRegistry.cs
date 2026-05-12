using Application.Common.Interfaces;

namespace Infrastructure.Services;

public class AgentToolRegistry
    : IAgentToolRegistry
{
    private readonly IEnumerable<IAgentTool>
        _tools;

    public AgentToolRegistry(
        IEnumerable<IAgentTool> tools)
    {
        _tools = tools;
    }

    public List<IAgentTool> GetTools()
    {
        return _tools.ToList();
    }

    public IAgentTool GetTool(string name)
    {
        return _tools.First(
            t => t.Name.Equals(
                name,
                StringComparison.OrdinalIgnoreCase));
    }
}