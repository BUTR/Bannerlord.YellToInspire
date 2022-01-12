using System;
using System.Collections.Generic;
using System.Text;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace YellToInspire
{
    internal static class Utils
    {
        public struct TroopStatistics
        {
            public int Inspired;
            public int Retreating;
            public int Nearby;
            public int Fled;
        }

        private static readonly TextObject AlliesInspiredText = new("{=gqr3o3aOfu}Allied unit(s) that were inspired: {INSPIRED}");
        private static readonly TextObject AlliesRestoredText = new("{=5fapWaqKVw}Allied unit(s) that are returning to battle: {RETURNING}");
        private static readonly TextObject EnemiesScaredText = new("{=Ldui1l6RMK}Enemy unit(s) that had their courage tested: {SCARED}");
        private static readonly TextObject EnemiesFledText = new("{=8aQ7tO6TbA}Enemy unit(s) that are fleeing: {FLED}");
        private static readonly TextObject EnemiesRestoredText = new("{=zy6xciLnky}Enemy unit(s) that are returning to battle: {RETURNING}");

        public static (TroopStatistics Statistics, List<WeakReference<Agent>> AffectedAgents) AffectTroops(Agent causingAgent)
        {
            if (Settings.Instance is not { } settings) return default;

            var vec2Pos = causingAgent.Position.AsVec2;
            var team = causingAgent.Team;

            var statistics = new TroopStatistics();
            var affectedAgents = new List<WeakReference<Agent>>();

            foreach (var nearbyAllyAgent in Mission.Current.GetNearbyAllyAgents(vec2Pos, settings.AbilityRadius(causingAgent.Character), team))
            {
                if (nearbyAllyAgent == causingAgent) continue;

                if (nearbyAllyAgent.GetComponent<CommonAIComponent>() is not { } commonAiComponent) continue;

                statistics.Inspired++;
                affectedAgents.Add(new(nearbyAllyAgent));

                nearbyAllyAgent.ChangeMorale(settings.AlliedMoraleGain(causingAgent.Character));

                if (commonAiComponent.IsRetreating)
                {
                    if (causingAgent.Character is not CharacterObject charObj || charObj.GetPerkValue(Perks.InspireResolve))
                    {
#if e162 || e163 || e164 || e165
                        nearbyAllyAgent.SetMorale(25f);
#elif e170
                        nearbyAllyAgent.SetMorale(commonAiComponent.RecoveryMorale);
#else
#error NOT SET
#endif
                        commonAiComponent.StopRetreating();
                        statistics.Retreating++;
                    }
                }
                if (commonAiComponent.IsPanicked)
                {
                    if (causingAgent.Character is not CharacterObject charObj || charObj.GetPerkValue(Perks.InspireResolve))
                    {
#if e162 || e163 || e164 || e165
                        nearbyAllyAgent.SetMorale(25f);
#elif e170
                        nearbyAllyAgent.SetMorale(commonAiComponent.RecoveryMorale);
#else
#error NOT SET
#endif
                        commonAiComponent.StopRetreating();
                        statistics.Retreating++;
                    }
                }
            }

            foreach (var nearbyEnemyAgent in Mission.Current.GetNearbyEnemyAgents(vec2Pos, settings.AbilityRadius(causingAgent.Character), team))
            {
                if (nearbyEnemyAgent.GetComponent<CommonAIComponent>() is not { } commonAiComponent) continue;

                statistics.Nearby++;
                affectedAgents.Add(new(nearbyEnemyAgent));

                if (MBRandom.RandomFloatRanged(0f, 1f) > settings.EnemyChanceToFlee(causingAgent.Character)) continue;

                if (nearbyEnemyAgent.GetMorale() <= settings.EnemyFleeMoraleThreshold)
                {
#if e162 || e163 || e164 || e165
                    if (true)
#elif e170
                     if (commonAiComponent.CanPanic())
#else
#error NOT SET
#endif
                    {
                        statistics.Fled++;

                        nearbyEnemyAgent.GetComponent<CommonAIComponent>().Panic();

                        //if (settings.FledEnemiesReturn)
                        //{
                        //    commonAiComponent.StopRetreating();
                        //}
                    }
                }
            }

            return (statistics, affectedAgents);
        }

        public static void ShowDetailedMessage(TroopStatistics troopsStatistics)
        {
            if (Settings.Instance is not { } settings) return;

            if (settings.ShowDetailedMessageText)
            {
                InformationManager.DisplayMessage(new(AlliesInspiredText.SetTextVariable("INSPIRED", troopsStatistics.Inspired).ToString()));
                InformationManager.DisplayMessage(new(AlliesRestoredText.SetTextVariable("RETURNING", troopsStatistics.Retreating).ToString()));
                InformationManager.DisplayMessage(new(EnemiesScaredText.SetTextVariable("SCARED", troopsStatistics.Nearby).ToString()));
                //if (settings.FleeingEnemiesReturn)
                //{
                //    InformationManager.DisplayMessage(new(EnemiesRestoredText.SetTextVariable("RETURNING", troopsStatistics.Fled).ToString()));
                //}
                //else
                {
                    InformationManager.DisplayMessage(new(EnemiesFledText.SetTextVariable("FLED", troopsStatistics.Fled).ToString()));
                }
            }
            else
            {
                var sb = new StringBuilder();
                if (troopsStatistics.Inspired > 0)
                {
                    sb.Append($"{troopsStatistics.Inspired} nearby friendly unit(s) are inspired");
                    sb.Append(troopsStatistics.Retreating <= 0
                        ? $"! "
                        : $", and {troopsStatistics.Retreating} retreating unit(s) have regained their resolve to fight! ");
                }

                if (troopsStatistics.Nearby > 0)
                {
                    sb.Append($"{troopsStatistics.Nearby} nearby enemy unit(s) have their courage tested");
                    if (troopsStatistics.Fled > 0)
                    {
                        sb.Append($", and {troopsStatistics.Fled} of them are shaken to the core and flee!");
                        //if (settings.FledEnemiesReturn)
                        //    sb.Append(" But they regain their composure and reengage!");
                    }
                    else
                        sb.Append(", but they all hold steadfast!");
                }

                InformationManager.DisplayMessage(new(sb.ToString()));
            }
        }
    }
}