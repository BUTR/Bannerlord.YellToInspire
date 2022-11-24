using Bannerlord.YellToInspire.Data;
using Bannerlord.YellToInspire.MissionBehaviors.AgentComponents;

using MCM;

using System;
using System.Collections.Generic;
using System.Text;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.Utils
{
    public static class CommonUtils
    {
        public static TextObject GetRandomAbilityPhrase() => Strings.AbilityPhrases[MBRandom.RandomInt(0, Strings.AbilityPhrases.Length)];

        /// <summary>
        /// Will work with any agent, not the main hero only
        /// </summary>
        public static TroopStatistics InspireAura(Agent causingAgent)
        {
            if (causingAgent.Mission.GetMissionBehavior<SettingsProviderMissionBehavior>() is not { } settingsProvider) return TroopStatistics.Empty;
            if (settingsProvider.Get<Settings>() is not { } settings) return TroopStatistics.Empty;

            var hero = causingAgent.Character is CharacterObject charObj ? charObj.HeroObject : null;

            if (hero is not null && hero.GetSkillValue(Skills.Leadership) >= 5 && !hero.GetPerkValue(Perks.Instance.InspireBasic))
                hero.HeroDeveloper.AddPerk(Perks.Instance.InspireBasic);

            if (causingAgent.GetComponent<SphereIndicatorAgentComponent>() is { } sphereIndicatorAgentComponent)
                sphereIndicatorAgentComponent.Trigger();

            var (troopsStatistics, affectedAgents) = AffectTroops(causingAgent, settings);
            if (causingAgent.GetComponent<InspireNearAgentsAgentComponent>() is { } inspireNearAgentsAgentComponent)
                inspireNearAgentsAgentComponent.Trigger(affectedAgents);

            if (hero is not null && causingAgent.Mission.GetMissionBehavior<BattleEndLogic>() is { PlayerVictory: false })
            {
                hero.AddSkillXp(Skills.Leadership, settings.LeadershipExpPerAlly * troopsStatistics.Inspired);
                hero.AddSkillXp(Skills.Roguery, settings.RogueryExpPerEnemy * troopsStatistics.Nearby);
            }

            var voiceType = troopsStatistics switch
            {
                { Fled: > 0 } => SkinVoiceManager.VoiceType.Victory,
                { Retreating: > 0 } => SkinVoiceManager.VoiceType.FaceEnemy,
                _ when causingAgent.Mission.GetMissionBehavior<BattleEndLogic>() is { PlayerVictory: true } => SkinVoiceManager.VoiceType.Victory,
                _ => SkinVoiceManager.VoiceType.Yell
            };
            causingAgent.MakeVoice(voiceType, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);

            return troopsStatistics;
        }

        public static (TroopStatistics Statistics, List<WeakReference<Agent>> AffectedAgents) AffectTroops(Agent causingAgent, Settings settings)
        {
            var vec2Pos = causingAgent.Position.AsVec2;
            var team = causingAgent.Team;

            var statistics = new TroopStatistics();
            var affectedAgents = new List<WeakReference<Agent>>();

            foreach (var nearbyAllyAgent in causingAgent.Mission.GetNearbyAllyAgents(vec2Pos, settings.AbilityRadius(causingAgent.Character), team))
            {
                if (nearbyAllyAgent == causingAgent) continue;

                if (nearbyAllyAgent.GetComponent<CommonAIComponent>() is not { } commonAiComponent) continue;

                statistics.Inspired++;
                affectedAgents.Add(new(nearbyAllyAgent));

                nearbyAllyAgent.ChangeMorale(settings.AlliedMoraleGain(causingAgent.Character));

                if (commonAiComponent.IsRetreating)
                {
                    if (causingAgent.Character is not CharacterObject charObj || charObj.GetPerkValue(Perks.Instance.InspireResolve))
                    {
                        nearbyAllyAgent.SetMorale(commonAiComponent.RecoveryMorale);

                        commonAiComponent.StopRetreating();
                        statistics.Retreating++;
                    }
                }
                if (commonAiComponent.IsPanicked)
                {
                    if (causingAgent.Character is not CharacterObject charObj || charObj.GetPerkValue(Perks.Instance.InspireResolve))
                    {
                        nearbyAllyAgent.SetMorale(commonAiComponent.RecoveryMorale);

                        commonAiComponent.StopRetreating();
                        statistics.Retreating++;
                    }
                }
            }

            foreach (var nearbyEnemyAgent in causingAgent.Mission.GetNearbyEnemyAgents(vec2Pos, settings.AbilityRadius(causingAgent.Character), team))
            {
                if (nearbyEnemyAgent.GetComponent<CommonAIComponent>() is not { } commonAiComponent) continue;

                statistics.Nearby++;
                affectedAgents.Add(new(nearbyEnemyAgent));

                if (MBRandom.RandomFloatRanged(0f, 1f) > settings.EnemyChanceToFlee(causingAgent.Character)) continue;

                if (nearbyEnemyAgent.GetMorale() <= settings.EnemyFleeMoraleThreshold)
                {
                    if (commonAiComponent.CanPanic())
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

        public static void ShowDetailedMessage(TroopStatistics troopsStatistics, bool showDetailedMessageText)
        {
            if (showDetailedMessageText)
            {
                InformationManager.DisplayMessage(new(Strings.AlliesInspired.SetTextVariable("INSPIRED", troopsStatistics.Inspired).ToString()));
                InformationManager.DisplayMessage(new(Strings.AlliesRestored.SetTextVariable("RETURNING", troopsStatistics.Retreating).ToString()));
                InformationManager.DisplayMessage(new(Strings.EnemiesScared.SetTextVariable("SCARED", troopsStatistics.Nearby).ToString()));
                //if (settings.FleeingEnemiesReturn)
                //{
                //    InformationManager.DisplayMessage(new(EnemiesRestoredText.SetTextVariable("RETURNING", troopsStatistics.Fled).ToString()));
                //}
                //else
                {
                    InformationManager.DisplayMessage(new(Strings.EnemiesFled.SetTextVariable("FLED", troopsStatistics.Fled).ToString()));
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

        public static TextObject SetVariables(TextObject textObject)
        {
            if (Settings.Instance is not { } settings) return textObject;
            var @char = CharacterObject.PlayerCharacter;

            return textObject
                .SetTextVariable("NEWLINE", "\n")
                .SetTextVariable("RADIUS", settings.AbilityRadius(@char))
                .SetTextVariable("MORALE", settings.AlliedMoraleGain(@char))
                .SetTextVariable("FLEE", settings.EnemyChanceToFlee(@char))
                .SetTextVariable("COOLDOWN", settings.AbilityCooldown(@char));
        }
    }
}