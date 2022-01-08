using HarmonyLib.BUTR.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.CustomBattle;
using TaleWorlds.ObjectSystem;

using YellToInspire.Skills;

namespace YellToInspire
{
    public class Main : MBSubModuleBase
    {
        private delegate void SetAllPerksDelegate(Campaign instance, MBReadOnlyList<PerkObject> perks);
        private static readonly SetAllPerksDelegate? SetAllPerks =
            AccessTools2.GetPropertySetterDelegate<SetAllPerksDelegate>(typeof(Campaign), "AllPerks");

        private static readonly List<Agent> _affectedAgents = new();
        private static readonly List<CheeringAgent> _cheeringAgents = new(); 
        private static bool _newCampaign = true;
        private static bool _inMission = false;
        private static int _agentCount = 0;
        private static bool _updatePerkOnLoad = false;
        private static bool _unitsResponded = true;
        private static float _minResponseDelay = 0.7f;
        private static float _maxResponseDelay = 2.2f;

        public SkillsAndTalents SkillsAndTalents;
        public bool IsNew = true;

        private readonly ActionIndexCache[] _cheerActions = new ActionIndexCache[5]
        {
            ActionIndexCache.Create("act_command"),
            ActionIndexCache.Create("act_command"),
            ActionIndexCache.Create("act_command"),
            ActionIndexCache.Create("act_command"),
            ActionIndexCache.Create("act_command_follow")
        };

        private Hero _mainHero;
        private int _currentLeadership = 0;
        private int _currentRoguery = 0;

        protected override void OnSubModuleLoad()
        {
            InspireBehaviour.LoadConfig();
            InspireBehaviour.LoadLanguage();

            base.OnSubModuleLoad();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            var val = (CampaignGameStarter) ((gameStarterObject is CampaignGameStarter) ? gameStarterObject : null);
            if (val is not null)
            {
                val.AddBehavior(new InspireCampaignBehaviour());
            }

            base.OnGameStart(game, gameStarterObject);
        }

        public override bool DoLoading(Game game)
        {
            InspireBehaviour.LoadConfig();
            if (CustomGame.Current is null)
            {
                LoadPerks(game);
            }
            return true;
        }

        private void LoadPerks(Game game)
        {
            SkillsAndTalents = new SkillsAndTalents(game);
            if (SetAllPerks is not null)
                SetAllPerks(Campaign.Current, MBObjectManager.Instance.GetObjectTypeList<PerkObject>());

            if (IsNew) return;

            foreach (var item in Hero.AllAliveHeroes)
                item.HeroDeveloper.SetInitialSkillLevel(DefaultSkills.Leadership, item.GetSkillValue(DefaultSkills.Leadership));
        }

        public override void OnCampaignStart(Game game, object starterObject)
        {
            IsNew = true;
        }

        public override void OnGameEnd(Game game)
        {
            _newCampaign = true;
            _updatePerkOnLoad = false;

           base.OnGameEnd(game);
        }

        protected override void OnApplicationTick(float dt)
        {
            if (Campaign.Current is not null)
            {
                if (_newCampaign && CharacterObject.PlayerCharacter is not null)
                {
                    _newCampaign = false;
                }
                if (!_newCampaign)
                {
                    _mainHero = Hero.MainHero;
                    UpdatePerks();
                }
            }

            if (Mission.Current is null)
            {
                _inMission = false;
                InspireBehaviour._abilityReady = true;
                InspireBehaviour._currentCooldownTime = 0.0;
                _cheeringAgents.Clear();
                _affectedAgents.Clear();
                MBDebug.ClearRenderObjects();
                return;
            }

            if (Agent.Main is not null && SphereIndicator.DisplaySphereIndicators)
            {
                EnableIndicators();
            }

            if (_cheeringAgents.Count > 0)
            {
                StopCheeringAgentsAfterDelay();
            }

            if (!InspireBehaviour._abilityReady)
            {
                var now = MissionTime.Now;
                InspireBehaviour._currentCooldownTime = now.ToSeconds - InspireBehaviour._cooldownStart;
                if (InspireBehaviour._currentCooldownTime >= InspireBehaviour._maxCooldownTime - (InspireBehaviour._maxCooldownTime - 0.5))
                {
                    DisableIndicators();
                }
                if (InspireBehaviour._currentCooldownTime >= InspireBehaviour._maxCooldownTime)
                {
                    ResetCooldownStatus();
                }
            }

            if (!_unitsResponded)
            {
                if (_affectedAgents.Count == 0 || _affectedAgents is null)
                {
                    _unitsResponded = true;
                }
                else
                {
                    DelayAndReact();
                }
            }

            if (InspireBehaviour.MainAgent is null || !InspireBehaviour.MainAgent.IsPlayerControlled)
                return;
            
            if (CustomGame.Current is null && _mainHero.GetSkillValue(DefaultSkills.Leadership) >= 5 && !_mainHero.GetPerkValue(SkillsAndTalents.InspireBasic))
                _mainHero.HeroDeveloper.AddPerk(SkillsAndTalents.InspireBasic);
            
            if (!InspireBehaviour._boundInput.IsPressed() || !InspireBehaviour.MainAgent.IsActive())
                return;
            
            if (CustomGame.Current is null)
            {
                if (_mainHero.GetPerkValue(SkillsAndTalents.InspireBasic))
                    InspireBehaviour.InspireAura();
            }
            else
                InspireBehaviour.InspireAura();

            base.OnApplicationTick(dt);
        }

