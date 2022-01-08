using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using YellToInspire.Skills;

namespace YellToInspire;

public class YellToInspireBehavior
{
	private static readonly ActionIndexCache[] _cheerActions = (ActionIndexCache[])(object)new ActionIndexCache[2]
	{
		ActionIndexCache.Create("act_command"),
		ActionIndexCache.Create("act_command_follow")
	};

	public static readonly string _configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json");

	public static readonly string _languagePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "language.json");

	public static InputKey _boundInput = (InputKey)47;

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

	public static List<Agent> affectedAgents = new List<Agent>();

	public static string[] _abilityPhrases = new string[4] { "A primal bellow echos forth from the depths of your soul! ", "Your banshee howl pierces the battlefield! ", "You let out a deafening warcry! ", "You explode with a thunderous roar! " };

	public static double _cooldownStart = 0.0;

	public static bool _useCustomPhrases = false;

	public static string[] _customPhrases = new string[4] { "A primal bellow echos forth from the depths of your soul! ", "Your banshee howl pierces the battlefield! ", "You let out a deafening warcry! ", "You explode with a thunderous roar! " };

	public static bool _useCustomDetailedText = false;

	public static string _alliesInspiredCustomDetailedText = "Allied unit(s) that were inspired: ";

	public static string _alliesRestoredCustomDetailedText = "Allied unit(s) that are returning to battle: ";

	public static string _enemiesScaredCustomDetailedText = "Enemy unit(s) that had their courage tested: ";

	public static string _enemiesFledCustomDetailedText = "Enemy unit(s) that are fleeing: ";

	public static string _enemiesRestoredCustomDetailedText = "Enemy unit(s) that are returning to battle: ";

	public static float _abilityRadius { get; set; } = 10f;


	public static double _maxCooldownTime { get; set; } = 10.0;


	public static float _percentChanceToFlee { get; set; } = 0.5f;


	public static float _baseLeadershipExpPerAlly { get; set; } = 1f;


	public static float _baseRogueryExpPerEnemy { get; set; } = 1f;


	public static float _baseRadius { get; set; } = 10f;


	public static double _baseCooldownTime { get; set; } = 8.0;


	public static Mission currentMission { get; set; }

	public static Agent mainAgent { get; set; }

	public static double _currentCooldownTime { get; set; }

	public static bool _abilityReady { get; set; } = true;


	public static void InspireAura()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Expected O, but got Unknown
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Expected O, but got Unknown
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Expected O, but got Unknown
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Expected O, but got Unknown
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Expected O, but got Unknown
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Expected O, but got Unknown
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Expected O, but got Unknown
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Expected O, but got Unknown
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Unknown result type (might be due to invalid IL or missing references)
		//IL_067f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Expected O, but got Unknown
		affectedAgents.Clear();
		if (_abilityReady)
		{
			_abilityReady = false;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			string text = "";
			YellToInspireSphereIndicator.AbilityUsedPosition = mainAgent.get_Position();
			YellToInspireSphereIndicator.SphereCurrentRadius = 0f;
			if (_showSphereIndicators)
			{
				YellToInspireSphereIndicator.DisplaySphereIndicators = true;
			}
			Mission current = Mission.get_Current();
			Vec3 position = mainAgent.get_Position();
			foreach (Agent nearbyAllyAgent in current.GetNearbyAllyAgents(((Vec3)(ref position)).get_AsVec2(), _abilityRadius, mainAgent.get_Team()))
			{
				if (nearbyAllyAgent == mainAgent)
				{
					continue;
				}
				affectedAgents.Add(nearbyAllyAgent);
				num++;
				AgentComponentExtensions.ChangeMorale(nearbyAllyAgent, _positiveMoraleChange);
				if (CustomGame.get_Current() == null)
				{
					if (nearbyAllyAgent.IsRetreating() && Hero.get_MainHero().GetPerkValue(YellToInspireSkillsAndTalents.InspireResolve))
					{
						AgentComponentExtensions.ChangeMorale(nearbyAllyAgent, 25f);
						nearbyAllyAgent.GetComponent<MoraleAgentComponent>().StopRetreating();
						num2++;
					}
				}
				else if (nearbyAllyAgent.IsRetreating())
				{
					AgentComponentExtensions.ChangeMorale(nearbyAllyAgent, 25f);
					nearbyAllyAgent.GetComponent<MoraleAgentComponent>().StopRetreating();
					num2++;
				}
			}
			Mission current3 = Mission.get_Current();
			position = mainAgent.get_Position();
			foreach (Agent nearbyEnemyAgent in current3.GetNearbyEnemyAgents(((Vec3)(ref position)).get_AsVec2(), _abilityRadius, mainAgent.get_Team()))
			{
				affectedAgents.Add(nearbyEnemyAgent);
				num3++;
				if (!(MBRandom.RandomFloatRanged(0f, 1f) < _percentChanceToFlee))
				{
					continue;
				}
				AgentComponentExtensions.ChangeMorale(nearbyEnemyAgent, 0f - _negativeMoraleChange);
				if ((double)AgentComponentExtensions.GetMorale(nearbyEnemyAgent) <= 0.0)
				{
					num4++;
					nearbyEnemyAgent.GetComponent<MoraleAgentComponent>().Panic();
					if (_fleeingEnemiesReturn)
					{
						AgentComponentExtensions.ChangeMorale(nearbyEnemyAgent, _negativeMoraleChange * 2f);
						nearbyEnemyAgent.GetComponent<MoraleAgentComponent>().StopRetreating();
					}
				}
				else
				{
					AgentComponentExtensions.ChangeMorale(nearbyEnemyAgent, _negativeMoraleChange);
				}
			}
			if (affectedAgents.Count > 0)
			{
				YellToInspireSubmodule.DelayedAgentReactions(affectedAgents, _minResponseDelay, _maxResponseDelay);
			}
			if (CustomGame.get_Current() == null && Mission.get_Current().GetMissionBehaviour<BattleEndLogic>() != null && !Mission.get_Current().GetMissionBehaviour<BattleEndLogic>().get_PlayerVictory())
			{
				Hero.get_MainHero().AddSkillXp(DefaultSkills.get_Leadership(), _baseLeadershipExpPerAlly * (float)num);
				Hero.get_MainHero().AddSkillXp(DefaultSkills.get_Roguery(), _baseRogueryExpPerEnemy * (float)num3);
			}
			SkinVoiceType val = VoiceType.Yell;
			if (num4 > 0)
			{
				val = VoiceType.Victory;
			}
			if (num2 > 0)
			{
				val = VoiceType.FaceEnemy;
			}
			if (Mission.get_Current().GetMissionBehaviour<BattleEndLogic>() != null && Mission.get_Current().GetMissionBehaviour<BattleEndLogic>().get_PlayerVictory())
			{
				val = VoiceType.Victory;
			}
			Mission current5 = Mission.get_Current();
			position = mainAgent.get_Position();
			MissionTime now;
			if (current5.GetNearbyEnemyAgents(((Vec3)(ref position)).get_AsVec2(), 8f, mainAgent.get_Team()).Count() <= 0 && _enableCheerAnimation)
			{
				mainAgent.SetActionChannel(1, _cheerActions[MBRandom.RandomInt(_cheerActions.Length)], false, 0uL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
				Agent agent = mainAgent;
				now = MissionTime.get_Now();
				YellToInspireSubmodule.AddToCheeringList(new CheeringAgent(agent, ((MissionTime)(ref now)).get_ToSeconds(), 1.5));
			}
			mainAgent.MakeVoice(val, (CombatVoiceNetworkPredictionType)2);
			if (!_useCustomPhrases)
			{
				InformationManager.DisplayMessage(new InformationMessage(_abilityPhrases[MBRandom.RandomInt(0, _abilityPhrases.Length)]));
			}
			else
			{
				InformationManager.DisplayMessage(new InformationMessage(_customPhrases[MBRandom.RandomInt(0, _customPhrases.Length)]));
			}
			if (_showDetailedMessage)
			{
				if (!_useCustomDetailedText)
				{
					if (num > 0)
					{
						text = text + num + " nearby friendly unit(s) are inspired";
						text = ((num2 <= 0) ? (text + "! ") : (text + ", and " + num2 + " retreating unit(s) have regained their resolve to fight! "));
					}
					if (num3 > 0)
					{
						text = text + num3 + " nearby enemy unit(s) have their courage tested";
						if (num4 > 0)
						{
							text = text + ", and " + num4 + " of them are shaken to the core and flee!";
							if (_fleeingEnemiesReturn)
							{
								text += " But they regain their composure and reengage!";
							}
						}
						else
						{
							text += ", but they all hold steadfast!";
						}
					}
					InformationManager.DisplayMessage(new InformationMessage(text));
				}
				else
				{
					InformationManager.DisplayMessage(new InformationMessage(_alliesInspiredCustomDetailedText + num));
					InformationManager.DisplayMessage(new InformationMessage(_alliesRestoredCustomDetailedText + num2));
					InformationManager.DisplayMessage(new InformationMessage(_enemiesScaredCustomDetailedText + num3));
					if (!_fleeingEnemiesReturn)
					{
						InformationManager.DisplayMessage(new InformationMessage(_enemiesFledCustomDetailedText + num4));
					}
					else
					{
						InformationManager.DisplayMessage(new InformationMessage(_enemiesRestoredCustomDetailedText + num4));
					}
				}
			}
			now = MissionTime.get_Now();
			_cooldownStart = ((MissionTime)(ref now)).get_ToSeconds();
			_currentCooldownTime = _cooldownStart;
		}
		else
		{
			InformationManager.DisplayMessage(new InformationMessage("Your ability is still on cooldown for " + (float)MBMath.Round((_maxCooldownTime - _currentCooldownTime) * 100.0) / 100f + " second(s)!"));
		}
	}

	public static void LoadConfig()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!File.Exists(_configPath))
		{
			return;
		}
		try
		{
			_boundInput = (InputKey)JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).YellKey;
			_abilityRadius = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).AbilityRadius;
			_maxCooldownTime = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).AbilityCooldown;
			_positiveMoraleChange = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).AlliedMoraleGain;
			_negativeMoraleChange = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).EnemyFleeMoraleThreshold;
			_minResponseDelay = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).MinResponseDelay;
			_maxResponseDelay = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).MaxResponseDelay;
			_percentChanceToFlee = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).EnemyChanceToFlee / 100f;
			_baseLeadershipExpPerAlly = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).BaseLeadershipExpPerAlly;
			_baseRogueryExpPerEnemy = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).BaseRogueryExpPerEnemy;
			_radiusIncreasePerLevelOfLeadership = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).RadiusIncreasePerLevel;
			_cooldownDecreasePerLevelOfLeadership = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).CooldownDecreasePerLevel;
			_moraleGainIncreasePerLevelOfLeadership = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).MoraleGainIncreasePerLevel;
			_chanceToFleeIncreasePerLevelOfRoguery = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).ChanceToFleeIncreasePerLevel;
			_fleeingEnemiesReturn = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).FleeingEnemiesReturn;
			_showSphereIndicators = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).ShowSphereIndicators;
			_showDetailedMessage = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).ShowDetailedMessage;
			_enableRandomCheerDelay = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).EnableRandomCheerDelay;
			_enableCheerAnimation = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).EnableCheerAnimation;
			_enableEnemyReaction = JsonConvert.DeserializeObject<YellToInspireConfig>(File.ReadAllText(_configPath)).EnableEnemyReaction;
			_baseRadius = _abilityRadius;
			_baseCooldownTime = _maxCooldownTime;
			_basePositiveMoraleChange = _positiveMoraleChange;
			_baseChanceToFlee = _percentChanceToFlee;
		}
		catch
		{
		}
	}

	public static void LoadLanguage()
	{
		if (!File.Exists(_languagePath))
		{
			return;
		}
		try
		{
			_useCustomPhrases = JsonConvert.DeserializeObject<YellToInspireLanguage>(File.ReadAllText(_languagePath)).UseCustomFlavorText;
			_customPhrases = JsonConvert.DeserializeObject<YellToInspireLanguage>(File.ReadAllText(_languagePath)).CustomFlavorText;
			_useCustomDetailedText = JsonConvert.DeserializeObject<YellToInspireLanguage>(File.ReadAllText(_languagePath)).UseCustomDetailedText;
			_alliesInspiredCustomDetailedText = JsonConvert.DeserializeObject<YellToInspireLanguage>(File.ReadAllText(_languagePath)).AlliesInspiredCustomDetailedText;
			_alliesRestoredCustomDetailedText = JsonConvert.DeserializeObject<YellToInspireLanguage>(File.ReadAllText(_languagePath)).AlliesRestoredCustomDetailedText;
			_enemiesScaredCustomDetailedText = JsonConvert.DeserializeObject<YellToInspireLanguage>(File.ReadAllText(_languagePath)).EnemiesScaredCustomDetailedText;
			_enemiesFledCustomDetailedText = JsonConvert.DeserializeObject<YellToInspireLanguage>(File.ReadAllText(_languagePath)).EnemiesFledCustomDetailedText;
			_enemiesRestoredCustomDetailedText = JsonConvert.DeserializeObject<YellToInspireLanguage>(File.ReadAllText(_languagePath)).EnemiesRestoredCustomDetailedText;
		}
		catch
		{
		}
	}
}
