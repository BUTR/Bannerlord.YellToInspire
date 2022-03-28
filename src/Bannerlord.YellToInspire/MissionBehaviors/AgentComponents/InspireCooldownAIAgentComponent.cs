using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    internal sealed class InspireCooldownAIAgentComponent : AgentComponent
    {
        private SettingsProviderMissionBehavior? SettingsProvider => Agent.Mission.GetMissionBehavior<SettingsProviderMissionBehavior>();
        private Settings? Settings => SettingsProvider is { } settingsProvider ? settingsProvider.Get<Settings>() : null;
        private InspireCooldownStateAgentComponent? State => Agent.GetComponent<InspireCooldownStateAgentComponent>();

        public InspireCooldownAIAgentComponent(Agent agent) : base(agent) { }

        public override void OnTickAsAI(float dt)
        {
            base.OnTickAsAI(dt);

            if (MBCommon.IsPaused) return;

            if (Settings is not { } settings) return;
            if (State is not { } state) return;

            if (Agent.Mission.GetNearbyAgents(Agent.Position.AsVec2, settings.AbilityRadius(Agent.Character)).Count() < 3) return;

            if (!state.CanInspire()) return;

            // TODO: Is the AI assigning itself perks?
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