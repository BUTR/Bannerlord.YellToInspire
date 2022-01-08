using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

namespace YellToInspire.Skills
{
    public class SkillsAndTalents
    {
        private static readonly TextObject inspireBasicDescription = new("Press {input} to belt out a mighty yell that inspires allies {newline}and instills fear in the hearts of your enemies. {newline}Nearby allies are given a small morale boost, and {newline}nearby enemies with low morale have a chance of fleeing! {newline}Radius, allied morale gain, and cooldown scale with Leadership. {newline}Enemy flee chance scales with Roguery. {newline} {newline}  Current radius size: {radius} meters {newline}  Allied morale gain: +{morale} morale {newline}  Enemy flee chance: {flee}% {newline}  Cooldown: {cooldown} seconds");
        private static readonly TextObject inspireTenacityDescription = new("Friendly fleeing units under the effect of Inspire regain their resolve and return to the fight!");
        private static readonly TextObject inspireHasteDescription = new("Friendly units under the effect of Inspire gain a short temporary speed boost!");
        private static readonly TextObject inspireFortitudeDescription = new("Friendly units under the effect of Inspire gain a small temporary health boost!");
        
        public static PerkObject InspireBasic;
        public static PerkObject InspireResolve;
        public static PerkObject InspireHaste;
        public static PerkObject InspireFortitude;

        private readonly Game _game;
        private List<PerkObject> perkObjects;

        public SkillsAndTalents(Game game)
        {
            _game = game;
            perkObjects = new List<PerkObject>();
            AddPerks();
        }

        private PerkObject CreatePerk(string stringId)
        {
            var val = new PerkObject(stringId);
            perkObjects.Add(val);
            return _game.ObjectManager.RegisterPresumedObject(val);
        }

        private void AddPerks()
        {
            InspireBasic = CreatePerk("Inspire");
            InspireResolve = CreatePerk("Inspire Resolve");
            InitializePerks();
        }

        private static void InitializePerks()
        {
            InitializeTextVariables();
            InspireBasic.Initialize("Inspire", inspireBasicDescription.ToString(), DefaultSkills.Leadership, 5, null, SkillEffect.PerkRole.PartyLeader, 10f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add, "");
            InspireResolve.Initialize("Inspire Resolve", inspireTenacityDescription.ToString(), DefaultSkills.Leadership, 35, null, SkillEffect.PerkRole.PartyLeader, 15f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, "");
        }

        public static void ReinitializePerks()
        {
            InitializeTextVariables();
            if (InspireBasic is not null)
            {
                InspireBasic.Initialize("Inspire", inspireBasicDescription.ToString(), DefaultSkills.Leadership, 5, null, SkillEffect.PerkRole.PartyLeader, 10f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, "");
            }
        }

        private static void InitializeTextVariables()
        {
            inspireBasicDescription.SetTextVariable("newline", "\n");
            inspireBasicDescription.SetTextVariable("input", Enum.GetName(typeof(InputKey), InspireBehaviour._boundInput));
            inspireBasicDescription.SetTextVariable("radius", InspireBehaviour._abilityRadius);
            inspireBasicDescription.SetTextVariable("morale", InspireBehaviour._positiveMoraleChange);
            inspireBasicDescription.SetTextVariable("flee", (float) Math.Round(InspireBehaviour._percentChanceToFlee * 100f, 4));
            inspireBasicDescription.SetTextVariable("cooldown", InspireBehaviour._maxCooldownTime.ToString());
        }
    }
}