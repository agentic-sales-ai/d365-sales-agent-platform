namespace Application.Common.Interfaces;

public interface IAgentToolRegistry
{
    List<IAgentTool> GetTools();

    IAgentTool GetTool(string name);
}