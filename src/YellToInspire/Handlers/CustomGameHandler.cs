using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace YellToInspire
{
    public class CustomGameHandler : MBSubModuleBase
    {
        private enum MissionState { None, Started, Finished }

        private InspireManager? _inspireManager;
        private MissionState _missionState;

        protected override void OnApplicationTick(float dt)
        {
            if (_inspireManager is null) return;

            if (Mission.Current is not null)
            {
                if (_missionState != MissionState.Started)
                {
                    _inspireManager.MissionStarted();
                    _missionState = MissionState.Started;
                }

                _inspireManager.MissionTick(dt);
            }
            else
            {
                if (_missionState != MissionState.Finished)
                {
                    _inspireManager.MissionEnded();
                    _missionState = MissionState.Finished;
                }
            }

            base.OnApplicationTick(dt);
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            if (gameStarter is BasicGameStarter)
            {
                _inspireManager = new InspireManager();
                _inspireManager.OnNewGameCreated(gameStarter);
            }

            base.OnGameStart(game, gameStarter);
        }

        public override void OnGameEnd(Game game)
        {
            if (_inspireManager is null) return;

            _missionState = MissionState.Finished;
            _inspireManager.MissionEnded();
            _inspireManager.Dispose();
            _inspireManager = null;

            base.OnGameEnd(game);
        }
    }
}