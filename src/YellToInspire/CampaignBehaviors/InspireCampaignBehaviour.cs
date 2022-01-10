using TaleWorlds.CampaignSystem;

namespace YellToInspire
{
    public class InspireCampaignBehaviour : CampaignBehaviorBase
    {
        private readonly InspireManager _inspireManager = new();
        private readonly CampaignHandler _campaignHandler;

        public InspireCampaignBehaviour(CampaignHandler campaignHandler)
        {
            _campaignHandler = campaignHandler;
            campaignHandler.GameEnd += OnGameEnd;
        }

        public override void SyncData(IDataStore dataStore) { }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, _inspireManager.SetupCampaign);
            CampaignEvents.TickEvent.AddNonSerializedListener(this, _inspireManager.CampaignTick);

            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, _inspireManager.OnNewGameCreated);

            CampaignEvents.MissionTickEvent.AddNonSerializedListener(this, _inspireManager.MissionTick);
            CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, _ => _inspireManager.MissionStarted());
            CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, _ => _inspireManager.MissionEnded());
        }

        private void OnGameEnd()
        {
            _inspireManager.Dispose();
            _campaignHandler.GameEnd -= OnGameEnd;
        }
    }
}