using System;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

using YellToInspire.Components;

namespace YellToInspire
{
    internal sealed class InspireManager : IDisposable
    {
        private static readonly TextObject CooldownText = new("{=FsbQpeMJYJ}Your ability is still on cooldown for {TIME} second(s)!");
        private static readonly TextObject[] AbilityPhrases =
        {
            new("{=OG3KT8KQvR}A primal bellow echos forth from the depths of your soul!"),
            new("{=0SKEk61Zj0}Your banshee howl pierces the battlefield!"),
            new("{=HT9YJgKEl2}You let out a deafening warcry!"),
            new("{=Nr6docL7MZ}You explode with a thunderous roar!")
        };

        public static InspireManager? Current { get; private set; }

        private readonly AbilitySphereIndicator _sphereIndicator = new();
        private readonly BattlefieldManager _battlefieldManager = new();

        private double _cooldownSnapshot;
        private double PastCooldown => MissionTime.Now.ToSeconds - _cooldownSnapshot;

        public InspireManager()
        {
            Current = this;
        }

        public void MissionStarted()
        {

        }

        public void MissionEnded()
        {
            _cooldownSnapshot = 0d;
            _sphereIndicator.Reset();
            _battlefieldManager.Reset();
        }

        public void MissionTick(float dt)
        {
            if (MBCommon.IsPaused) return;

            _sphereIndicator.Tick(dt);
            _battlefieldManager.Tick(dt);
        }

        /// <summary>
        /// Will work with any agent, not the main hero only
        /// </summary>
        public void InspireAura(Agent causingAgent)
        {
            if (Settings.Instance is not { } settings) return;

            var hero = causingAgent.Character is CharacterObject charObj ? charObj.HeroObject : null;

            if (hero is not null && hero.GetSkillValue(Skills.Leadership) >= 5 && !hero.GetPerkValue(Perks.InspireBasic))
                hero.HeroDeveloper.AddPerk(Perks.InspireBasic);

            if (_cooldownSnapshot != 0 && !(PastCooldown > settings.AbilityCooldown(causingAgent.Character)))
            {
                var time = settings.AbilityCooldown(causingAgent.Character) - PastCooldown;
                InformationManager.DisplayMessage(new(CooldownText.SetTextVariable("TIME", (float) time).ToString()));
                return;
            }

            _sphereIndicator.Trigger(causingAgent);
            _battlefieldManager.Trigger(causingAgent);
            _cooldownSnapshot = MissionTime.Now.ToSeconds;

            var (troopsStatistics, affectedAgents) = Utils.AffectTroops(causingAgent);
            if (affectedAgents.Count > 0)
                _battlefieldManager.DelayedAgentReactions(affectedAgents);

#if e164
            if (hero is not null && Mission.Current.GetMissionBehaviour<BattleEndLogic>() is { PlayerVictory: false })
#elif e170
            if (hero is not null && Mission.Current.GetMissionBehavior<BattleEndLogic>() is { PlayerVictory: false })
#else
#error NOT SET
#endif
            {
                hero.AddSkillXp(Skills.Leadership, settings.LeadershipExpPerAlly * troopsStatistics.Inspired);
                hero.AddSkillXp(Skills.Roguery, settings.RogueryExpPerEnemy * troopsStatistics.Nearby);
            }

            if (!Mission.Current.GetNearbyEnemyAgents(causingAgent.Position.AsVec2, 8f, causingAgent.Team).Any() && settings.EnableCheerAnimation)
            {
                _battlefieldManager.Cheer(causingAgent);
            }

            var voiceType = troopsStatistics switch
            {
                { Fled: > 0 } => SkinVoiceManager.VoiceType.Victory,
                { Retreating: > 0 } => SkinVoiceManager.VoiceType.FaceEnemy,
#if e164
                _ when Mission.Current.GetMissionBehaviour<BattleEndLogic>() is { PlayerVictory: true } => SkinVoiceManager.VoiceType.Victory,
#elif e170
                _ when Mission.Current.GetMissionBehavior<BattleEndLogic>() is { PlayerVictory: true } => SkinVoiceManager.VoiceType.Victory,
#else
#error NOT SET
#endif
                _ => SkinVoiceManager.VoiceType.Yell
            };
            causingAgent.MakeVoice(voiceType, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);

            InformationManager.DisplayMessage(new(AbilityPhrases[MBRandom.RandomInt(0, AbilityPhrases.Length)].ToString()));

            if (settings.ShowDetailedMessage)
                Utils.ShowDetailedMessage(troopsStatistics);
        }



        public void Dispose()
        {
            _sphereIndicator?.Dispose();
            _battlefieldManager.Dispose();
        }
    }
}