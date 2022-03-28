using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    public abstract class InspireBaseWithStateAgentComponent<TState> : InspireBaseAgentComponent where TState : AgentComponent
    {
        protected TState? State => Agent.GetComponent<TState>();

        protected InspireBaseWithStateAgentComponent(Agent agent) : base(agent) { }
    }
}