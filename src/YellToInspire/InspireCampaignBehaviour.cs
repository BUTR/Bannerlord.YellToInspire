using System;
using TaleWorlds.CampaignSystem;

namespace YellToInspire
{
    public class InspireCampaignBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnLaunch);
        }

        private void OnLaunch(CampaignGameStarter campaignGameStarter)
        {
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}