using Bannerlord.YellToInspire.Utils;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire
{
    public class Perks : MBSubModuleBase
    {
        public static Perks Instance { get; private set; } = default!;

        public PerkObject? InspireBasic { get; internal set; } = default!;
        public PerkObject? InspireResolve { get; internal set; } = default!;
        //public PerkObject? InspireHaste { get; internal set; } = default!;
        //public PerkObject? InspireFortitude { get; internal set; } = default!;

        public Perks()
        {
            Instance = this;
        }

        public override void RegisterSubModuleObjects(bool isSavedCampaign)
        {
            RegisterAll();

            base.RegisterSubModuleObjects(isSavedCampaign);
        }

        public override void OnGameEnd(Game game)
        {
            UnregisterAll();

            base.OnGameEnd(game);
        }

        private void RegisterAll()
        {
            InspireBasic = CreatePerk(Strings.InspireName);
            InspireResolve = CreatePerk(Strings.InspireResolveName);

            InitializeAll();
        }

        private void InitializeAll()
        {
            InspireBasic?.Initialize(Strings.InspireName, CommonUtils.SetVariables(Strings.InspireBasicDescription).ToString(),
                Skills.Leadership, 5, null, SkillEffect.PerkRole.PartyLeader, 10f,
                SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Add);
            InspireResolve?.Initialize(Strings.InspireResolveName, Strings.InspireResolveDescription.ToString(),
                Skills.Leadership, 35, null, SkillEffect.PerkRole.PartyLeader, 15f,
                SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor);
        }

        private void UnregisterAll()
        {
            InspireBasic = null;
            InspireResolve = null;
        }

        private static PerkObject CreatePerk(string stringId) => Game.Current.ObjectManager.RegisterPresumedObject(new PerkObject(stringId));
    }
}