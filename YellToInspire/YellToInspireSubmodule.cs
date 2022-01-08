using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using YellToInspire.Skills;

namespace YellToInspire;

public class YellToInspireSubmodule : MBSubModuleBase
{
	public static bool _newCampaign = true;

	public bool IsNew = true;

	public static bool _inMission = false;

	public static int _agentCount = 0;

	public static bool _updatePerkOnLoad = false;

	private readonly ActionIndexCache[] _cheerActions = (ActionIndexCache[])(object)new ActionIndexCache[5]
	{
		ActionIndexCache.Create("act_command"),
		ActionIndexCache.Create("act_command"),
		ActionIndexCache.Create("act_command"),
		ActionIndexCache.Create("act_command"),
		ActionIndexCache.Create("act_command_follow")
	};

	private static bool _unitsResponded = true;

	private static List<Agent> _affectedAgents = new List<Agent>();

	public static List<CheeringAgent> _cheeringAgents = new List<CheeringAgent>();

	private static float _minResponseDelay = 0.7f;

	private static float _maxResponseDelay = 2.2f;

	private Hero _mainHero;

	private int _currentLeadership = 0;

	private int _currentRoguery = 0;

	public YellToInspireSkillsAndTalents SkillsAndTalents;

	protected override void OnSubModuleLoad()
	{
		((MBSubModuleBase)this).OnSubModuleLoad();
		YellToInspireBehavior.LoadConfig();
		YellToInspireBehavior.LoadLanguage();
	}

	protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
	{
		((MBSubModuleBase)this).OnGameStart(game, gameStarterObject);
		CampaignGameStarter val = (CampaignGameStarter)(object)((gameStarterObject is CampaignGameStarter) ? gameStarterObject : null);
		if (val != null)
		{
			val.AddBehavior((CampaignBehaviorBase)(object)new YellToInspireCampaignBehavior());
		}
	}

	public override bool DoLoading(Game game)
	{
		YellToInspireBehavior.LoadConfig();
		if (CustomGame.get_Current() == null)
		{
			LoadPerks(game);
		}
		return true;
	}

	public void LoadPerks(Game game)
	{
		SkillsAndTalents = new YellToInspireSkillsAndTalents(game);
		Campaign.get_Current().set_PerkList(MBObjectManager.get_Instance().GetObjectTypeList<PerkObject>());
		if (IsNew)
		{
			return;
		}
		foreach (Hero item in Hero.get_All())
		{
			item.get_HeroDeveloper().SetInitialSkillLevel(DefaultSkills.get_Leadership(), item.GetSkillValue(DefaultSkills.get_Leadership()));
		}
	}

	public override void OnCampaignStart(Game game, object starterObject)
	{
		IsNew = true;
	}

	public override void OnGameEnd(Game game)
	{
		((MBSubModuleBase)this).OnGameEnd(game);
		_newCampaign = true;
		_updatePerkOnLoad = false;
	}

