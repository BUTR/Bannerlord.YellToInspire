using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

using System;
using System.Text;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.CustomBattle;
using YellToInspire.Skills;

namespace YellToInspire
{
    public class InspireBehaviour
    {
        private static readonly ActionIndexCache[] _cheerActions = 
        {
            ActionIndexCache.Create("act_command"),
            ActionIndexCache.Create("act_command_follow")
        };

        public static readonly string _configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json");

        public static readonly string _languagePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "language.json");

        public static InputKey _boundInput = (InputKey) 47;

        public static string _boundInputString = "V";

        public static float _positiveMoraleChange = 0f;

        public static float _negativeMoraleChange = 15f;

        public static float _minResponseDelay = 0.7f;

        public static float _maxResponseDelay = 2.2f;

        public static float _radiusIncreasePerLevelOfLeadership = 0.1f;

        public static float _cooldownDecreasePerLevelOfLeadership = 0.015f;

        public static float _moraleGainIncreasePerLevelOfLeadership = 0.05f;

        public static float _chanceToFleeIncreasePerLevelOfRoguery = 0.0015f;

        public static bool _fleeingEnemiesReturn = false;

        public static bool _showSphereIndicators = true;

        public static bool _showDetailedMessage = true;

        public static bool _enableRandomCheerDelay = true;

        public static bool _enableCheerAnimation = true;

        public static bool _enableEnemyReaction = true;

        public static float _basePositiveMoraleChange = 0.5f;

        public static float _baseChanceToFlee = 0.5f;

        private static readonly List<Agent> affectedAgents = new();

        private static readonly string[] _abilityPhrases = 
        {
            "A primal bellow echos forth from the depths of your soul! ", 
            "Your banshee howl pierces the battlefield! ", 
            "You let out a deafening warcry! ", 
            "You explode with a thunderous roar! "
        };

        public static double _cooldownStart = 0.0;

        private static bool _useCustomPhrases = false;

        private static string[] _customPhrases =
        {
            "A primal bellow echos forth from the depths of your soul! ",
            "Your banshee howl pierces the battlefield! ",
            "You let out a deafening warcry! ", 
            "You explode with a thunderous roar! "
        };

        private static bool _useCustomDetailedText = false;

        private static string _alliesInspiredCustomDetailedText = "Allied unit(s) that were inspired: ";
        private static string _alliesRestoredCustomDetailedText = "Allied unit(s) that are returning to battle: ";
        private static string _enemiesScaredCustomDetailedText = "Enemy unit(s) that had their courage tested: ";
        private static string _enemiesFledCustomDetailedText = "Enemy unit(s) that are fleeing: ";
        private static string _enemiesRestoredCustomDetailedText = "Enemy unit(s) that are returning to battle: ";

        public static Agent? MainAgent => Mission.Current?.MainAgent;

        public static float _abilityRadius { get; set; } = 10f;
        public static double _maxCooldownTime { get; set; } = 10.0;
        public static float _percentChanceToFlee { get; set; } = 0.5f;
        public static float _baseLeadershipExpPerAlly { get; set; } = 1f;
        public static float _baseRogueryExpPerEnemy { get; set; } = 1f;
        public static float _baseRadius { get; set; } = 10f;
        public static double _baseCooldownTime { get; set; } = 8.0;
        public static double _currentCooldownTime { get; set; }
        public static bool _abilityReady { get; set; } = true;


