using TaleWorlds.Core;

#if e162 || e163 || e164 || e165 || e170 || e171
using PerkObject = TaleWorlds.CampaignSystem.PerkObject;
#elif e172
using PerkObject = TaleWorlds.CampaignSystem.CharacterDevelopment.PerkObject;
#endif

namespace Bannerlord.YellToInspire
{
    public static class Skills
    {
        public static SkillObject? Leadership => Game.Current.DefaultSkills is null ? null : DefaultSkills.Leadership;
        public static SkillObject? Roguery => Game.Current.DefaultSkills is null ? null : DefaultSkills.Roguery;
    }

    public static class Perks
    {
        public static PerkObject? InspireBasic { get; internal set; }
        public static PerkObject? InspireResolve { get; internal set; }
        //public static PerkObject? InspireHaste { get; internal set; }
        //public static PerkObject? InspireFortitude { get; internal set; }
    }
}