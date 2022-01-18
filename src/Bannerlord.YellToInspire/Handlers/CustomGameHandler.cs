using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.Handlers
{
    internal sealed class CustomGameHandler : MBSubModuleBase
    {
        private enum CustomGameState { None, BattleSetupScreen, Battle }

        private InspireManager? _inspireManager;
        private CustomGameState _missionState;

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            if (gameStarter is BasicGameStarter)
            {
                _inspireManager = new InspireManager();
            }

            base.OnGameStart(game, gameStarter);
        }

        public override void OnGameEnd(Game game)
        {
            if (_inspireManager is null) return;

            _missionState = CustomGameState.None;
            _inspireManager.MissionEnded();
            _inspireManager.Dispose();
            _inspireManager = null;

            base.OnGameEnd(game);
        }

        protected override void OnApplicationTick(float dt)
        {
            if (_inspireManager is null) return;

            if (Mission.Current is not null)
            {
                if (_missionState != CustomGameState.Battle)
                {
                    _inspireManager.MissionStarted();
                    _missionState = CustomGameState.Battle;
                }

                _inspireManager.MissionTick(dt);
            }
            else
            {
                if (_missionState != CustomGameState.BattleSetupScreen)
                {
                    _inspireManager.MissionEnded();
                    _missionState = CustomGameState.BattleSetupScreen;
                }
            }

            base.OnApplicationTick(dt);
        }
    }
}