        public static void InspireAura()
        {
            affectedAgents.Clear();
            if (_abilityReady)
            {
                var total = 0;
                var retreating = 0;
                var nearby = 0;
                var fleeting = 0;

                _abilityReady = false;

                SphereIndicator.AbilityUsedPosition = MainAgent.Position;
                SphereIndicator.SphereCurrentRadius = 0f;
                if (_showSphereIndicators)
                    SphereIndicator.DisplaySphereIndicators = true;

                foreach (var nearbyAllyAgent in Mission.Current.GetNearbyAllyAgents(MainAgent.Position.AsVec2, _abilityRadius, MainAgent.Team))
                {
                    if (nearbyAllyAgent == MainAgent)
                        continue;

                    affectedAgents.Add(nearbyAllyAgent);
                    total++;
                    nearbyAllyAgent.ChangeMorale(_positiveMoraleChange);
                    if (CustomGame.Current is null)
                    {
                        if (nearbyAllyAgent.IsRetreating() && Hero.MainHero.GetPerkValue(SkillsAndTalents.InspireResolve))
                        {
                            nearbyAllyAgent.ChangeMorale(25f);
                            nearbyAllyAgent.GetComponent<CommonAIComponent>().StopRetreating();
                            retreating++;
                        }
                    }
                    else if (nearbyAllyAgent.IsRetreating())
                    {
                        nearbyAllyAgent.ChangeMorale(25f);
                        nearbyAllyAgent.GetComponent<CommonAIComponent>().StopRetreating();
                        retreating++;
                    }
                }

                foreach (var nearbyEnemyAgent in Mission.Current.GetNearbyEnemyAgents(MainAgent.Position.AsVec2, _abilityRadius, MainAgent.Team))
                {
                    affectedAgents.Add(nearbyEnemyAgent);
                    nearby++;

                    if (MBRandom.RandomFloatRanged(0f, 1f) > _percentChanceToFlee)
                        continue;
                    
                    nearbyEnemyAgent.ChangeMorale(0f - _negativeMoraleChange);
                    if (nearbyEnemyAgent.GetMorale() <= 0.0)
                    {
                        fleeting++;
                        nearbyEnemyAgent.GetComponent<CommonAIComponent>().Panic();
                        if (_fleeingEnemiesReturn)
                        {
                            nearbyEnemyAgent.ChangeMorale(_negativeMoraleChange * 2f);
                            nearbyEnemyAgent.GetComponent<CommonAIComponent>().StopRetreating();
                        }
                    }
                    else
                        nearbyEnemyAgent.ChangeMorale(_negativeMoraleChange);
                }

                if (affectedAgents.Count > 0)
                    SubModule.DelayedAgentReactions(affectedAgents, _minResponseDelay, _maxResponseDelay);

                if (CustomGame.Current is null && Mission.Current.GetMissionBehavior<BattleEndLogic>() is { PlayerVictory: false })
                {
                    Hero.MainHero.AddSkillXp(DefaultSkills.Leadership, _baseLeadershipExpPerAlly * total);
                    Hero.MainHero.AddSkillXp(DefaultSkills.Roguery, _baseRogueryExpPerEnemy * nearby);
                }

                var voiceType = SkinVoiceManager.VoiceType.Yell;
                if (fleeting > 0)
                    voiceType = SkinVoiceManager.VoiceType.Victory;
                if (retreating > 0)
                    voiceType = SkinVoiceManager.VoiceType.FaceEnemy;
                if (Mission.Current.GetMissionBehavior<BattleEndLogic>() is { PlayerVictory: true })
                    voiceType = SkinVoiceManager.VoiceType.Victory;

                if (!Mission.Current.GetNearbyEnemyAgents(MainAgent.Position.AsVec2, 8f, MainAgent.Team).Any() && _enableCheerAnimation)
                {
                    MainAgent.SetActionChannel(1, _cheerActions[MBRandom.RandomInt(_cheerActions.Length)], false, 0uL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
                    SubModule.AddToCheeringList(new CheeringAgent(MainAgent, MissionTime.Now.ToSeconds, 1.5));
                }

                MainAgent.MakeVoice(voiceType, (SkinVoiceManager.CombatVoiceNetworkPredictionType) 2);
                InformationManager.DisplayMessage(!_useCustomPhrases
                    ? new InformationMessage(_abilityPhrases[MBRandom.RandomInt(0, _abilityPhrases.Length)])
                    : new InformationMessage(_customPhrases[MBRandom.RandomInt(0, _customPhrases.Length)]));

                _cooldownStart = MissionTime.Now.ToSeconds;
                _currentCooldownTime = _cooldownStart;

                if (_showDetailedMessage)
                    ShowDetailedMessage(total, retreating, nearby, fleeting);
            }
            else
                InformationManager.DisplayMessage(new InformationMessage("Your ability is still on cooldown for " + (float) Math.Round((_maxCooldownTime - _currentCooldownTime) * 100.0) / 100f + " second(s)!"));
        }

        private static void ShowDetailedMessage(int total, int retreating, int nearby, int fleeting)
        {
            if (!_useCustomDetailedText)
            {
                var sb = new StringBuilder();
                if (total > 0)
                {
                    sb.Append($"{total} nearby friendly unit(s) are inspired");
                    sb.Append(retreating <= 0
                        ? $"! "
                        : $", and {retreating} retreating unit(s) have regained their resolve to fight! ");
                }
                if (nearby > 0)
                {
                    sb.Append($"{nearby} nearby enemy unit(s) have their courage tested");
                    if (fleeting > 0)
                    {
                        sb.Append($", and {fleeting} of them are shaken to the core and flee!");
                        if (_fleeingEnemiesReturn)
                            sb.Append(" But they regain their composure and reengage!");
                    }
                    else
                        sb.Append(", but they all hold steadfast!");
                }
                InformationManager.DisplayMessage(new InformationMessage(sb.ToString()));
            }
            else
            {
                InformationManager.DisplayMessage(new InformationMessage(_alliesInspiredCustomDetailedText + total));
                InformationManager.DisplayMessage(new InformationMessage(_alliesRestoredCustomDetailedText + retreating));
                InformationManager.DisplayMessage(new InformationMessage(_enemiesScaredCustomDetailedText + nearby));
                InformationManager.DisplayMessage(!_fleeingEnemiesReturn
                    ? new InformationMessage(_enemiesFledCustomDetailedText + fleeting)
                    : new InformationMessage(_enemiesRestoredCustomDetailedText + fleeting));
            }
        }

        public static void LoadConfig()
        {
            if (!File.Exists(_configPath)) return;

            try
            {
                _boundInput = (InputKey) JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).YellKey;
                _abilityRadius = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).AbilityRadius;
                _maxCooldownTime = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).AbilityCooldown;
                _positiveMoraleChange = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).AlliedMoraleGain;
                _negativeMoraleChange = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).EnemyFleeMoraleThreshold;
                _minResponseDelay = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).MinResponseDelay;
                _maxResponseDelay = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).MaxResponseDelay;
                _percentChanceToFlee = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).EnemyChanceToFlee / 100f;
                _baseLeadershipExpPerAlly = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).BaseLeadershipExpPerAlly;
                _baseRogueryExpPerEnemy = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).BaseRogueryExpPerEnemy;
                _radiusIncreasePerLevelOfLeadership = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).RadiusIncreasePerLevel;
                _cooldownDecreasePerLevelOfLeadership = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).CooldownDecreasePerLevel;
                _moraleGainIncreasePerLevelOfLeadership = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).MoraleGainIncreasePerLevel;
                _chanceToFleeIncreasePerLevelOfRoguery = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).ChanceToFleeIncreasePerLevel;
                _fleeingEnemiesReturn = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).FleeingEnemiesReturn;
                _showSphereIndicators = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).ShowSphereIndicators;
                _showDetailedMessage = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).ShowDetailedMessage;
                _enableRandomCheerDelay = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).EnableRandomCheerDelay;
                _enableCheerAnimation = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).EnableCheerAnimation;
                _enableEnemyReaction = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_configPath)).EnableEnemyReaction;
                _baseRadius = _abilityRadius;
                _baseCooldownTime = _maxCooldownTime;
                _basePositiveMoraleChange = _positiveMoraleChange;
                _baseChanceToFlee = _percentChanceToFlee;
            }
            catch { }
        }

        public static void LoadLanguage()
        {
            if (!File.Exists(_languagePath)) return;

            try
            {
                _useCustomPhrases = JsonConvert.DeserializeObject<Language>(File.ReadAllText(_languagePath)).UseCustomFlavorText;
                _customPhrases = JsonConvert.DeserializeObject<Language>(File.ReadAllText(_languagePath)).CustomFlavorText;
                _useCustomDetailedText = JsonConvert.DeserializeObject<Language>(File.ReadAllText(_languagePath)).UseCustomDetailedText;
                _alliesInspiredCustomDetailedText = JsonConvert.DeserializeObject<Language>(File.ReadAllText(_languagePath)).AlliesInspiredCustomDetailedText;
                _alliesRestoredCustomDetailedText = JsonConvert.DeserializeObject<Language>(File.ReadAllText(_languagePath)).AlliesRestoredCustomDetailedText;
                _enemiesScaredCustomDetailedText = JsonConvert.DeserializeObject<Language>(File.ReadAllText(_languagePath)).EnemiesScaredCustomDetailedText;
                _enemiesFledCustomDetailedText = JsonConvert.DeserializeObject<Language>(File.ReadAllText(_languagePath)).EnemiesFledCustomDetailedText;
                _enemiesRestoredCustomDetailedText = JsonConvert.DeserializeObject<Language>(File.ReadAllText(_languagePath)).EnemiesRestoredCustomDetailedText;
            }
            catch { }
        }
    }
}