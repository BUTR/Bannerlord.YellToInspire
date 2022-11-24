using Bannerlord.BUTR.Shared.Helpers;
using Bannerlord.ButterLib.HotKeys;
using Bannerlord.YellToInspire.CampaignBehaviors;
using Bannerlord.YellToInspire.Data;
using Bannerlord.YellToInspire.HotKeys;
using Bannerlord.YellToInspire.MissionBehaviors;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

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

                if (HotKeyManager.Create(ModuleInfoHelper.GetModuleByType(typeof(SubModule))!.Id) is { } hkm)
                {
                    hkm.Add<YellToInspireHotKey>();
                    hkm.Build();
                }
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (game.GameType is Campaign && gameStarterObject is CampaignGameStarter cgs)
            {
                cgs.AddBehavior(new InspireCampaignBehavior());
            }

            base.OnGameStart(game, gameStarterObject);
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            if (mission.CombatType != Mission.MissionCombatType.NoCombat && Settings.Instance is { } settings)
            {
                if (settings.EnableInAnyMission || (mission.AttackerTeam is not null && mission.DefenderTeam is not null))
                {
                    mission.AddMissionBehavior(new InspireComponentTickBehaviour());
                    mission.AddMissionBehavior(settings.GameplayType.SelectedValue.Type switch
                    {
                        GameplaySystem.Killing => new InspireGameplayKillingBehaviour(),
                        GameplaySystem.Cooldown => new InspireGameplayCooldownBehaviour(),
                    });
                }
            }

            base.OnMissionBehaviorInitialize(mission);
        }
    }
}