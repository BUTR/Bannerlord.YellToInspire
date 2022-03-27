using Bannerlord.YellToInspire.HotKeys;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    internal sealed class InspireCooldownPlayerAgentComponent : AgentComponent, IAgentComponentOnTick
    {
        private static readonly TextObject CooldownText = new("{=FsbQpeMJYJ}Your ability is still on cooldown for {TIME} second(s)!");

        public InspireCooldownPlayerAgentComponent(Agent agent) : base(agent) { }

        public void OnTick(float dt)
        {
            if (MBCommon.IsPaused) return;

            if (Agent.Mission.InputManager.IsHotKeyDownAndReleased(YellToInspireHotkeyCategory.YellToInspireKeyId))
            {
                if (Settings.Instance is not { } settings) return;

                if (Agent.GetComponent<InspireCooldownStateAgentComponent>() is not { } state) return;

                if (!state.CanInspire())
                {
                    var time = settings.AbilityCooldown(Agent.Character) - state.PastCooldown;
                    InformationManager.DisplayMessage(new(CooldownText.SetTextVariable("TIME", $"{time:###0}").ToString()));
                    return;
                }

                if (Agent.Character is CharacterObject { HeroObject: { } hero })
                {
                    if (hero.GetPerkValue(Perks.InspireBasic))
                    {
                        state.ResetInspiration();
                        var troopsStatistics = Utils.InspireAura(Agent);

                        InformationManager.DisplayMessage(new(Utils.AbilityPhrases[MBRandom.RandomInt(0, Utils.AbilityPhrases.Length)].ToString()));
                        if (settings.ShowDetailedMessage)
                            Utils.ShowDetailedMessage(troopsStatistics);
                    }
                }
                else
                {
                    state.ResetInspiration();
                    var troopsStatistics = Utils.InspireAura(Agent);

                    InformationManager.DisplayMessage(new(Utils.AbilityPhrases[MBRandom.RandomInt(0, Utils.AbilityPhrases.Length)].ToString()));
                    if (settings.ShowDetailedMessage)
                        Utils.ShowDetailedMessage(troopsStatistics);
                }
            }
        }
    }
}