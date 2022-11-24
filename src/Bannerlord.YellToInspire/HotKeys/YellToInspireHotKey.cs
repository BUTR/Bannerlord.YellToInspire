using Bannerlord.ButterLib.HotKeys;

using TaleWorlds.InputSystem;

using HotKeyManager = Bannerlord.ButterLib.HotKeys.HotKeyManager;

namespace Bannerlord.YellToInspire.HotKeys
{
    internal sealed class YellToInspireHotKey : HotKeyBase
    {
        protected override string DisplayName => Strings.HotKeyYellToInspire;
        protected override string Description => Strings.HotKeyYellToInspireDescription;
        protected override InputKey DefaultKey => InputKey.V;
        protected override string Category { get; } = HotKeyManager.Categories[HotKeyCategory.Action];

        public YellToInspireHotKey() : base(nameof(YellToInspireHotKey)) { }
    }
}