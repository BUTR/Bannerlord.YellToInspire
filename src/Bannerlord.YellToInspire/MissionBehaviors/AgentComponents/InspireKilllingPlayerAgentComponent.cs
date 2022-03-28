using Bannerlord.YellToInspire.HotKeys;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    internal sealed class InspireKilllingPlayerAgentComponent : AgentComponent, IAgentComponentOnTick
    {
        private static readonly TextObject ReadyText = new("{=Zt8Qbxh3HP}Your Inspire ability is ready!");
        private static readonly TextObject NotReadyText = new("{=MFZQek5Upy}You are not ready to use Inspire yet!");

        private SettingsProviderMissionBehavior? SettingsProvider => Agent.Mission.GetMissionBehavior<SettingsProviderMissionBehavior>();
        private Settings? Settings => SettingsProvider is { } settingsProvider ? settingsProvider.Get<Settings>() : null;
        private InspireKillingStateAgentComponent? State => Agent.GetComponent<InspireKillingStateAgentComponent>();

        private bool _messageWasShown = false;

        public InspireKilllingPlayerAgentComponent(Agent agent) : base(agent) { }

        public void OnTick(float dt)
        {
            if (MBCommon.IsPaused) return;

            if (Settings is not { } settings) return;
            if (State is not { } state) return;

            if (!_messageWasShown)
            {
                if (state.CanInspire())
                {
                    InformationManager.DisplayMessage(new(ReadyText.ToString()));
                    _messageWasShown = true;
                }
            }

            if (Agent.Mission.InputManager.IsHotKeyDownAndReleased(YellToInspireHotkeyCategory.YellToInspireKeyId))
            {

                if (!state.CanInspire())
                {
                    InformationManager.DisplayMessage(new(NotReadyText.ToString()));
                    return;
                }

                if (Agent.Character is CharacterObject { HeroObject: { } hero })
                {
                    if (hero.GetPerkValue(Perks.InspireBasic))
                    {
                        state.ResetInspiration();
                        var troopsStatistics = Utils.InspireAura(Agent);

                        InformationManager.DisplayMessage(new(Utils.AbilityPhrases[MBRandom.RandomInt(0, Utils.AbilityPhrases.Length)].ToString()));
                        if (settings.ShowDetailedMessage)
                            Utils.ShowDetailedMessage(troopsStatistics);
                    }
                }
                else
                {
                    state.ResetInspiration();
                    var troopsStatistics = Utils.InspireAura(Agent);

                    InformationManager.DisplayMessage(new(Utils.AbilityPhrases[MBRandom.RandomInt(0, Utils.AbilityPhrases.Length)].ToString()));
                    if (settings.ShowDetailedMessage)
                        Utils.ShowDetailedMessage(troopsStatistics);
                }
            }
        }
    }
}