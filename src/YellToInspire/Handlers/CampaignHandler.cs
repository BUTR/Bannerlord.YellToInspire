using System;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace YellToInspire
{
    public class CampaignHandler : MBSubModuleBase
    {
        public event Action? GameEnd;

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (gameStarterObject is CampaignGameStarter campaignGameStarter)
            {
                campaignGameStarter.AddBehavior(new InspireCampaignBehaviour(this));
            }

            base.OnGameStart(game, gameStarterObject);
        }

        public override void OnGameEnd(Game game)
        {
            GameEnd?.Invoke();

            base.OnGameEnd(game);
        }
    }
}