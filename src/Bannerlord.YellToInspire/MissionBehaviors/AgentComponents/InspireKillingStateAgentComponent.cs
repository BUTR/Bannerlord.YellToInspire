using Bannerlord.YellToInspire.Data;

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    /// <summary>
    /// Manages the Inspiration state for the <see cref="Agent"/> when <see cref="GameplayType.Killing"/> is used.
    /// </summary>
    internal sealed class InspireKillingStateAgentComponent : AgentComponent
    {
        public float InspireMeter { get; private set; } = 0f;

        private SettingsProviderMissionBehavior? SettingsProvider => Agent.Mission.GetMissionBehavior<SettingsProviderMissionBehavior>();
        private Settings? Settings => SettingsProvider is { } settingsProvider ? settingsProvider.Get<Settings>() : null;

        public InspireKillingStateAgentComponent(Agent agent) : base(agent) { }

        public bool CanInspire() => InspireMeter >= 100f;

        public void ResetInspiration()
        {
            InspireMeter = 0f;
        }

        public void OnRemovedAgent(Agent affectedAgent, AgentState affectedAgentState, KillingBlow blow)
        {
            var agentFlags = affectedAgent.GetAgentFlags();
            if (!agentFlags.HasFlag(AgentFlag.IsHumanoid)) return;

            if (Settings is not { } settings) return;
            var multiplier = settings.AbilityKillMultiplier(Agent.Character);

            InspireMeter += affectedAgentState switch
            {
                AgentState.Unconscious => 2f * multiplier,
                AgentState.Killed => 4f * multiplier,
                _ => 0
            };
        }
    }
}