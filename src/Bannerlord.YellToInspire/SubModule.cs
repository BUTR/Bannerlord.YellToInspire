using Bannerlord.YellToInspire.HotKeys;

using TaleWorlds.MountAndBlade;

using HotKeyManager = Bannerlord.ButterLib.HotKeys.HotKeyManager;

namespace Bannerlord.YellToInspire
{
    internal sealed class SubModule : MBSubModuleBase
    {
        private bool _isInitialized;

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;

                if (HotKeyManager.Create("Bannerlord.YellToInspire") is { } hkm)
                {
                    hkm.Add<YellToInspireHotKey>();
                    hkm.Build();
                }
            }
        }
    }
}