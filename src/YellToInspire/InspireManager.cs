using HarmonyLib.BUTR.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

using YellToInspire.Skills;

namespace YellToInspire
{
    public sealed class InspireManager : IDisposable
    {
        private delegate void SetAllPerksDelegate(Campaign instance, MBReadOnlyList<PerkObject> perks);
        private static readonly SetAllPerksDelegate? SetAllPerks =
            AccessTools2.GetPropertySetterDelegate<SetAllPerksDelegate>(typeof(Campaign), "AllPerks");

        public static InspireManager Current { get; private set; }

        private static Agent? MainAgent => Mission.Current?.MainAgent;
        private static Hero? Hero => CharacterObject.PlayerCharacter?.HeroObject;

        private readonly AbilitySphereIndicator _sphereIndicator = new();
        private readonly List<Agent> _affectedAgents = new();
        private readonly List<CheeringAgent> _cheeringAgents = new();

        /// <summary>
        /// Will be null when not a campaign
        /// </summary>
        private SkillsAndTalents? _skillsAndTalents;
        //private bool _isNew;
        private bool _unitsResponded = true;
        private double _cooldownSnapshot;
        private double PastCooldown => MissionTime.Now.ToSeconds - _cooldownSnapshot;
        private bool AbilityReady => _cooldownSnapshot == 0 || PastCooldown > Settings.Instance?.AbilityCooldown(Hero);

        public InspireManager()
        {
            Current = this;
        }

        public void OnNewGameCreated(IGameStarter starter)
        {
            //_isNew = true;
        }

        public void SetupCampaign(CampaignGameStarter campaignGameStarter)
        {
            _skillsAndTalents = new SkillsAndTalents(Game.Current);
            if (SetAllPerks is not null)
                SetAllPerks(Campaign.Current, MBObjectManager.Instance.GetObjectTypeList<PerkObject>());

            //if (_isNew) return;
            //
            //foreach (var item in Hero.AllAliveHeroes)
            //    item.HeroDeveloper.SetInitialSkillLevel(DefaultSkills.Leadership, item.GetSkillValue(DefaultSkills.Leadership));
        }

        public void MissionStarted()
        {

        }

        public void MissionEnded()
        {
            _cheeringAgents.Clear();
            _affectedAgents.Clear();
            _cooldownSnapshot = 0d;
            _sphereIndicator.Reset();
            MBDebug.ClearRenderObjects();
        }

        public void MissionTick(float dt)
        {
            if (MBCommon.IsPaused) return;

            _sphereIndicator.Tick(dt);

            if (_cheeringAgents.Count > 0)
            {
                StopCheeringAgentsAfterDelay();
            }

            if (!_unitsResponded)
            {
                if (_affectedAgents.Count == 0)
                {
                    _unitsResponded = true;
                }
                else
                {
                    DelayAndReact();
                }
            }

            if (MainAgent is null || !MainAgent.IsPlayerControlled)
                return;

            if ((Hero?.GetSkillValue(DefaultSkills.Leadership) ?? 0) >= 5 && Hero?.GetPerkValue(SkillsAndTalents.InspireBasic) == false)
                Hero.HeroDeveloper.AddPerk(SkillsAndTalents.InspireBasic);
        }

        public void CampaignTick(float dt)
        {
            _skillsAndTalents?.Update(dt);
        }


