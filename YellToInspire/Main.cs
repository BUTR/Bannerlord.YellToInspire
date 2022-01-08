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

namespace YellToInspire;

public class Main : MBSubModuleBase
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

	public SkillsAndTalents SkillsAndTalents;

	protected override void OnSubModuleLoad()
	{
		((MBSubModuleBase)this).OnSubModuleLoad();
		InspireBehaviour.LoadConfig();
		InspireBehaviour.LoadLanguage();
	}

	protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
	{
		((MBSubModuleBase)this).OnGameStart(game, gameStarterObject);
		CampaignGameStarter val = (CampaignGameStarter)(object)((gameStarterObject is CampaignGameStarter) ? gameStarterObject : null);
		if (val != null)
		{
			val.AddBehavior((CampaignBehaviorBase)(object)new InspireCampaignBehaviour());
		}
	}

	public override bool DoLoading(Game game)
	{
		InspireBehaviour.LoadConfig();
		if (CustomGame.get_Current() == null)
		{
			LoadPerks(game);
		}
		return true;
	}

	public void LoadPerks(Game game)
	{
		SkillsAndTalents = new SkillsAndTalents(game);
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
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		((MBSubModuleBase)this).OnApplicationTick(dt);
		InspireBehaviour.currentMission = Mission.get_Current();
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
		if (InspireBehaviour.currentMission == null)
		{
			_inMission = false;
			InspireBehaviour._abilityReady = true;
			InspireBehaviour._currentCooldownTime = 0.0;
			_cheeringAgents.Clear();
			_affectedAgents.Clear();
			MBDebug.ClearRenderObjects();
			return;
		}
		if (Agent.get_Main() != null && SphereIndicator.DisplaySphereIndicators)
		{
			EnableIndicators();
		}
		if (_cheeringAgents.Count > 0)
		{
			StopCheeringAgentsAfterDelay();
		}
		if (!InspireBehaviour._abilityReady)
		{
			MissionTime now = MissionTime.get_Now();
			InspireBehaviour._currentCooldownTime = ((MissionTime)(ref now)).get_ToSeconds() - InspireBehaviour._cooldownStart;
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
			if (_affectedAgents.Count == 0 || _affectedAgents == null)
			{
				_unitsResponded = true;
			}
			else
			{
				DelayAndReact();
			}
		}
		InspireBehaviour.mainAgent = InspireBehaviour.currentMission.get_MainAgent();
		if (InspireBehaviour.mainAgent == null || !InspireBehaviour.mainAgent.get_IsPlayerControlled())
		{
			return;
		}
		if (CustomGame.get_Current() == null && _mainHero.GetSkillValue(DefaultSkills.get_Leadership()) >= 5 && !_mainHero.GetPerkValue(SkillsAndTalents.InspireBasic))
		{
			_mainHero.SetPerkValue(SkillsAndTalents.InspireBasic, true);
		}
		if (!Input.IsPressed(InspireBehaviour._boundInput) || !InspireBehaviour.mainAgent.IsActive())
		{
			return;
		}
		if (CustomGame.get_Current() == null)
		{
			if (_mainHero.GetPerkValue(SkillsAndTalents.InspireBasic))
			{
				InspireBehaviour.InspireAura();
			}
		}
		else
		{
			InspireBehaviour.InspireAura();
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
		SphereIndicator.SphereCurrentRadius = MBMath.Lerp(SphereIndicator.SphereCurrentRadius, InspireBehaviour._abilityRadius, 0.1f, 1E-05f);
		Vec3 abilityUsedPosition = SphereIndicator.AbilityUsedPosition;
		float x = ((Vec3)(ref abilityUsedPosition)).get_X();
		abilityUsedPosition = SphereIndicator.AbilityUsedPosition;
		Vec3 val = default(Vec3);
		((Vec3)(ref val))._002Ector(x, ((Vec3)(ref abilityUsedPosition)).get_Y(), Mission.get_Current().get_Scene().GetGroundHeightAtPosition(SphereIndicator.AbilityUsedPosition, (BodyFlags)6402441, true), -1f);
		MBDebug.RenderDebugSphere(val, SphereIndicator.SphereCurrentRadius, uint.MaxValue, true, 0.05f);
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
		_currentLeadership = _mainHero.GetSkillValue(DefaultSkills.get_Leadership());
		_currentRoguery = _mainHero.GetSkillValue(DefaultSkills.get_Roguery());
		InspireBehaviour._abilityRadius = InspireBehaviour._baseRadius + (float)_mainHero.GetSkillValue(DefaultSkills.get_Leadership()) * InspireBehaviour._radiusIncreasePerLevelOfLeadership;
		InspireBehaviour._maxCooldownTime = Math.Round(InspireBehaviour._baseCooldownTime - (double)((float)_mainHero.GetSkillValue(DefaultSkills.get_Leadership()) * InspireBehaviour._cooldownDecreasePerLevelOfLeadership), 2);
		InspireBehaviour._positiveMoraleChange = (float)Math.Round(InspireBehaviour._basePositiveMoraleChange + (float)_mainHero.GetSkillValue(DefaultSkills.get_Leadership()) * InspireBehaviour._moraleGainIncreasePerLevelOfLeadership, 2);
		InspireBehaviour._percentChanceToFlee = (float)Math.Round(InspireBehaviour._baseChanceToFlee + (float)_mainHero.GetSkillValue(DefaultSkills.get_Roguery()) * InspireBehaviour._chanceToFleeIncreasePerLevelOfRoguery, 4);
		if (InspireBehaviour._percentChanceToFlee > 1f)
		{
			InspireBehaviour._percentChanceToFlee = 1f;
		}
		SkillsAndTalents.ReinitializePerks();
	}

	private void DelayAndReact()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		foreach (Agent item in _affectedAgents.ToList())
		{
			if (item == null || !item.IsActive())
			{
				_affectedAgents.Remove(item);
				continue;
			}
			float num = ((!InspireBehaviour._enableRandomCheerDelay) ? 1f : MBRandom.RandomFloatRanged(_minResponseDelay, _maxResponseDelay));
			MissionTime now = MissionTime.get_Now();
			if (!(((MissionTime)(ref now)).get_ToSeconds() >= InspireBehaviour._cooldownStart + (double)num))
			{
				continue;
			}
			if (item.get_Team() == Agent.get_Main().get_Team())
			{
				item.SetWantsToYell();
				Mission current2 = Mission.get_Current();
				Vec3 position = item.get_Position();
				if (current2.GetNearbyEnemyAgents(((Vec3)(ref position)).get_AsVec2(), 8f, item.get_Team()).Count() <= 0 && InspireBehaviour._enableCheerAnimation)
				{
					item.SetActionChannel(1, _cheerActions[MBRandom.RandomInt(_cheerActions.Length)], false, 0uL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					now = MissionTime.get_Now();
					AddToCheeringList(new CheeringAgent(item, ((MissionTime)(ref now)).get_ToSeconds(), 1.5));
				}
			}
			else if (InspireBehaviour._enableEnemyReaction)
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
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		foreach (CheeringAgent item in _cheeringAgents.ToList())
		{
			if (Mission.get_Current() == null)
			{
				_cheeringAgents.Clear();
				break;
			}
			bool flag;
			if (item._agent != null)
			{
				Vec3 position = item._agent.get_Position();
				flag = false;
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				_cheeringAgents.Remove(item);
				continue;
			}
			MissionTime now = MissionTime.get_Now();
			if (!(((MissionTime)(ref now)).get_ToSeconds() >= item._initialTime + item._timeDelay))
			{
				Mission current2 = Mission.get_Current();
				Vec3 position2 = item._agent.get_Position();
				if (current2.GetNearbyEnemyAgents(((Vec3)(ref position2)).get_AsVec2(), 8f, item._agent.get_Team()).Count() <= 0)
				{
					continue;
				}
			}
			item._agent.SetActionChannel(1, ActionIndexCache.get_act_none(), true, 0uL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
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
