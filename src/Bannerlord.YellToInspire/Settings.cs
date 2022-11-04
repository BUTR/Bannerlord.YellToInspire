using MCM.Abstractions.Base.Global;

using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.YellToInspire
{
    public sealed partial class Settings : AttributeGlobalSettings<Settings>
    {
        public override string Id => "YellToInspire_v1";
        public override string FolderName => "YellToInspire";
        public override string FormatType => "json2";
        public override string DisplayName => new TextObject("{=V8iSlcN2mq}Yell To Inspire {VERSION}", new()
        {
            { "VERSION", typeof(Settings).Assembly.GetName().Version?.ToString(3) ?? "ERROR" }
        }).ToString();

        public float AbilityRadius(BasicCharacterObject? character)
        {
            var raw = BaseAbilityRadius + (((float?) character?.GetSkillValue(Skills.Leadership) ?? 0f) * AbilityRadiusIncreasePerLevel);
            return MathF.Clamp(raw, 1f, 100f);
        }
        public float AbilityCooldown(BasicCharacterObject? character)
        {
            var raw = BaseAbilityCooldown - (((float?) character?.GetSkillValue(Skills.Leadership) ?? 0f) * (AbilityCooldownDecreasePerLevel / 1000f));
            return MathF.Clamp(raw, 3f, 300f);
        }
        public float AbilityKillMultiplier(BasicCharacterObject? character)
        {
            var raw = BaseAbilityKillMultiplier + (((float?) character?.GetSkillValue(Skills.Leadership) ?? 0f) * (1f + (AbilityKillMultiplierPerLevel / 100f)));
            return MathF.Clamp(raw, 1f, 5f);
        }
        public float AlliedMoraleGain(BasicCharacterObject? character)
        {
            var raw = BaseAlliedMoraleGain + (((float?) character?.GetSkillValue(Skills.Leadership) ?? 0f) * AlliedMoraleGainIncreasePerLevel);
            return MathF.Clamp(raw, 0f, 100f);
        }
        public float EnemyChanceToFlee(BasicCharacterObject? character)
        {
            var raw = BaseEnemyChanceToFlee + (((float?) character?.GetSkillValue(Skills.Roguery) ?? 0f) * ChanceToFleeIncreasePerLevel / 100f);
            return MathF.Clamp(raw, 0f, 1f);
        }
    }
}