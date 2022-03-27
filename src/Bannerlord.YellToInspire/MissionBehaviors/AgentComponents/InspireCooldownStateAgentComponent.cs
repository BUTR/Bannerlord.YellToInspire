using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    internal sealed class InspireCooldownStateAgentComponent : AgentComponent
    {
        private double _cooldownSnapshot;
        public double PastCooldown => MissionTime.Now.ToSeconds - _cooldownSnapshot;

        public InspireCooldownStateAgentComponent(Agent agent) : base(agent) { }

        public bool CanInspire()
        {
            if (Settings.Instance is not { } settings) return false;
            return _cooldownSnapshot == 0 || PastCooldown > settings.AbilityCooldown(Agent.Character);
        }

        public void ResetInspiration()
        {
            _cooldownSnapshot = MissionTime.Now.ToSeconds;
        }
    }
}