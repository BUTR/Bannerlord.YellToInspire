using Bannerlord.YellToInspire.CampaignBehaviors;

using System;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

#if e162 || e163 || e164 || e165 || e170 || e171
using PerkObject = TaleWorlds.CampaignSystem.PerkObject;
#elif e172
using PerkObject = TaleWorlds.CampaignSystem.CharacterDevelopment.PerkObject;
#endif

namespace Bannerlord.YellToInspire.Handlers
{
    internal sealed class CampaignHandler : MBSubModuleBase
    {
        private static readonly string InspireName = "{=X0lXr5OeSQ}Inspire";
        private static readonly string InspireResolveName = "{=fTNsdClmjc}Inspire Resolve";

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
        private static readonly TextObject _inspireResolveDescription = new(@"{=cGR5dN440y}
Friendly fleeing units under the effect of Inspire regain their resolve and return to the fight!");
        //        private static readonly TextObject _inspireHasteDescription = new(@"{=NmyOWk1YDa}
        //Friendly units under the effect of Inspire gain a short temporary speed boost!");
        //        private static readonly TextObject _inspireFortitudeDescription = new(@"{=YwMTY7RbUv}
        //Friendly units under the effect of Inspire gain a small temporary health boost!");

        //private double _campaignTime;
        private bool _isDirty;


        public event Action? GameEnd;

        protected override void OnSubModuleLoad()
        {
            if (Settings.Instance is not { } settings) return;

            settings.PropertyChanged += (_, _) => _isDirty = true;

            base.OnSubModuleLoad();
        }

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            if (starterObject is CampaignGameStarter campaignStarter)
            {
                CampaignEvents.HeroGainedSkill.AddNonSerializedListener(this, OnHeroGainedSkill);
                campaignStarter.AddBehavior(new InspireCampaignBehaviour(this));
            }

            base.InitializeGameStarter(game, starterObject);
        }
        private void OnHeroGainedSkill(Hero hero, SkillObject skill, bool hasNewPerk, int change, bool shouldNotify)
        {
            if (hero != Hero.MainHero) return;

            if (skill != DefaultSkills.Leadership && skill != DefaultSkills.Roguery) return;

            _isDirty = true;
        }

        public override void RegisterSubModuleObjects(bool isSavedCampaign)
        {
            PerkObject CreatePerk(string stringId) => Game.Current.ObjectManager.RegisterPresumedObject(new PerkObject(stringId));

            Perks.InspireBasic = CreatePerk(InspireName);
            Perks.InspireResolve = CreatePerk(InspireResolveName);

            Perks.InspireBasic.Initialize(InspireName, SetVariables(_inspireBasicDescription).ToString(),
                Skills.Leadership, 5, null, SkillEffect.PerkRole.PartyLeader, 10f,
                SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add);
            Perks.InspireResolve.Initialize(InspireResolveName, _inspireResolveDescription.ToString(),
                Skills.Leadership, 35, null, SkillEffect.PerkRole.PartyLeader, 15f,
                SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor);

            base.RegisterSubModuleObjects(isSavedCampaign);
        }

        public override void OnGameEnd(Game game)
        {
            GameEnd?.Invoke();

            base.OnGameEnd(game);
        }

        protected override void OnApplicationTick(float dt)
        {
            if (Campaign.Current is null) return;

            if (_isDirty)
            {
                _isDirty = false;
                Perks.InspireBasic?.Initialize(new TextObject(InspireName), SetVariables(_inspireBasicDescription));
            }

            base.OnApplicationTick(dt);
        }

        private static TextObject SetVariables(TextObject textObject)
        {
            if (Settings.Instance is not { } settings) return textObject;
            var @char = CharacterObject.PlayerCharacter;

            return textObject
                .SetTextVariable("NEWLINE", "\n")
                .SetTextVariable("RADIUS", settings.AbilityRadius(@char))
                .SetTextVariable("MORALE", settings.AlliedMoraleGain(@char))
                .SetTextVariable("FLEE", settings.EnemyChanceToFlee(@char))
                .SetTextVariable("COOLDOWN", settings.AbilityCooldown(@char));
        }
    }
}