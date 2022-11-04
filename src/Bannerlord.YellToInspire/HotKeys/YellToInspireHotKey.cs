using Bannerlord.ButterLib.HotKeys;

using TaleWorlds.InputSystem;

using HotKeyManager = Bannerlord.ButterLib.HotKeys.HotKeyManager;

namespace Bannerlord.YellToInspire.HotKeys
{
    internal sealed class YellToInspireHotKey : HotKeyBase
    {
        protected override string DisplayName { get; } = "{=3F9hwn8h4W}Yell To Inspire";
        protected override string Description { get; } = "{=u68iVSY338}Yell To Inspire.";
        protected override InputKey DefaultKey { get; } = InputKey.V;
        protected override string Category { get; } = HotKeyManager.Categories[HotKeyCategory.Action];

        public YellToInspireHotKey() : base(nameof(YellToInspireHotKey)) { }
    }
}