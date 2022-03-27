using TaleWorlds.Core;

namespace Bannerlord.YellToInspire
{
    public static class Skills
    {
        public static SkillObject? Leadership => Game.Current.DefaultSkills is null ? null : DefaultSkills.Leadership;
        public static SkillObject? Roguery => Game.Current.DefaultSkills is null ? null : DefaultSkills.Roguery;
    }
}