        private void EnableIndicators()
        {
            SphereIndicator.SphereCurrentRadius = MBMath.Lerp(SphereIndicator.SphereCurrentRadius, InspireBehaviour._abilityRadius, 0.1f, 1E-05f);

            MBDebug.RenderDebugSphere(
                new Vec3(
                    SphereIndicator.AbilityUsedPosition.X,
                    SphereIndicator.AbilityUsedPosition.Y,
                    Mission.Current.Scene.GetGroundHeightAtPosition(SphereIndicator.AbilityUsedPosition, BodyFlags.CommonCollisionExcludeFlags), -1f),
                SphereIndicator.SphereCurrentRadius,
                uint.MaxValue,
                true,
                0.05f);
        }

        private void DisableIndicators()
        {
            SphereIndicator.DisplaySphereIndicators = false;
        }

        private void ResetCooldownStatus()
        {
            InspireBehaviour._abilityReady = true;
        }

        private void UpdatePerks()
        {
            _currentLeadership = _mainHero.GetSkillValue(DefaultSkills.Leadership);
            _currentRoguery = _mainHero.GetSkillValue(DefaultSkills.Roguery);
            InspireBehaviour._abilityRadius = InspireBehaviour._baseRadius + _mainHero.GetSkillValue(DefaultSkills.Leadership) * InspireBehaviour._radiusIncreasePerLevelOfLeadership;
            InspireBehaviour._maxCooldownTime = Math.Round(InspireBehaviour._baseCooldownTime - _mainHero.GetSkillValue(DefaultSkills.Leadership) * InspireBehaviour._cooldownDecreasePerLevelOfLeadership, 2);
            InspireBehaviour._positiveMoraleChange = (float) Math.Round(InspireBehaviour._basePositiveMoraleChange + _mainHero.GetSkillValue(DefaultSkills.Leadership) * InspireBehaviour._moraleGainIncreasePerLevelOfLeadership, 2);
            InspireBehaviour._percentChanceToFlee = (float) Math.Round(InspireBehaviour._baseChanceToFlee + _mainHero.GetSkillValue(DefaultSkills.Roguery) * InspireBehaviour._chanceToFleeIncreasePerLevelOfRoguery, 4);
            if (InspireBehaviour._percentChanceToFlee > 1f)
            {
                InspireBehaviour._percentChanceToFlee = 1f;
            }
            SkillsAndTalents.ReinitializePerks();
        }

        private void DelayAndReact()
        {
            foreach (var item in _affectedAgents.ToList())
            {
                if (item is null || !item.IsActive())
                {
                    _affectedAgents.Remove(item);
                    continue;
                }

                var delay = InspireBehaviour._enableRandomCheerDelay ? MBRandom.RandomFloatRanged(_minResponseDelay, _maxResponseDelay) : 1f;
                if (MissionTime.Now.ToSeconds < InspireBehaviour._cooldownStart + delay)
                    continue;

                if (item.Team == Agent.Main.Team)
                {
                    item.SetWantsToYell();
                    if (!Mission.Current.GetNearbyEnemyAgents(item.Position.AsVec2, 8f, item.Team).Any() && InspireBehaviour._enableCheerAnimation)
                    {
                        item.SetActionChannel(1, _cheerActions[MBRandom.RandomInt(_cheerActions.Length)], false, 0uL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
                        AddToCheeringList(new CheeringAgent(item, MissionTime.Now.ToSeconds, 1.5));
                    }
                }
                else if (InspireBehaviour._enableEnemyReaction)
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

        private static void StopCheeringAgentsAfterDelay()
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

        public static void DelayedAgentReactions(List<Agent> agents, float timeInSecondsMin, float timeInSecondsMax)
        {
            _affectedAgents.Clear();
            _minResponseDelay = timeInSecondsMin;
            _maxResponseDelay = timeInSecondsMax;

            if (agents.Count == 0)
                return;

            foreach (var item in agents.ToList())
                _affectedAgents.Add(item);

            _unitsResponded = false;
        }

        public static void AddToCheeringList(CheeringAgent cheeringAgent)
        {
            _cheeringAgents.Add(cheeringAgent);
        }
    }
}