        public void InspireAura()
        {
            if (Settings.Instance is not { } settings) return;
            if (MainAgent is null) return;

            if (AbilityReady)
            {
                _sphereIndicator.Trigger();

                _cooldownSnapshot = MissionTime.Now.ToSeconds;
                var (troopsStatistics, affectedAgents) = Utils.IterateTroops();

                if (affectedAgents.Count > 0)
                    DelayedAgentReactions(affectedAgents);

                if (Hero is not null && Mission.Current.GetMissionBehavior<BattleEndLogic>() is { PlayerVictory: false })
                {
                    Hero.AddSkillXp(DefaultSkills.Leadership, settings.BaseLeadershipExpPerAlly * troopsStatistics.Inspired);
                    Hero.AddSkillXp(DefaultSkills.Roguery, settings.BaseRogueryExpPerEnemy * troopsStatistics.Nearby);
                }

                var voiceType = SkinVoiceManager.VoiceType.Yell;
                if (troopsStatistics.Fleeting > 0)
                    voiceType = SkinVoiceManager.VoiceType.Victory;
                if (troopsStatistics.Retreating > 0)
                    voiceType = SkinVoiceManager.VoiceType.FaceEnemy;
                if (Mission.Current.GetMissionBehavior<BattleEndLogic>() is { PlayerVictory: true })
                    voiceType = SkinVoiceManager.VoiceType.Victory;

                if (!Mission.Current.GetNearbyEnemyAgents(MainAgent.Position.AsVec2, 8f, MainAgent.Team).Any() && settings.EnableCheerAnimation)
                {
                    MainAgent.SetActionChannel(1, Shared.CheerActions[MBRandom.RandomInt(Shared.CheerActions.Length)]);
                    _cheeringAgents.Add(new CheeringAgent(MainAgent, MissionTime.Now.ToSeconds, 1.5));
                }

                MainAgent.MakeVoice(voiceType, (SkinVoiceManager.CombatVoiceNetworkPredictionType) 2);
                InformationManager.DisplayMessage(new(Shared.AbilityPhrases[MBRandom.RandomInt(0, Shared.AbilityPhrases.Length)].ToString()));

                if (settings.ShowDetailedMessage)
                    Utils.ShowDetailedMessage(troopsStatistics);
            }
            else
            {
                var time = (float) (settings.AbilityCooldown(Hero) - PastCooldown);
                InformationManager.DisplayMessage(new(Shared.CooldownText.SetTextVariable("TIME", time).ToString()));
            }
        }


        private void DelayAndReact()
        {
            if (Settings.Instance is not { } settings) return;
            if (MainAgent is null) return;

            foreach (var item in _affectedAgents.ToList())
            {
                if (item is null)
                    continue;

                if (!item.IsActive())
                {
                    _affectedAgents.Remove(item);
                    continue;
                }

                var delay = settings.EnableCheerRandomDelay ? MBRandom.RandomFloatRanged(settings.MinResponseDelay, settings.MaxResponseDelay) : 1f;
                if (MissionTime.Now.ToSeconds < _cooldownSnapshot + delay)
                    continue;

                if (item.Team == MainAgent.Team)
                {
                    item.SetWantsToYell();
                    if (!Mission.Current.GetNearbyEnemyAgents(item.Position.AsVec2, 8f, item.Team).Any() && settings.EnableCheerAnimation)
                    {
                        item.SetActionChannel(1, Shared.CheerActions[MBRandom.RandomInt(Shared.CheerActions.Length)]);
                        _cheeringAgents.Add(new CheeringAgent(item, MissionTime.Now.ToSeconds, 1.5));
                    }
                }
                else if (settings.EnableEnemyReaction)
                {
                    item.MakeVoice(SkinVoiceManager.VoiceType.Fear, (SkinVoiceManager.CombatVoiceNetworkPredictionType) 2);
                }
                _affectedAgents.Remove(item);
            }
            if (_affectedAgents.Count == 0)
            {
                _unitsResponded = true;
            }
        }

        private void StopCheeringAgentsAfterDelay()
        {
            foreach (var item in _cheeringAgents.ToList())
            {
                if (Mission.Current is null)
                {
                    _cheeringAgents.Clear();
                    break;
                }

                if (item.Agent is null)
                {
                    _cheeringAgents.Remove(item);
                    continue;
                }

                if (MissionTime.Now.ToSeconds < item.InitialTime + item.TimeDelay)
                {
                    if (!Mission.Current.GetNearbyEnemyAgents(item.Agent.Position.AsVec2, 8f, item.Agent.Team).Any())
                        continue;
                }

                item.Agent.SetActionChannel(1, ActionIndexCache.act_none, true, 0uL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
                _cheeringAgents.Remove(item);
            }
        }

        private void DelayedAgentReactions(List<Agent> agents)
        {
            _affectedAgents.Clear();

            if (agents.Count == 0)
                return;

            foreach (var item in agents.ToList())
                _affectedAgents.Add(item);

            _unitsResponded = false;
        }


        public void Dispose()
        {
            _skillsAndTalents?.Dispose();
        }
    }
}