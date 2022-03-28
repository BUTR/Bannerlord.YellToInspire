using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    internal sealed class InspireKillingAIAgentComponent : AgentComponent
    {
        private SettingsProviderMissionBehavior? SettingsProvider => Agent.Mission.GetMissionBehavior<SettingsProviderMissionBehavior>();
        private Settings? Settings => SettingsProvider is { } settingsProvider ? settingsProvider.Get<Settings>() : null;
        private InspireKillingStateAgentComponent? State => Agent.GetComponent<InspireKillingStateAgentComponent>();

        public InspireKillingAIAgentComponent(Agent agent) : base(agent) { }

        public override void OnTickAsAI(float dt)
        {
            base.OnTickAsAI(dt);

            if (MBCommon.IsPaused) return;

            if (Settings is not { } settings) return;

            if (Agent.Mission.GetNearbyAgents(Agent.Position.AsVec2, settings.AbilityRadius(Agent.Character)).Count() < 3) return;

            if (State is not { } state) return;

            if (!state.CanInspire()) return;

            if (Agent.Character is CharacterObject { HeroObject: { } hero })
            {
                if (hero.GetPerkValue(Perks.InspireBasic))
                {
                    state.ResetInspiration();
                    Utils.InspireAura(Agent);
                }
            }
            else
            {
                state.ResetInspiration();
                Utils.InspireAura(Agent);
            }
        }
    }
}