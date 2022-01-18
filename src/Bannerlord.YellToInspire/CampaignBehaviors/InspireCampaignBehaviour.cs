using TaleWorlds.CampaignSystem;

using YellToInspire.Handlers;

namespace YellToInspire.CampaignBehaviors
{
    internal sealed class InspireCampaignBehaviour : CampaignBehaviorBase
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