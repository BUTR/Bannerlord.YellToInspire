using Bannerlord.BUTR.Shared.Helpers;
using Bannerlord.YellToInspire.Data;
using Bannerlord.YellToInspire.HotKeys;
using Bannerlord.YellToInspire.MissionBehaviors;

using BUTR.DependencyInjection;

using MCM.Abstractions.Settings.Providers;

using System;
using System.IO;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
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


        protected override void OnSubModuleLoad()
        {
            if (Settings.Instance is { } settings)
            {
                settings.PropertyChanged += (_, _) => RefreshPerks();
            }

            var moduleInfo = ModuleInfoHelper.GetModuleByType(typeof(SubModule));
            var path = Path.Combine(ModuleHelper.GetModuleFullPath(moduleInfo!.Id), "ModuleData", "module_strings.xml");
            Module.CurrentModule.GlobalTextManager.LoadGameTexts(path);
            HotKeyManager.AddAuxiliaryCategory(new YellToInspireHotkeyCategory());

            base.OnSubModuleLoad();
        }

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            void OnHeroGainedSkill(Hero hero, SkillObject skill, bool hasNewPerk, int change, bool shouldNotify)
            {
                if (hero != Hero.MainHero) return;

                if (skill != DefaultSkills.Leadership && skill != DefaultSkills.Roguery) return;

                RefreshPerks();
            }

            if (starterObject is CampaignGameStarter campaignStarter)
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
                mission.AddMissionBehavior(new InspireHotKeyBehaviour());
                mission.AddMissionBehavior(new InspireComponentTickBehaviour());
                mission.AddMissionBehavior(settings.GameplayType.SelectedValue switch
                {
                    GameplayType.Killing => new InspireGameplayKillingBehaviour(),
                    GameplayType.Cooldown => new InspireGameplayCooldownBehaviour(),
                    _ => throw new ArgumentOutOfRangeException()
                });
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