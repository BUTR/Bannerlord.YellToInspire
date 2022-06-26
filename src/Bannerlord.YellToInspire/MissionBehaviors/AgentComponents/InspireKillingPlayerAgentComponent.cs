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
    public sealed class InspireKillingPlayerAgentComponent : InspireBaseWithStateAgentComponent<InspireKillingStateAgentComponent>, IAgentComponentOnTick
    {
        private static readonly TextObject ReadyText = new("{=Zt8Qbxh3HP}Your Inspire ability is ready!");
        private static readonly TextObject NotReadyText = new("{=MFZQek5Upy}You are not ready to use Inspire yet!");


        private bool _messageWasShown = false;

        public InspireKillingPlayerAgentComponent(Agent agent) : base(agent) { }

        public void OnTick(float dt)
        {
            if (MBCommon.IsPaused) return;

            if (Settings is not { } settings) return;
            if (State is not { } state) return;

            if (!_messageWasShown)
            {
                if (state.CanInspire())
                {
                    InformationManager.DisplayMessage(new(ReadyText.ToString()));
                    _messageWasShown = true;
                }
            }

            if (Agent.Mission.InputManager.IsHotKeyDownAndReleased(YellToInspireHotkeyCategory.YellToInspireKeyId))
            {
                if (!state.CanInspire())
                {
                    InformationManager.DisplayMessage(new(NotReadyText.ToString()));
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