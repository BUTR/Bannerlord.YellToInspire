using System;
using TaleWorlds.CampaignSystem;

namespace YellToInspire;

public class InspireCampaignBehaviour : CampaignBehaviorBase
{
	public override void RegisterEvents()
	{
		CampaignEvents.get_OnSessionLaunchedEvent().AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnLaunch);
	}

	private void OnLaunch(CampaignGameStarter campaignGameStarter)
	{
	}

	public override void SyncData(IDataStore dataStore)
	{
	}
}
