using TaleWorlds.InputSystem;

namespace Bannerlord.YellToInspire.HotKeys
{
    public sealed class YellToInspireHotkeyCategory : GameKeyContext
    {
        public static readonly string CategoryId = "YellToInspireCategory";
        public static readonly string YellToInspireKeyId = "YellToInspireKey";
        public static GameKeyContext Category => HotKeyManager.GetCategory(CategoryId);

        public YellToInspireHotkeyCategory() : base(CategoryId, 0, GameKeyContextType.AuxiliarySerializedAndShownInOptions)
        {
            RegisterHotKey(new HotKey(YellToInspireKeyId, CategoryId, InputKey.V));
        }
    }
}