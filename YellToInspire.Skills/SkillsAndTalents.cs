using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

namespace YellToInspire.Skills;

public class SkillsAndTalents
{
	private Game _game;

	public List<PerkObject> perkObjects;

	public static PerkObject InspireBasic;

	public static PerkObject InspireResolve;

	public static PerkObject InspireHaste;

	public static PerkObject InspireFortitude;

	public static TextObject inspireBasicDescription = new TextObject("Press {input} to belt out a mighty yell that inspires allies {newline}and instills fear in the hearts of your enemies. {newline}Nearby allies are given a small morale boost, and {newline}nearby enemies with low morale have a chance of fleeing! {newline}Radius, allied morale gain, and cooldown scale with Leadership. {newline}Enemy flee chance scales with Roguery. {newline} {newline}  Current radius size: {radius} meters {newline}  Allied morale gain: +{morale} morale {newline}  Enemy flee chance: {flee}% {newline}  Cooldown: {cooldown} seconds", (Dictionary<string, TextObject>)null);

	public static TextObject inspireTenacityDescription = new TextObject("Friendly fleeing units under the effect of Inspire regain their resolve and return to the fight!", (Dictionary<string, TextObject>)null);

	public static TextObject inspireHasteDescription = new TextObject("Friendly units under the effect of Inspire gain a short temporary speed boost!", (Dictionary<string, TextObject>)null);

	public static TextObject inspireFortitudeDescription = new TextObject("Friendly units under the effect of Inspire gain a small temporary health boost!", (Dictionary<string, TextObject>)null);

	public SkillsAndTalents(Game game)
	{
		_game = game;
		perkObjects = new List<PerkObject>();
		AddPerks();
	}

	private PerkObject CreatePerk(string stringId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		PerkObject val = new PerkObject(stringId);
		perkObjects.Add(val);
		return _game.get_ObjectManager().RegisterPresumedObject<PerkObject>(val);
	}

	private void AddPerks()
	{
		InspireBasic = CreatePerk("Inspire");
		InspireResolve = CreatePerk("Inspire Resolve");
		InitializePerks();
	}

	public void InitializePerks()
	{
		InitializeTextVariables();
		InspireBasic.Initialize("Inspire", ((object)inspireBasicDescription).ToString(), DefaultSkills.get_Leadership(), 5, (PerkObject)null, (PerkRole)5, 10f, (PerkRole)0, 0f, (EffectIncrementType)0, "");
		InspireResolve.Initialize("Inspire Resolve", ((object)inspireTenacityDescription).ToString(), DefaultSkills.get_Leadership(), 35, (PerkObject)null, (PerkRole)5, 15f, (PerkRole)0, 0f, (EffectIncrementType)1, "");
	}

	public static void ReinitializePerks()
	{
		InitializeTextVariables();
		if (InspireBasic != null)
		{
			InspireBasic.Initialize("Inspire", ((object)inspireBasicDescription).ToString(), DefaultSkills.get_Leadership(), 5, (PerkObject)null, (PerkRole)5, 10f, (PerkRole)0, 0f, (EffectIncrementType)1, "");
		}
	}

	private static void InitializeTextVariables()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		inspireBasicDescription.SetTextVariable("newline", "\n");
		inspireBasicDescription.SetTextVariable("input", Enum.GetName(typeof(InputKey), InspireBehaviour._boundInput));
		inspireBasicDescription.SetTextVariable("radius", InspireBehaviour._abilityRadius);
		inspireBasicDescription.SetTextVariable("morale", InspireBehaviour._positiveMoraleChange);
		inspireBasicDescription.SetTextVariable("flee", (float)Math.Round(InspireBehaviour._percentChanceToFlee * 100f, 4));
		inspireBasicDescription.SetTextVariable("cooldown", InspireBehaviour._maxCooldownTime.ToString());
	}
}
