using Bannerlord.BUTR.Shared.Helpers;
using Bannerlord.ButterLib.HotKeys;
using Bannerlord.YellToInspire.Data;
using Bannerlord.YellToInspire.HotKeys;
using Bannerlord.YellToInspire.MissionBehaviors;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire
{
    internal sealed class SubModule : MBSubModuleBase
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


        private bool _isInitialized;

        protected override void OnSubModuleLoad()
        {
            if (Settings.Instance is { } settings)
            {
                settings.PropertyChanged += (_, _) => RefreshPerks();
            }

            base.OnSubModuleLoad();
        }


        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;

                if (HotKeyManager.Create(ModuleInfoHelper.GetModuleByType(typeof(SubModule))!.Id) is { } hkm)
                {
                    hkm.Add<YellToInspireHotKey>();
                    hkm.Build();
                }
            }
        }

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            void OnHeroGainedSkill(Hero hero, SkillObject skill, int change, bool shouldNotify)
            {
                if (hero != Hero.MainHero) return;

                if (skill != DefaultSkills.Leadership && skill != DefaultSkills.Roguery) return;

                RefreshPerks();
            }

            if (starterObject is CampaignGameStarter)
            {
                CampaignEvents.HeroGainedSkill.AddNonSerializedListener(this, OnHeroGainedSkill);
            }

            base.InitializeGameStarter(game, starterObject);
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

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            if (mission.CombatType != Mission.MissionCombatType.NoCombat && Settings.Instance is { } settings)
            {
                if (settings.EnableInAnyMission || (mission.AttackerTeam is not null && mission.DefenderTeam is not null))
                {
                    mission.AddMissionBehavior(new InspireComponentTickBehaviour());
                    mission.AddMissionBehavior(settings.GameplayType.SelectedValue.Type switch
                    {
                        GameplaySystem.Killing => new InspireGameplayKillingBehaviour(),
                        GameplaySystem.Cooldown => new InspireGameplayCooldownBehaviour(),
                    });
                }
            }

            base.OnMissionBehaviorInitialize(mission);
        }

        private static void RefreshPerks()
        {
            Perks.InspireBasic?.Initialize(new TextObject(InspireName), SetVariables(_inspireBasicDescription));
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