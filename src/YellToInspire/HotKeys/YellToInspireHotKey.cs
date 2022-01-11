using Bannerlord.ButterLib.HotKeys;

using TaleWorlds.CampaignSystem;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

using HotKeyManager = Bannerlord.ButterLib.HotKeys.HotKeyManager;

namespace YellToInspire.HotKeys
{
    internal sealed class YellToInspireHotKey : HotKeyBase
    {
        protected override string DisplayName { get; }
        protected override string Description { get; }
        protected override InputKey DefaultKey { get; }
        protected override string Category { get; }

        public YellToInspireHotKey() : base(nameof(YellToInspireHotKey))
        {
            DisplayName = "Yell To Inspire";
            Description = "Yell To Inspire.";
            DefaultKey = InputKey.V;
            Category = HotKeyManager.Categories[HotKeyCategory.Action];
        }

        protected override void OnReleased()
        {
            if (Mission.Current?.MainAgent is not { IsPlayerControlled: true } agent) return;
            if (InspireManager.Current is not { } inspireManager) return;
            if (MBCommon.IsPaused) return;

            if (agent.Character is CharacterObject { HeroObject: { } hero })
            {
                if (hero.GetPerkValue(Perks.InspireBasic))
                {
                    inspireManager.InspireAura(agent);
                }
            }
            else
            {
                inspireManager.InspireAura(agent);
            }
        }
    }
}