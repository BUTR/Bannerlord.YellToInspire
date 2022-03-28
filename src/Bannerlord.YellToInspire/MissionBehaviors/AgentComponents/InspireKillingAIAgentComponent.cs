using System.Linq;

using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    public sealed class InspireKillingAIAgentComponent : InspireBaseWithStateAgentComponent<InspireKillingStateAgentComponent>
    {
        public InspireKillingAIAgentComponent(Agent agent) : base(agent) { }

        public override void OnTickAsAI(float dt)
        {
            base.OnTickAsAI(dt);

            if (MBCommon.IsPaused) return;

            if (Settings is not { } settings) return;

            if (Agent.Mission.GetNearbyAgents(Agent.Position.AsVec2, settings.AbilityRadius(Agent.Character)).Count() < 3) return;

            if (State is not { } state) return;

            if (!state.CanInspire()) return;

            state.Inspire();
        }
    }
}