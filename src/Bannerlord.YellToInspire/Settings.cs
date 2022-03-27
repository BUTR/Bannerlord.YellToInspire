using Bannerlord.YellToInspire.Data;

using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Dropdown;
using MCM.Abstractions.Settings.Base.Global;

using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.YellToInspire
{
    public sealed class Settings : AttributeGlobalSettings<Settings>
    {
        public override string Id => "YellToInspire_v1";
        public override string FolderName => "YellToInspire";
        public override string FormatType => "json2";
        public override string DisplayName => new TextObject("{=V8iSlcN2mq}Yell To Inspire {VERSION}", new()
        {
            { "VERSION", typeof(Settings).Assembly.GetName().Version?.ToString(3) ?? "ERROR" }
        }).ToString();

        public float AbilityRadius(BasicCharacterObject? character)
        {
            var raw = BaseAbilityRadius + (((float?) character?.GetSkillValue(Skills.Leadership) ?? 0f) * AbilityRadiusIncreasePerLevel);
            return MathF.Clamp(raw, 1f, 100f);
        }
        public float AbilityCooldown(BasicCharacterObject? character)
        {
            var raw = BaseAbilityCooldown - (((float?) character?.GetSkillValue(Skills.Leadership) ?? 0f) * (AbilityCooldownDecreasePerLevel / 1000f));
            return MathF.Clamp(raw, 3f, 300f);
        }
        public float AbilityKillMultiplier(BasicCharacterObject? character)
        {
            var raw = BaseAbilityKillMultiplier + (((float?) character?.GetSkillValue(Skills.Leadership) ?? 0f) * (1f + (AbilityKillMultiplierPerLevel / 100f)));
            return MathF.Clamp(raw, 1f, 5f);
        }
        public float AlliedMoraleGain(BasicCharacterObject? character)
        {
            var raw = BaseAlliedMoraleGain + (((float?) character?.GetSkillValue(Skills.Leadership) ?? 0f) * AlliedMoraleGainIncreasePerLevel);
            return MathF.Clamp(raw, 0f, 100f);
        }
        public float EnemyChanceToFlee(BasicCharacterObject? character)
        {
            var raw = BaseEnemyChanceToFlee + (((float?) character?.GetSkillValue(Skills.Roguery) ?? 0f) * ChanceToFleeIncreasePerLevel / 100f);
            return MathF.Clamp(raw, 0f, 1f);
        }


        [SettingPropertyFloatingInteger("{=OSbrbCqXUR}Base Ability Radius", 0f, 100f, "#0 units", Order = 0, RequireRestart = false, HintText = "{=4eLA2GbEcp}Determines the base radius of your yell ability in meters, also the value for Custom Battles.")]
        public float BaseAbilityRadius { get; set; } = 10f;

        [SettingPropertyFloatingInteger("{=p2qRWZ7Ezg}Ability Radius Increase per Level", 0f, 10f, "0.00 units", Order = 1, RequireRestart = false, HintText = "{=f0iSyhJJ9H}The base amount (which is multiplied by attribute and focus bonus) of Leadership exp gained per ally affected by Inspire. Determines how much extra ability radius in meters per level in Leadership.")]
        public float AbilityRadiusIncreasePerLevel { get; set; } = 0.1f;


        [SettingPropertyFloatingInteger("{=dpeaW5tpf0}Base Ability Cooldown", 0f, 300f, "#0 sec", Order = 2, RequireRestart = false, HintText = "{=E9nVaeVhWx}Determines the base length of the cooldown period in seconds between ability uses, also the value for Custom Battles.")]
        public float BaseAbilityCooldown { get; set; } = 10.0f;

        [SettingPropertyFloatingInteger("{=pVdQUEHQub}Ability Cooldown Decrease per Level", 0f, 200f, "#0 ms", Order = 3, RequireRestart = false, HintText = "{=dmGkKtAq4M}Determines how much less cooldown time in seconds per level in Leadership. Used in the Cooldown Gameplay Type.")]
        public float AbilityCooldownDecreasePerLevel { get; set; } = 150f;


        [SettingPropertyFloatingInteger("{=rRlOssxSIO}Base Ability Kill Multiplier", 0f, 500f, "#0", Order = 4, RequireRestart = false, HintText = "{=XwRROTxNCu}Determines the base inspiration multiplier of killing someone, also the value for Custom Battles. Used in the Killing Gameplay Type. 1/100.")]
        public float BaseAbilityKillMultiplier { get; set; } = 100.0f;

        [SettingPropertyFloatingInteger("{=lsn3e1jhWd}Ability Kill Multiplier", 0f, 100f, "#0", Order = 5, RequireRestart = false, HintText = "{=HLvZSlfuA4}Determines the extra inspiration multiplier of killing someone per level. Used in the Killing Gameplay Type. 1 + 1/100.")]
        public float AbilityKillMultiplierPerLevel { get; set; } = 10.0f;


        [SettingPropertyFloatingInteger("{=xEeTLJBPXY}Base Allied Morale Gain", 0f, 100f, "#0", Order = 6, RequireRestart = false, HintText = "{=lYflwGnkSY}Determines the base anount of how much morale each friendly unit gains per yell, also the value for Custom Battles.")]
        public float BaseAlliedMoraleGain { get; set; } = 0f;

        [SettingPropertyFloatingInteger("{=UIOWxJ8oHq}Allied Morale Gain Increase per Level", 0f, 1f, "0.00", Order = 7, RequireRestart = false, HintText = "{=KFuKPM6vlD}Determines how much extra morale gain per level in Leadership.")]
        public float AlliedMoraleGainIncreasePerLevel { get; set; } = 0.05f;


        [SettingPropertyFloatingInteger("{=t8iYNduG6l}Base Enemy Chance to Flee", 0f, 1f, "0.00%", Order = 8, RequireRestart = false, HintText = "{=9k0gcHzZiB}The % chance an enemy will flee if affected by a yell and their morale is low enough.")]
        public float BaseEnemyChanceToFlee { get; set; } = 0.5f;

        [SettingPropertyFloatingInteger("{=lqtQwtLcWt}Chance to Flee Increase per Level", 0f, 100f, "#0%", Order = 9, RequireRestart = false, HintText = "{=BadCyQmGTs}Determines how much extra % chance to make enemies flee per level in Roguery. 1/100 of a %.")] //
        public float ChanceToFleeIncreasePerLevel { get; set; } = 15f;


        [SettingPropertyFloatingInteger("{=gvqobBynZ4}Enemy Flee Morale Threshold", 0f, 100f, "#0", Order = 10, RequireRestart = false, HintText = "{=fKTW3QsIOh}Determines how low morale needs to be for an enemy unit to flee.")]
        public float EnemyFleeMoraleThreshold { get; set; } = 15f;


        [SettingPropertyFloatingInteger("{=L5BB55wpAZ}Min Response Delay", 0f, 10f, "0.00 sec", Order = 11, RequireRestart = false, HintText = "{=RyxKXEQgm6}The minimum amount of random time in seconds before a unit responds to your yell.")]
        public float MinResponseDelay { get; set; } = 0.7f;

        [SettingPropertyFloatingInteger("{=ykC6USItRT}Max Response Delay", 0f, 10f, "0.00 sec", Order = 12, RequireRestart = false, HintText = "{=nAwgC95YKZ}The maximum amount of random time in seconds before a unit responds to your yell.")]
        public float MaxResponseDelay { get; set; } = 2.2f;


        [SettingPropertyFloatingInteger("{=MVuF5MdRAd}Leadership Exp per Ally", 0f, 100f, "#0 exp", Order = 13, RequireRestart = false, HintText = "{=DOR2xn2cYG}The base amount (which is multiplied by attribute and focus bonus) of Leadership exp gained per ally affected by Inspire.")]
        public float LeadershipExpPerAlly { get; set; } = 1f;

        [SettingPropertyFloatingInteger("{=mnVFi42L7r}Roguery Exp per Enemy", 0f, 100f, "#0 exp", Order = 14, RequireRestart = false, HintText = "{=z8P8DzRpwc}The base amount (which is multiplied by attribute and focus bonus) of Roguery exp gained per enemy affected by Inspire.")]
        public float RogueryExpPerEnemy { get; set; } = 1f;


        //[SettingPropertyBool("{=I7z9HhTKKJ}Fled Enemies Return", Order = 15, RequireRestart = false, HintText = "{=n6ViryinCm}Determines if fleeing enemies return to battle, like a taunt of sorts.")]
        //public bool FledEnemiesReturn { get; set; } = false;


        [SettingPropertyBool("{=8LnTLK7Eap}Show Sphere Indicators", Order = 16, RequireRestart = false, HintText = "{=pkSxma77ZI}Show sphere showing the area affecting soldiers.")]
        public bool ShowSphereIndicators { get; set; } = true;


        [SettingPropertyBool("{=kDA1RynjWe}Show Detailed Message", Order = 17, RequireRestart = false, HintText = "")]
        public bool ShowDetailedMessage { get; set; } = true;

        [SettingPropertyBool("{=oIjsBtyRi6}Show Detailed Message Text", Order = 18, RequireRestart = false, HintText = "{=qiJ4Uud7qw}Displays message containing a rundown of the number of friendly and enemy units affected per ability use.")]
        public bool ShowDetailedMessageText { get; set; } = true;


        [SettingPropertyBool("{=MBlehjhwnx}Enable Cheer Random Delay", Order = 19, RequireRestart = false, HintText = "{=CTT0d1Q6Q7}Determines if units individually respond to yells with a small random amount of delay.")]
        public bool EnableCheerRandomDelay { get; set; } = true;

        [SettingPropertyBool("{=GCpn22ub5U}Enable Cheer Animation", Order = 20, RequireRestart = false, HintText = "{=cPUY6zLpa6}Determines if the player and allies units play cheering animations.")]
        public bool EnableCheerAnimation { get; set; } = true;


        [SettingPropertyBool("{=EYQuao3mup}Enable Enemy Reaction", Order = 21, RequireRestart = false, HintText = "{=YncLuM2Sps}Determines whether enemies verbally respond in fear to your yells.")]
        public bool EnableEnemyReaction { get; set; } = true;


        [SettingPropertyBool("{=K4Inr7E7PZ}Gameplay Type", Order = 22, RequireRestart = false, HintText = "{=adWH7RVK6o}The type of gameplay to use.")]
        public DropdownDefault<GameplayType> GameplayType { get; set; } = new(new []
        {
            Data.GameplayType.Cooldown,
            Data.GameplayType.Killing,
        }, 0);
    }
}