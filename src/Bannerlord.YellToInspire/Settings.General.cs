using Bannerlord.YellToInspire.Data;

using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Common;

namespace Bannerlord.YellToInspire
{
    public sealed partial class Settings
    {
        [SettingPropertyDropdown("{=K4Inr7E7PZ}Gameplay Type", Order = 0, RequireRestart = false, HintText = "{=adWH7RVK6o}The type of gameplay to use.")]
        [SettingPropertyGroup("{=n0R1p0achr}General", GroupOrder = 0)]
        public Dropdown<GameplayType> GameplayType { get; set; } = new(new[]
        {
            new GameplayType(GameplaySystem.Cooldown),
            new GameplayType(GameplaySystem.Killing),
        }, 0);

        [SettingPropertyBool("{=Hy6Tg26GAw}Enable in Any Mission", Order = 0, RequireRestart = false, HintText = "{=adWH7RVK6o}Keep Yell To Inspire working is missions like visiting the Tavern.")]
        [SettingPropertyGroup("{=n0R1p0achr}General", GroupOrder = 0)]
        public bool EnableInAnyMission { get; set; } = false;
    }
}