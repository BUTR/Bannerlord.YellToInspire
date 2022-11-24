using Bannerlord.ButterLib.Extensions;
using Bannerlord.YellToInspire.HotKeys;
using Bannerlord.YellToInspire.Utils;

using System;

using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    public sealed class InspireKillingPlayerAgentComponent : InspireBaseWithStateAgentComponent<InspireKillingStateAgentComponent>, IAgentComponentOnTick, IDisposable
    {
        private readonly IDisposable? _subscription;
        private bool _messageWasShown = false;

        public InspireKillingPlayerAgentComponent(Agent agent) : base(agent)
        {
            _subscription = Agent.Mission.InputManager.SubscribeToIsDownAndReleasedEvent<YellToInspireHotKey>(() =>
            {
                if (MBCommon.IsPaused) return;

                if (Settings is not { } settings) return;
                if (State is not { } state) return;

                if (!state.CanInspire())
                {
                    InformationManager.DisplayMessage(new(Strings.AbilityNotReady.ToString()));
                    return;
                }

                var troopsStatistics = state.Inspire();

                InformationManager.DisplayMessage(new(CommonUtils.GetRandomAbilityPhrase().ToString()));
                if (settings.ShowDetailedMessage)
                    CommonUtils.ShowDetailedMessage(troopsStatistics, settings.ShowDetailedMessageText);
            });
        }

        public void OnTick(float dt)
        {
            if (MBCommon.IsPaused) return;

            if (Settings is not { } settings) return;
            if (State is not { } state) return;

            if (!_messageWasShown)
            {
                if (state.CanInspire())
                {
                    InformationManager.DisplayMessage(new(Strings.AbilityReady.ToString()));
                    _messageWasShown = true;
                }
            }
        }

        public override void OnAgentRemoved()
        {
            base.OnAgentRemoved();
            _subscription?.Dispose();
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}