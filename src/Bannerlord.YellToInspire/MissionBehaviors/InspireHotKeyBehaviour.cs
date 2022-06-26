using Bannerlord.YellToInspire.HotKeys;

#if e172
using TaleWorlds.MountAndBlade.View.Missions;
#elif e180
using TaleWorlds.MountAndBlade.View.MissionViews;
#endif

namespace Bannerlord.YellToInspire.MissionBehaviors
{
    /// <summary>
    /// We need the derivative <see cref="MissionView"/> so we can use <see cref="MissionView.OnMissionScreenInitialize"/>
    /// to register the <see cref="YellToInspireHotkeyCategory"/>
    /// </summary>
    internal sealed class InspireHotKeyBehaviour : MissionView
    {
        public override void OnMissionScreenInitialize()
        {
            base.OnMissionScreenInitialize();

            // A strange way to inject HotKeys.
            // We need to have an early entrypoint to when Mission.InputManager is injected
            // This provides this early entrypoint, as does OnRenderingStarted()
            // Another way could be to use IsCategoryRegistered() on tick, but no.
            MissionScreen.SceneLayer.Input.RegisterHotKeyCategory(YellToInspireHotkeyCategory.Category);
        }
    }
}