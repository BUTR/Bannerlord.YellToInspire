using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Settings.Base.Global;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace YellToInspire
{
    public sealed class Settings : AttributeGlobalSettings<Settings>
    {
        public override string Id => "YellToInspire_v1";
        public override string FolderName => "YellToInspire";
        public override string DisplayName => new TextObject("{=V8iSlcN2mq}Yell To Inspire {VERSION}", new()
        {
            { "VERSION", typeof(Settings).Assembly.GetName().Version?.ToString(3) ?? "ERROR" }
        }).ToString();

        public float AbilityRadius(BasicCharacterObject? character) => BaseAbilityRadius + (character?.GetSkillValue(Skills.Leadership) ?? 0 * AbilityRadiusIncreasePerLevel);
        public float AbilityCooldown(BasicCharacterObject? character) => BaseAbilityCooldown + (character?.GetSkillValue(Skills.Leadership) ?? 0 * AbilityCooldownDecreasePerLevel);
        public float AlliedMoraleGain(BasicCharacterObject? character) => BaseAlliedMoraleGain + (character?.GetSkillValue(Skills.Leadership) ?? 0 * AlliedMoraleGainIncreasePerLevel);
        public float EnemyChanceToFlee(BasicCharacterObject? character) => BaseEnemyChanceToFlee + (character?.GetSkillValue(Skills.Roguery) ?? 0 * ChanceToFleeIncreasePerLevel / 100f);


        [SettingPropertyFloatingInteger("{=OSbrbCqXUR}Base Ability Radius", 0f, 100f, "#0 units", Order = 0, RequireRestart = false, HintText = "")]
        public float BaseAbilityRadius { get; set; } = 10f;

        [SettingPropertyFloatingInteger("{=p2qRWZ7Ezg}Ability Radius Increase per Level", 0f, 10f, "0.00 units", Order = 1, RequireRestart = false, HintText = "{=f0iSyhJJ9H}Depends on Leadership.")]
        public float AbilityRadiusIncreasePerLevel { get; set; } = 0.1f;


        [SettingPropertyFloatingInteger("{=dpeaW5tpf0}Base Ability Cooldown", 0f, 300f, "#0 sec", Order = 2, RequireRestart = false, HintText = "")]
        public float BaseAbilityCooldown { get; set; } = 10.0f;

        [SettingPropertyFloatingInteger("{=pVdQUEHQub}Ability Cooldown Decrease per Level", 0f, 200f, "#0 ms", Order = 3, RequireRestart = false, HintText = "{=dmGkKtAq4M}Depends on Leadership.")]
        public float AbilityCooldownDecreasePerLevel { get; set; } = 150f;


        [SettingPropertyFloatingInteger("{=xEeTLJBPXY}Base Allied Morale Gain", 0f, 100f, "#0", Order = 4, RequireRestart = false, HintText = "")]
        public float BaseAlliedMoraleGain { get; set; } = 0f;

        [SettingPropertyFloatingInteger("{=UIOWxJ8oHq}Allied Morale Gain Increase per Level", 0f, 1f, "0.00", Order = 5, RequireRestart = false, HintText = "{=KFuKPM6vlD}Depends on Leadership.")]
        public float AlliedMoraleGainIncreasePerLevel { get; set; } = 0.05f;


        [SettingPropertyFloatingInteger("{=t8iYNduG6l}Base Enemy Chance to Flee", 0f, 1f, "0.00%", Order = 6, RequireRestart = false, HintText = "")]
        public float BaseEnemyChanceToFlee { get; set; } = 0.5f;

        [SettingPropertyFloatingInteger("{=lqtQwtLcWt}Chance to Flee Increase per Level", 0f, 100f, "#0%", Order = 7, RequireRestart = false, HintText = "{=BadCyQmGTs}Depends on Roguery. 1/100 of a %")]
        public float ChanceToFleeIncreasePerLevel { get; set; } = 15f;


        [SettingPropertyFloatingInteger("{=gvqobBynZ4}Enemy Flee Morale Threshold", 0f, 100f, "#0", Order = 8, RequireRestart = false, HintText = "")]
        public float EnemyFleeMoraleThreshold { get; set; } = 15f;


        [SettingPropertyFloatingInteger("{=L5BB55wpAZ}Min Response Delay", 0f, 10f, "0.00 sec", Order = 9, RequireRestart = false, HintText = "")]
        public float MinResponseDelay { get; set; } = 0.7f;

        [SettingPropertyFloatingInteger("{=ykC6USItRT}Max Response Delay", 0f, 10f, "0.00 sec", Order = 10, RequireRestart = false, HintText = "")]
        public float MaxResponseDelay { get; set; } = 2.2f;


        [SettingPropertyFloatingInteger("{=MVuF5MdRAd}Leadership Exp per Ally", 0f, 100f, "#0 exp", Order = 11, RequireRestart = false, HintText = "{=DOR2xn2cYG}Amount of added Leadership experience per ally")]
        public float LeadershipExpPerAlly { get; set; } = 1f;

        [SettingPropertyFloatingInteger("{=mnVFi42L7r}Roguery Exp per Enemy", 0f, 100f, "#0 exp", Order = 12, RequireRestart = false, HintText = "{=z8P8DzRpwc}Amount of added Roguery experience per enemy")]
        public float RogueryExpPerEnemy { get; set; } = 1f;


        //[SettingPropertyBool("{=I7z9HhTKKJ}Fled Enemies Return", Order = 13, RequireRestart = false, HintText = "")]
        //public bool FledEnemiesReturn { get; set; } = false;


        [SettingPropertyBool("{=8LnTLK7Eap}Show Sphere Indicators", Order = 14, RequireRestart = false, HintText = "{=pkSxma77ZI}Show sphere showing the area affecting soldiers")]
        public bool ShowSphereIndicators { get; set; } = true;


        [SettingPropertyBool("{=kDA1RynjWe}Show Detailed Message", Order = 15, RequireRestart = false, HintText = "")]
        public bool ShowDetailedMessage { get; set; } = true;

        [SettingPropertyBool("{=oIjsBtyRi6}Show Detailed Message Text", Order = 16, RequireRestart = false, HintText = "")]
        public bool ShowDetailedMessageText { get; set; } = true;


        [SettingPropertyBool("{=MBlehjhwnx}Enable Cheer Random Delay", Order = 17, RequireRestart = false, HintText = "")]
        public bool EnableCheerRandomDelay { get; set; } = true;

        [SettingPropertyBool("{=GCpn22ub5U}Enable Cheer Animation", Order = 18, RequireRestart = false, HintText = "")]
        public bool EnableCheerAnimation { get; set; } = true;


        [SettingPropertyBool("{=EYQuao3mup}Enable Enemy Reaction", Order = 19, RequireRestart = false, HintText = "")]
        public bool EnableEnemyReaction { get; set; } = true;
    }
}