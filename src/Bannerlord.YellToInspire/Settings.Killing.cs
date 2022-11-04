using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;

namespace Bannerlord.YellToInspire
{
    public sealed partial class Settings
    {
        [SettingPropertyFloatingInteger("{=rRlOssxSIO}Base Ability Kill Multiplier", 0f, 500f, "#0", Order = 0, RequireRestart = false, HintText = "{=XwRROTxNCu}Determines the base inspiration multiplier of killing someone, also the value for Custom Battles. Used in the Killing Gameplay Type. 1/100.")]
        [SettingPropertyGroup("{=s3UCkECKCw}Killing Gameplay System", GroupOrder = 1)]
        public float BaseAbilityKillMultiplier { get; set; } = 100.0f;

        [SettingPropertyFloatingInteger("{=lsn3e1jhWd}Ability Kill Multiplier", 0f, 100f, "#0", Order = 1, RequireRestart = false, HintText = "{=HLvZSlfuA4}Determines the extra inspiration multiplier of killing someone per level. Used in the Killing Gameplay Type. 1 + 1/100.")]
        [SettingPropertyGroup("{=s3UCkECKCw}Killing Gameplay System", GroupOrder = 1)]
        public float AbilityKillMultiplierPerLevel { get; set; } = 10.0f;
    }
}