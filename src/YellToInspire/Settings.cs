using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Settings.Base.Global;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace YellToInspire
{
    public class Settings : AttributeGlobalSettings<Settings>
    {
        public override string Id => "YellToInspire_v1";
        public override string DisplayName => new TextObject("{=V8iSlcN2mq}Yell To Inspire {VERSION}", new()
        {
            { "VERSION", typeof(Settings).Assembly.GetName().Version?.ToString(3) ?? "ERROR" }
        }).ToString();

        public float AbilityRadius(Hero? hero) => BaseAbilityRadius + (hero?.GetSkillValue(DefaultSkills.Leadership) ?? 0 * AbilityRadiusIncreasePerLevel);
        public float AbilityCooldown(Hero? hero) => BaseAbilityCooldown + (hero?.GetSkillValue(DefaultSkills.Leadership) ?? 0 * AbilityCooldownDecreasePerLevel);
        public float AlliedMoraleGain(Hero? hero) => BaseAlliedMoraleGain + (hero?.GetSkillValue(DefaultSkills.Leadership) ?? 0 * AlliedMoraleGainIncreasePerLevel);
        public float EnemyChanceToFlee(Hero? hero) => BaseEnemyChanceToFlee + (hero?.GetSkillValue(DefaultSkills.Roguery) ?? 0 * ChanceToFleeIncreasePerLevel / 100f);


        [SettingPropertyFloatingInteger("Base Ability Radius", 0f, 100f, "#0 units", Order = 0, RequireRestart = false, HintText = "")]
        public float BaseAbilityRadius { get; set; } = 10f;

        [SettingPropertyFloatingInteger("Ability Radius Increase per Level", 0f, 10f, "0.00 units", Order = 1, RequireRestart = false, HintText = "Depends on Leadership.")]
        public float AbilityRadiusIncreasePerLevel { get; set; } = 0.1f;


        [SettingPropertyFloatingInteger("Base Ability Cooldown", 0f, 300f, "#0 sec", Order = 2, RequireRestart = false, HintText = "")]
        public float BaseAbilityCooldown { get; set; } = 10.0f;

        [SettingPropertyFloatingInteger("Ability Cooldown Decrease per Level", 0f, 200f, "#0 ms", Order = 3, RequireRestart = false, HintText = "Depends on Leadership.")]
        public float AbilityCooldownDecreasePerLevel { get; set; } = 150f;


        [SettingPropertyFloatingInteger("Base Allied Morale Gain", 0f, 100f, "#0", Order = 4, RequireRestart = false, HintText = "")]
        public float BaseAlliedMoraleGain { get; set; } = 0f;

        [SettingPropertyFloatingInteger("Allied Morale Gain Increase per Level", 0f, 1f, "0.00", Order = 5, RequireRestart = false, HintText = "Depends on Leadership.")]
        public float AlliedMoraleGainIncreasePerLevel { get; set; } = 0.05f;


        [SettingPropertyFloatingInteger("Base Enemy Chance to Flee", 0f, 1f, "0.00%", Order = 6, RequireRestart = false, HintText = "")]
        public float BaseEnemyChanceToFlee { get; set; } = 0.5f;

        [SettingPropertyFloatingInteger("Chance to Flee Increase per Level", 0f, 1f, "#0%", Order = 7, RequireRestart = false, HintText = "Depends on Roguery. 1/100 of a %")]
        public float ChanceToFleeIncreasePerLevel { get; set; } = 0.15f;


        [SettingPropertyFloatingInteger("Enemy Flee Morale Threshold", 0f, 100f, "#0", Order = 8, RequireRestart = false, HintText = "")]
        public float EnemyFleeMoraleThreshold { get; set; } = 15f;


        [SettingPropertyFloatingInteger("Min Response Delay", 0f, 10f, "0.00 sec", Order = 9, RequireRestart = false, HintText = "")]
        public float MinResponseDelay { get; set; } = 0.7f;

        [SettingPropertyFloatingInteger("Max Response Delay", 0f, 10f, "0.00 sec", Order = 10, RequireRestart = false, HintText = "")]
        public float MaxResponseDelay { get; set; } = 2.2f;


        [SettingPropertyFloatingInteger("Base Leadership Exp per Ally", 0f, 100f, "#0 exp", Order = 11, RequireRestart = false, HintText = "")]
        public float BaseLeadershipExpPerAlly { get; set; } = 1f;

        [SettingPropertyFloatingInteger("Base Roguery Exp per Enemy", 0f, 100f, "#0 exp", Order = 12, RequireRestart = false, HintText = "")]
        public float BaseRogueryExpPerEnemy { get; set; } = 1f;


        [SettingPropertyBool("Fleeing Enemies Return", Order = 13, RequireRestart = false, HintText = "")]
        public bool FleeingEnemiesReturn { get; set; } = false;


        [SettingPropertyBool("Show Sphere Indicators", Order = 14, RequireRestart = false, HintText = "")]
        public bool ShowSphereIndicators { get; set; } = true;


        [SettingPropertyBool("Show Detailed Message", Order = 15, RequireRestart = false, HintText = "")]
        public bool ShowDetailedMessage { get; set; } = true;

        [SettingPropertyBool("Show Detailed Message Text", Order = 16, RequireRestart = false, HintText = "")]
        public bool ShowDetailedMessageText { get; set; } = true;


        [SettingPropertyBool("Enable Cheer Random Delay", Order = 17, RequireRestart = false, HintText = "")]
        public bool EnableCheerRandomDelay { get; set; } = true;

        [SettingPropertyBool("Enable Cheer Animation", Order = 18, RequireRestart = false, HintText = "")]
        public bool EnableCheerAnimation { get; set; } = true;


        [SettingPropertyBool("Enable Enemy Reaction", Order = 19, RequireRestart = false, HintText = "")]
        public bool EnableEnemyReaction { get; set; } = true;
    }
}