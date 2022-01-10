using System.Collections.Generic;
using System.Text;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

using YellToInspire.Skills;

namespace YellToInspire
{
    internal static class Shared
    {
        public static readonly TextObject CooldownText = new("{=FsbQpeMJYJ}Your ability is still on cooldown for {TIME} second(s)!");
        public static readonly TextObject AlliesInspiredText = new("{=gqr3o3aOfu}Allied unit(s) that were inspired: {INSPIRED}");
        public static readonly TextObject AlliesRestoredText = new("{=5fapWaqKVw}Allied unit(s) that are returning to battle: {RETURNING}");
        public static readonly TextObject EnemiesScaredText = new("{=Ldui1l6RMK}Enemy unit(s) that had their courage tested: {SCARED}");
        public static readonly TextObject EnemiesFledText = new("{=8aQ7tO6TbA}Enemy unit(s) that are fleeing: {FLED}");
        public static readonly TextObject EnemiesRestoredText = new("{=zy6xciLnky}Enemy unit(s) that are returning to battle: {RETURNING}");
        public static readonly TextObject[] AbilityPhrases =
        {
            new("{=OG3KT8KQvR}A primal bellow echos forth from the depths of your soul!"),
            new("{=0SKEk61Zj0}Your banshee howl pierces the battlefield!"),
            new("{=HT9YJgKEl2}You let out a deafening warcry!"),
            new("{=Nr6docL7MZ}You explode with a thunderous roar!")
        };

        public static readonly ActionIndexCache[] CheerActions =
        {
            ActionIndexCache.Create("act_command"),
            ActionIndexCache.Create("act_command_follow")
        };
    }

    internal static class Utils
    {
        public struct TroopsStatistics
        {
            public int Inspired;
            public int Retreating;
            public int Nearby;
            public int Fleeting;
        }

        private static Agent? MainAgent => Mission.Current?.MainAgent;
        private static Hero? Hero => CharacterObject.PlayerCharacter?.HeroObject;

        public static (TroopsStatistics, List<Agent>) IterateTroops()
        {
            if (Settings.Instance is not { } settings) return default;
            if (MainAgent is null) return default;

            var data = new TroopsStatistics();
            var list = new List<Agent>();

            foreach (var nearbyAllyAgent in Mission.Current.GetNearbyAllyAgents(MainAgent.Position.AsVec2,
                         settings.AbilityRadius(Hero), MainAgent.Team))
            {
                if (nearbyAllyAgent == MainAgent) continue;

                data.Inspired++;
                list.Add(nearbyAllyAgent);
                nearbyAllyAgent.ChangeMorale(settings.AlliedMoraleGain(Hero));

                if (Hero is not null)
                {
                    if (nearbyAllyAgent.IsRetreating() && Hero.GetPerkValue(SkillsAndTalents.InspireResolve))
                    {
                        nearbyAllyAgent.ChangeMorale(25f);
                        nearbyAllyAgent.GetComponent<CommonAIComponent>().StopRetreating();
                        data.Retreating++;
                    }
                }
                else if (nearbyAllyAgent.IsRetreating())
                {
                    nearbyAllyAgent.ChangeMorale(25f);
                    nearbyAllyAgent.GetComponent<CommonAIComponent>().StopRetreating();
                    data.Retreating++;
                }
            }

            foreach (var nearbyEnemyAgent in Mission.Current.GetNearbyEnemyAgents(MainAgent.Position.AsVec2,
                         settings.AbilityRadius(Hero), MainAgent.Team))
            {
                data.Nearby++;
                list.Add(nearbyEnemyAgent);

                if (MBRandom.RandomFloatRanged(0f, 1f) > settings.EnemyChanceToFlee(Hero)) continue;

                nearbyEnemyAgent.ChangeMorale(0f - settings.EnemyFleeMoraleThreshold);
                if (nearbyEnemyAgent.GetMorale() <= 0.0)
                {
                    data.Fleeting++;
                    nearbyEnemyAgent.GetComponent<CommonAIComponent>().Panic();
                    if (settings.FleeingEnemiesReturn)
                    {
                        nearbyEnemyAgent.ChangeMorale(settings.EnemyFleeMoraleThreshold * 2f);
                        nearbyEnemyAgent.GetComponent<CommonAIComponent>().StopRetreating();
                    }
                }
                else
                    nearbyEnemyAgent.ChangeMorale(settings.EnemyFleeMoraleThreshold);
            }

            return (data, list);
        }

        public static void ShowDetailedMessage(TroopsStatistics troopsStatistics)
        {
            if (Settings.Instance is not { } settings) return;

            if (settings.ShowDetailedMessageText)
            {
                
                InformationManager.DisplayMessage(new(Shared.AlliesInspiredText.SetTextVariable("INSPIRED", troopsStatistics.Inspired).ToString()));
                InformationManager.DisplayMessage(new(Shared.AlliesRestoredText.SetTextVariable("RETURNING", troopsStatistics.Retreating).ToString()));
                InformationManager.DisplayMessage(new(Shared.EnemiesScaredText.SetTextVariable("SCARED", troopsStatistics.Nearby).ToString()));
                InformationManager.DisplayMessage(settings.FleeingEnemiesReturn
                    ? new(Shared.EnemiesRestoredText.SetTextVariable("RETURNING", troopsStatistics.Fleeting).ToString())
                    : new(Shared.EnemiesFledText.SetTextVariable("FLED", troopsStatistics.Fleeting).ToString())
                );
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
                    if (troopsStatistics.Fleeting > 0)
                    {
                        sb.Append($", and {troopsStatistics.Fleeting} of them are shaken to the core and flee!");
                        if (settings.FleeingEnemiesReturn)
                            sb.Append(" But they regain their composure and reengage!");
                    }
                    else
                        sb.Append(", but they all hold steadfast!");
                }

                InformationManager.DisplayMessage(new(sb.ToString()));
            }
        }
    }
}
