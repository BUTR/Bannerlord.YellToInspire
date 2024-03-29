﻿using Bannerlord.YellToInspire.Data;
using Bannerlord.YellToInspire.Utils;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    /// <summary>
    /// Manages the Inspiration state for the <see cref="Agent"/> when <see cref="Bannerlord.YellToInspire.Data.GameplaySystem.Killing"/> is used.
    /// </summary>
    public class InspireKillingStateAgentComponent : InspireBaseAgentComponent
    {
        public float InspireMeter { get; private set; } = 0f;

        public InspireKillingStateAgentComponent(Agent agent) : base(agent) { }

        public virtual bool CanInspire() => InspireMeter >= 100f &&
                                            (Agent.Character is not CharacterObject { HeroObject: { } hero } || hero.GetPerkValue(Perks.Instance.InspireBasic));

        public virtual TroopStatistics Inspire()
        {
            InspireMeter = 0f;
            return CommonUtils.InspireAura(Agent);
        }

        public virtual void OnAffectedAgentRemoved(Agent affectedAgent, AgentState affectedAgentState, KillingBlow blow)
        {
            var agentFlags = affectedAgent.GetAgentFlags();
            if (!agentFlags.HasFlag(AgentFlag.IsHumanoid)) return;

            if (Settings is not { } settings) return;
            var multiplier = settings.AbilityKillMultiplier(Agent.Character);

            InspireMeter += affectedAgentState switch
            {
                AgentState.Unconscious => 2f * multiplier,
                AgentState.Killed => 4f * multiplier,
                _ => 0
            };
        }
    }
}