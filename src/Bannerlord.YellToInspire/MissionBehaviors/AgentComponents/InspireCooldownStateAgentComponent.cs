using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    internal sealed class InspireCooldownStateAgentComponent : AgentComponent
    {
        private double _cooldownSnapshot;
        public double PastCooldown => MissionTime.Now.ToSeconds - _cooldownSnapshot;

        private SettingsProviderMissionBehavior? SettingsProvider => Agent.Mission.GetMissionBehavior<SettingsProviderMissionBehavior>();
        private Settings? Settings => SettingsProvider is { } settingsProvider ? settingsProvider.Get<Settings>() : null;

        public InspireCooldownStateAgentComponent(Agent agent) : base(agent) { }

        public bool CanInspire()
        {
            if (Settings is not { } settings) return false;
            return _cooldownSnapshot == 0 || PastCooldown > settings.AbilityCooldown(Agent.Character);
        }

        public void ResetInspiration()
        {
            _cooldownSnapshot = MissionTime.Now.ToSeconds;
        }
    }
}