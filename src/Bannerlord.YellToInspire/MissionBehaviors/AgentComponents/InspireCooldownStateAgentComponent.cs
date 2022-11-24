using Bannerlord.YellToInspire.Data;
using Bannerlord.YellToInspire.Utils;

using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    /// <summary>
    /// Manages the Inspiration state for the <see cref="Agent"/> when <see cref="Bannerlord.YellToInspire.Data.GameplaySystem.Cooldown"/> is used.
    /// </summary>
    public class InspireCooldownStateAgentComponent : InspireBaseAgentComponent
    {
        protected double _cooldownSnapshot;
        public virtual double PastCooldown => MissionTime.Now.ToSeconds - _cooldownSnapshot;

        public InspireCooldownStateAgentComponent(Agent agent) : base(agent) { }

        public virtual bool CanInspire(out double cooldown)
        {
            if (Settings is not { } settings)
            {
                cooldown = 0f;
                return false;
            }

            var abilityCooldown = settings.AbilityCooldown(Agent.Character);
            cooldown = abilityCooldown - PastCooldown;

            return Agent.Character is CharacterObject { HeroObject: { } hero }
                ? hero.GetPerkValue(Perks.Instance.InspireBasic) && CooldownCheck()
                : CooldownCheck();

            bool CooldownCheck() => _cooldownSnapshot == 0 || PastCooldown > abilityCooldown;
        }

        public virtual TroopStatistics Inspire()
        {
            _cooldownSnapshot = MissionTime.Now.ToSeconds;
            return CommonUtils.InspireAura(Agent);
        }
    }
}