	protected override void OnApplicationTick(float dt)
	{
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		((MBSubModuleBase)this).OnApplicationTick(dt);
		YellToInspireBehavior.currentMission = Mission.get_Current();
		if (Campaign.get_Current() != null)
		{
			if (_newCampaign && CharacterObject.get_PlayerCharacter() != null)
			{
				_newCampaign = false;
			}
			if (!_newCampaign)
			{
				_mainHero = Hero.get_MainHero();
				UpdatePerks();
			}
		}
		if (YellToInspireBehavior.currentMission == null)
		{
			_inMission = false;
			YellToInspireBehavior._abilityReady = true;
			YellToInspireBehavior._currentCooldownTime = 0.0;
			_cheeringAgents.Clear();
			_affectedAgents.Clear();
			MBDebug.ClearRenderObjects();
			return;
		}
		if (Agent.get_Main() != null && YellToInspireSphereIndicator.DisplaySphereIndicators)
		{
			EnableIndicators();
		}
		if (_cheeringAgents.Count > 0)
		{
			StopCheeringAgentsAfterDelay();
		}
		if (!YellToInspireBehavior._abilityReady)
		{
			MissionTime now = MissionTime.get_Now();
			YellToInspireBehavior._currentCooldownTime = ((MissionTime)(ref now)).get_ToSeconds() - YellToInspireBehavior._cooldownStart;
			if (YellToInspireBehavior._currentCooldownTime >= YellToInspireBehavior._maxCooldownTime - (YellToInspireBehavior._maxCooldownTime - 0.5))
			{
				DisableIndicators();
			}
			if (YellToInspireBehavior._currentCooldownTime >= YellToInspireBehavior._maxCooldownTime)
			{
				ResetCooldownStatus();
			}
		}
		if (!_unitsResponded)
		{
			if (_affectedAgents.Count == 0 || _affectedAgents == null)
			{
				_unitsResponded = true;
			}
			else
			{
				DelayAndReact();
			}
		}
		YellToInspireBehavior.mainAgent = YellToInspireBehavior.currentMission.get_MainAgent();
		if (YellToInspireBehavior.mainAgent == null || !YellToInspireBehavior.mainAgent.get_IsPlayerControlled())
		{
			return;
		}
		if (CustomGame.get_Current() == null && _mainHero.GetSkillValue(DefaultSkills.get_Leadership()) >= 5 && !_mainHero.GetPerkValue(YellToInspireSkillsAndTalents.InspireBasic))
		{
			_mainHero.SetPerkValue(YellToInspireSkillsAndTalents.InspireBasic, true);
		}
		if (!Input.IsPressed(YellToInspireBehavior._boundInput) || !YellToInspireBehavior.mainAgent.IsActive())
		{
			return;
		}
		if (CustomGame.get_Current() == null)
		{
			if (_mainHero.GetPerkValue(YellToInspireSkillsAndTalents.InspireBasic))
			{
				YellToInspireBehavior.InspireAura();
			}
		}
		else
		{
			YellToInspireBehavior.InspireAura();
		}
	}

