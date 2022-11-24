using Bannerlord.YellToInspire.Utils;

using MCM;

using System.ComponentModel;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Bannerlord.YellToInspire.CampaignBehaviors
{
    public class InspireCampaignBehavior : CampaignBehaviorBase
    {
        public override void SyncData(IDataStore dataStore) { }

        public override void RegisterEvents()
        {
            if (GetCampaignBehavior<SettingsProviderCampaignBehavior>() is { } settingsProvider && settingsProvider.Get<Settings>() is { } settings)
            {
                settings.PropertyChanged += SettingsOnPropertyChanged;
            }

            CampaignEvents.HeroGainedSkill.AddNonSerializedListener(this, OnHeroGainedSkill);
            CampaignEvents.OnGameOverEvent.AddNonSerializedListener(this, OnGameOver);
        }

        private void SettingsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshPerks();
        }

        private void OnHeroGainedSkill(Hero hero, SkillObject skill, int change, bool shouldNotify)
        {
            if (hero != Hero.MainHero) return;

            if (skill != DefaultSkills.Leadership && skill != DefaultSkills.Roguery) return;

            RefreshPerks();
        }

        private void OnGameOver()
        {
            if (GetCampaignBehavior<SettingsProviderCampaignBehavior>() is { } settingsProvider && settingsProvider.Get<Settings>() is { } settings)
            {
                settings.PropertyChanged -= SettingsOnPropertyChanged;
            }
        }

        private static void RefreshPerks()
        {
            Perks.Instance.InspireBasic?.Initialize(new TextObject(Strings.InspireName), CommonUtils.SetVariables(Strings.InspireBasicDescription));
        }
    }
}