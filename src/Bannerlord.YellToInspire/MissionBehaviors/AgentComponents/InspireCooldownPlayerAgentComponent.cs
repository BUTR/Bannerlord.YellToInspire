using Bannerlord.ButterLib.Extensions;
using Bannerlord.YellToInspire.HotKeys;
using Bannerlord.YellToInspire.Utils;

using System;

using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    public sealed class InspireCooldownPlayerAgentComponent : InspireBaseWithStateAgentComponent<InspireCooldownStateAgentComponent>, IDisposable
    {
        private readonly IDisposable? _subscription;

        public InspireCooldownPlayerAgentComponent(Agent agent) : base(agent)
        {
            _subscription = Agent.Mission.InputManager.SubscribeToIsDownAndReleasedEvent<YellToInspireHotKey>(() =>
            {
                if (MBCommon.IsPaused) return;

                if (Settings is not { } settings) return;

                if (State is not { } state) return;

                if (!state.CanInspire(out var cooldown))
                {
                    InformationManager.DisplayMessage(new(Strings.MessageCooldown.SetTextVariable("TIME", $"{cooldown:###0}").ToString()));
                    return;
                }

                var troopsStatistics = state.Inspire();

                InformationManager.DisplayMessage(new(CommonUtils.GetRandomAbilityPhrase().ToString()));
                if (settings.ShowDetailedMessage)
                    CommonUtils.ShowDetailedMessage(troopsStatistics, settings.ShowDetailedMessageText);
            });
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