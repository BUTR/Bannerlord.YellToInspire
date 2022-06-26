using Bannerlord.YellToInspire.HotKeys;

#if e172
using TaleWorlds.Core;
#elif e180
using TaleWorlds.Library;
#endif
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    public sealed class InspireCooldownPlayerAgentComponent : InspireBaseWithStateAgentComponent<InspireCooldownStateAgentComponent>, IAgentComponentOnTick
    {
        private static readonly TextObject CooldownText = new("{=FsbQpeMJYJ}Your ability is still on cooldown for {TIME} second(s)!");


        public InspireCooldownPlayerAgentComponent(Agent agent) : base(agent) { }

        public void OnTick(float dt)
        {
            if (MBCommon.IsPaused) return;

            if (Agent.Mission.InputManager.IsHotKeyDownAndReleased(YellToInspireHotkeyCategory.YellToInspireKeyId))
            {
                if (Settings is not { } settings) return;

                if (State is not { } state) return;

                if (!state.CanInspire(out var cooldown))
                {
                    InformationManager.DisplayMessage(new(CooldownText.SetTextVariable("TIME", $"{cooldown:###0}").ToString()));
                    return;
                }

                var troopsStatistics = state.Inspire();

                InformationManager.DisplayMessage(new(Utils.GetRandomAbilityPhrase().ToString()));
                if (settings.ShowDetailedMessage)
                    Utils.ShowDetailedMessage(troopsStatistics, settings.ShowDetailedMessageText);
            }
        }
    }
}