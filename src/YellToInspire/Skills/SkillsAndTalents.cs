using System;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace YellToInspire.Skills
{
    public sealed class SkillsAndTalents : IDisposable
    {
        private static readonly string InspireId = "Inspire";
        private static readonly string InspireResolveId = "Inspire Resolve";

        private static readonly TextObject _inspireBasicDescription = new(@"{=c2MjU4ZqPQ}
Press {INPUT} to belt out a mighty yell that inspires allies{NEWLINE}
and instills fear in the hearts of your enemies.{NEWLINE}
Nearby allies are given a small morale boost, and{NEWLINE}
nearby enemies with low morale have a chance of fleeing!{NEWLINE}
Radius, allied morale gain, and cooldown scale with Leadership.{NEWLINE}
Enemy flee chance scales with Roguery.{NEWLINE}{NEWLINE}
Current radius size: {RADIUS} meters{NEWLINE}
Allied morale gain: +{MORALE} morale {NEWLINE}
Enemy flee chance: {FLEE}% {NEWLINE}
Cooldown: {COOLDOWN} seconds");
        private static readonly TextObject _inspireTenacityDescription = new(@"{=cGR5dN440y}
Friendly fleeing units under the effect of Inspire regain their resolve and return to the fight!");
        private static readonly TextObject _inspireHasteDescription = new(@"{=NmyOWk1YDa}
Friendly units under the effect of Inspire gain a short temporary speed boost!");
        private static readonly TextObject _inspireFortitudeDescription = new(@"{=YwMTY7RbUv}
Friendly units under the effect of Inspire gain a small temporary health boost!");

        public static PerkObject InspireBasic { get; private set; }
        public static PerkObject InspireResolve { get; private set; }
        public static PerkObject InspireHaste { get; private set; }
        public static PerkObject InspireFortitude { get; private set; }

        private readonly Game _game;
        private float _deltaSum;

        public SkillsAndTalents(Game game)
        {
            _game = game;

            InspireBasic = CreatePerk(InspireId);
            InspireResolve = CreatePerk(InspireResolveId);
            InitializePerks();
        }

        public void Update(float dt)
        {
            // TODO:
            _deltaSum += dt;
            if (_deltaSum < 500f) return;
            _deltaSum = 0f;

            InspireBasic.Initialize(InspireId, SetVariables(_inspireBasicDescription).ToString(), DefaultSkills.Leadership, 5, null, SkillEffect.PerkRole.PartyLeader, 10f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, "");
        }

        private PerkObject CreatePerk(string stringId) => _game.ObjectManager.RegisterPresumedObject(new PerkObject(stringId));

        private void InitializePerks()
        {
            InspireBasic.Initialize(InspireId, SetVariables(_inspireBasicDescription).ToString(), DefaultSkills.Leadership, 5, null, SkillEffect.PerkRole.PartyLeader, 10f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add, "");
            InspireResolve.Initialize(InspireResolveId, _inspireTenacityDescription.ToString(), DefaultSkills.Leadership, 35, null, SkillEffect.PerkRole.PartyLeader, 15f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor, "");
        }

        private static TextObject SetVariables(TextObject textObject)
        {
            if (_inspireBasicDescription is null) return textObject;
            if (Settings.Instance is not { } settings) return textObject;

            return textObject
                .SetTextVariable("NEWLINE", "\n")
                .SetTextVariable("RADIUS", settings.AbilityRadius(Hero.MainHero))
                .SetTextVariable("MORALE", settings.AlliedMoraleGain(Hero.MainHero))
                .SetTextVariable("FLEE", settings.EnemyChanceToFlee(Hero.MainHero))
                .SetTextVariable("COOLDOWN", settings.AbilityCooldown(Hero.MainHero));
        }

        public void Dispose()
        {

        }
    }
}