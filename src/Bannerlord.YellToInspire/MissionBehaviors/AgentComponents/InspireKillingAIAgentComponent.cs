using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    internal sealed class InspireKillingAIAgentComponent : AgentComponent
    {
        public InspireKillingAIAgentComponent(Agent agent) : base(agent) { }

        public override void OnTickAsAI(float dt)
        {
            base.OnTickAsAI(dt);

            if (MBCommon.IsPaused) return;

            if (Settings.Instance is not { } settings) return;

            if (Agent.Mission.GetNearbyAgents(Agent.Position.AsVec2, settings.AbilityRadius(Agent.Character)).Count() < 3) return;

            if (Agent.GetComponent<InspireKillingStateAgentComponent>() is not { } inspireStateAgentComponent) return;

            if (!inspireStateAgentComponent.CanInspire()) return;

            if (Agent.Character is CharacterObject { HeroObject: { } hero })
            {
                if (hero.GetPerkValue(Perks.InspireBasic))
                {
                    inspireStateAgentComponent.ResetInspiration();
                    Utils.InspireAura(Agent);
                }
            }
            else
            {
                inspireStateAgentComponent.ResetInspiration();
                Utils.InspireAura(Agent);
            }
        }
    }
}