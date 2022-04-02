using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;

namespace Bannerlord.YellToInspire
{
    public sealed partial class Settings
    {
        [SettingPropertyFloatingInteger("{=dpeaW5tpf0}Base Ability Cooldown", 0f, 300f, "#0 sec", Order = 0, RequireRestart = false, HintText = "{=E9nVaeVhWx}Determines the base length of the cooldown period in seconds between ability uses, also the value for Custom Battles.")]
        [SettingPropertyGroup("{=ZX1S3NcDvK}Cooldown Gameplay System", GroupOrder = 2)]
        public float BaseAbilityCooldown { get; set; } = 10.0f;

        [SettingPropertyFloatingInteger("{=pVdQUEHQub}Ability Cooldown Decrease per Level", 0f, 200f, "#0 ms", Order = 1, RequireRestart = false, HintText = "{=dmGkKtAq4M}Determines how much less cooldown time in seconds per level in Leadership. Used in the Cooldown Gameplay Type.")]
        [SettingPropertyGroup("{=ZX1S3NcDvK}Cooldown Gameplay System", GroupOrder = 2)]
        public float AbilityCooldownDecreasePerLevel { get; set; } = 150f;
    }
}