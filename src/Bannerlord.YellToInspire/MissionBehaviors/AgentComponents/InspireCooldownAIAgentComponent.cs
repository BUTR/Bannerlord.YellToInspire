using System.Linq;

using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    internal sealed class InspireCooldownAIAgentComponent : InspireBaseWithStateAgentComponent<InspireCooldownStateAgentComponent>
    {
        public InspireCooldownAIAgentComponent(Agent agent) : base(agent) { }

        public override void OnTickAsAI(float dt)
        {
            base.OnTickAsAI(dt);

            if (MBCommon.IsPaused) return;

            if (Settings is not { } settings) return;
            if (State is not { } state) return;

            if (Agent.Mission.GetNearbyAgents(Agent.Position.AsVec2, settings.AbilityRadius(Agent.Character)).Count() < 3) return;

            if (!state.CanInspire(out _)) return;

            state.Inspire();
        }
    }
}