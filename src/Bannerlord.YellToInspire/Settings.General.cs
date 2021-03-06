using Bannerlord.YellToInspire.Data;

using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Dropdown;

namespace Bannerlord.YellToInspire
{
    public sealed partial class Settings
    {
        [SettingPropertyBool("{=K4Inr7E7PZ}Gameplay Type", Order = 0, RequireRestart = false, HintText = "{=adWH7RVK6o}The type of gameplay to use.")]
        [SettingPropertyGroup("{=n0R1p0achr}General", GroupOrder = 0)]
        public DropdownDefault<GameplayType> GameplayType { get; set; } = new(new[]
        {
            Data.GameplayType.Cooldown,
            Data.GameplayType.Killing,
        }, 0);
    }
}