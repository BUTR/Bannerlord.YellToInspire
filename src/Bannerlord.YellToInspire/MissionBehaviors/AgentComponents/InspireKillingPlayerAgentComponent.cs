﻿using Bannerlord.YellToInspire.HotKeys;

using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    public sealed class InspireKilllingPlayerAgentComponent : InspireBaseWithStateAgentComponent<InspireKillingStateAgentComponent>, IAgentComponentOnTick
    {
        private static readonly TextObject ReadyText = new("{=Zt8Qbxh3HP}Your Inspire ability is ready!");
        private static readonly TextObject NotReadyText = new("{=MFZQek5Upy}You are not ready to use Inspire yet!");


        private bool _messageWasShown = false;

        public InspireKilllingPlayerAgentComponent(Agent agent) : base(agent) { }

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

                InformationManager.DisplayMessage(new(Utils.AbilityPhrases[MBRandom.RandomInt(0, Utils.AbilityPhrases.Length)].ToString()));
                if (settings.ShowDetailedMessage)
                    Utils.ShowDetailedMessage(troopsStatistics, settings.ShowDetailedMessageText);
            }
        }
    }
}