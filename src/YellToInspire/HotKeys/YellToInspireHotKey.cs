using Bannerlord.ButterLib.HotKeys;

using TaleWorlds.CampaignSystem;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.CustomBattle;

using YellToInspire.Skills;

using HotKeyManager = Bannerlord.ButterLib.HotKeys.HotKeyManager;

namespace YellToInspire.HotKeys
{
    public class YellToInspireHotKey : HotKeyBase
    {
        private static Agent? MainAgent => Mission.Current?.MainAgent;

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
            if (MainAgent is null) return;
            if (!MainAgent.IsPlayerControlled) return;
            if (MBCommon.IsPaused) return;

            if (CustomGame.Current is null && Hero.MainHero is not null)
            {
                if (Hero.MainHero.GetPerkValue(SkillsAndTalents.InspireBasic))
                {
                    InspireManager.Current.InspireAura();
                }
            }
            else
            {
                InspireManager.Current.InspireAura();
            }
        }
    }
}