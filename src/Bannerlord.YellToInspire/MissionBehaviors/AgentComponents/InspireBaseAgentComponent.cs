using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    public abstract class InspireBaseAgentComponent : AgentComponent
    {
        protected SettingsProviderMissionBehavior? SettingsProvider => Agent.Mission.GetMissionBehavior<SettingsProviderMissionBehavior>();
        protected Settings? Settings => SettingsProvider is { } settingsProvider ? settingsProvider.Get<Settings>() : null;

        protected InspireBaseAgentComponent(Agent agent) : base(agent) { }
    }
}