using TaleWorlds.MountAndBlade;

using YellToInspire.HotKeys;

using HotKeyManager = Bannerlord.ButterLib.HotKeys.HotKeyManager;

namespace YellToInspire
{
    internal sealed class SubModule : MBSubModuleBase
    {
        private bool _isInitialized;

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;

                if (HotKeyManager.Create("YellToInspire") is { } hkm)
                {
                    hkm.Add<YellToInspireHotKey>();
                    hkm.Build();
                }
            }
        }
    }
}