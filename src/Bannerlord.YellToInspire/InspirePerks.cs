using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;

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