	private void EnableIndicators()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		YellToInspireSphereIndicator.SphereCurrentRadius = MBMath.Lerp(YellToInspireSphereIndicator.SphereCurrentRadius, YellToInspireBehavior._abilityRadius, 0.1f, 1E-05f);
		Vec3 abilityUsedPosition = YellToInspireSphereIndicator.AbilityUsedPosition;
		float x = ((Vec3)(ref abilityUsedPosition)).get_X();
		abilityUsedPosition = YellToInspireSphereIndicator.AbilityUsedPosition;
		Vec3 val = default(Vec3);
		((Vec3)(ref val))._002Ector(x, ((Vec3)(ref abilityUsedPosition)).get_Y(), Mission.get_Current().get_Scene().GetGroundHeightAtPosition(YellToInspireSphereIndicator.AbilityUsedPosition, (BodyFlags)6402441, true), -1f);
		MBDebug.RenderDebugSphere(val, YellToInspireSphereIndicator.SphereCurrentRadius, uint.MaxValue, true, 0.05f);
	}

	private void DisableIndicators()
	{
		YellToInspireSphereIndicator.DisplaySphereIndicators = false;
	}

	private void ResetCooldownStatus()
	{
		YellToInspireBehavior._abilityReady = true;
	}

	private void UpdatePerks()
	{
		_currentLeadership = _mainHero.GetSkillValue(DefaultSkills.get_Leadership());
		_currentRoguery = _mainHero.GetSkillValue(DefaultSkills.get_Roguery());
		YellToInspireBehavior._abilityRadius = YellToInspireBehavior._baseRadius + (float)_mainHero.GetSkillValue(DefaultSkills.get_Leadership()) * YellToInspireBehavior._radiusIncreasePerLevelOfLeadership;
		YellToInspireBehavior._maxCooldownTime = Math.Round(YellToInspireBehavior._baseCooldownTime - (double)((float)_mainHero.GetSkillValue(DefaultSkills.get_Leadership()) * YellToInspireBehavior._cooldownDecreasePerLevelOfLeadership), 2);
		YellToInspireBehavior._positiveMoraleChange = (float)Math.Round(YellToInspireBehavior._basePositiveMoraleChange + (float)_mainHero.GetSkillValue(DefaultSkills.get_Leadership()) * YellToInspireBehavior._moraleGainIncreasePerLevelOfLeadership, 2);
		YellToInspireBehavior._percentChanceToFlee = (float)Math.Round(YellToInspireBehavior._baseChanceToFlee + (float)_mainHero.GetSkillValue(DefaultSkills.get_Roguery()) * YellToInspireBehavior._chanceToFleeIncreasePerLevelOfRoguery, 4);
		if (YellToInspireBehavior._percentChanceToFlee > 1f)
		{
			YellToInspireBehavior._percentChanceToFlee = 1f;
		}
		YellToInspireSkillsAndTalents.ReinitializePerks();
	}

	private void DelayAndReact()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		foreach (Agent item in _affectedAgents.ToList())
		{
			if (item == null || !item.IsActive())
			{
				_affectedAgents.Remove(item);
				continue;
			}
			float num = ((!YellToInspireBehavior._enableRandomCheerDelay) ? 1f : MBRandom.RandomFloatRanged(_minResponseDelay, _maxResponseDelay));
			MissionTime now = MissionTime.get_Now();
			if (!(((MissionTime)(ref now)).get_ToSeconds() >= YellToInspireBehavior._cooldownStart + (double)num))
			{
				continue;
			}
			if (item.get_Team() == Agent.get_Main().get_Team())
			{
				item.SetWantsToYell();
				Mission current2 = Mission.get_Current();
				Vec3 position = item.get_Position();
				if (current2.GetNearbyEnemyAgents(((Vec3)(ref position)).get_AsVec2(), 8f, item.get_Team()).Count() <= 0 && YellToInspireBehavior._enableCheerAnimation)
				{
					item.SetActionChannel(1, _cheerActions[MBRandom.RandomInt(_cheerActions.Length)], false, 0uL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					now = MissionTime.get_Now();
					AddToCheeringList(new CheeringAgent(item, ((MissionTime)(ref now)).get_ToSeconds(), 1.5));
				}
			}
			else if (YellToInspireBehavior._enableEnemyReaction)
			{
				item.MakeVoice(VoiceType.Fear, (CombatVoiceNetworkPredictionType)2);
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
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		foreach (CheeringAgent item in _cheeringAgents.ToList())
		{
			if (Mission.get_Current() == null)
			{
				_cheeringAgents.Clear();
				break;
			}
			if (item._agent != null)
			{
				item._agent.get_Position();
				if (0 == 0)
				{
					MissionTime now = MissionTime.get_Now();
					if (!(((MissionTime)(ref now)).get_ToSeconds() >= item._initialTime + item._timeDelay))
					{
						Mission current2 = Mission.get_Current();
						Vec3 position = item._agent.get_Position();
						if (current2.GetNearbyEnemyAgents(((Vec3)(ref position)).get_AsVec2(), 8f, item._agent.get_Team()).Count() <= 0)
						{
							continue;
						}
					}
					item._agent.SetActionChannel(1, ActionIndexCache.get_act_none(), true, 0uL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					_cheeringAgents.Remove(item);
					continue;
				}
			}
			_cheeringAgents.Remove(item);
		}
	}

	public static void DelayedAgentReactions(List<Agent> agents, float timeInSecondsMin, float timeInSecondsMax)
	{
		_affectedAgents.Clear();
		_minResponseDelay = timeInSecondsMin;
		_maxResponseDelay = timeInSecondsMax;
		if (agents.Count == 0)
		{
			return;
		}
		foreach (Agent item in agents.ToList())
		{
			_affectedAgents.Add(item);
		}
		_unitsResponded = false;
	}

	public static void AddToCheeringList(CheeringAgent cheeringAgent)
	{
		_cheeringAgents.Add(cheeringAgent);
	}
}
