using System;

using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    /// <summary>
    /// Displays the Debug Sphere of the affected area.
    /// </summary>
    internal sealed class SphereIndicatorAgentComponent : AgentComponent, IAgentComponentOnTick
    {
        private SettingsProviderMissionBehavior? SettingsProvider => Agent.Mission.GetMissionBehavior<SettingsProviderMissionBehavior>();
        private Settings? Settings => SettingsProvider is { } settingsProvider ? settingsProvider.Get<Settings>() : null;

        private Vec3 _position = Vec3.Zero;
        private float _radius = 0f;

        private double _triggerTime = 0d;
        private double _lifetime = 3d;

        private bool DisplaySphereIndicators => Math.Abs(_radius - 0f) > float.Epsilon;
        private double PassedSinceTrigger => MissionTime.Now.ToSeconds - _triggerTime;

        public SphereIndicatorAgentComponent(Agent agent) : base(agent) { }

        public void Trigger()
        {
            if (Settings is not { } settings) return;

            _radius = settings.ShowSphereIndicators ? float.Epsilon * 2f : 0f;
            _position = Agent.Position;
            _triggerTime = MissionTime.Now.ToSeconds;
        }

        public void OnTick(float _)
        {
            if (Settings is not { } settings) return;

            if (!MBCommon.IsPaused && _triggerTime != 0 && PassedSinceTrigger <= settings.AbilityCooldown(Agent.Character))
            {
                _radius = PassedSinceTrigger > _lifetime
                    ? MBMath.Lerp(_radius, 0, 0.1f)
                    : MBMath.Lerp(_radius, settings.AbilityRadius(Agent.Character), 0.1f);
            }

            if (settings.ShowSphereIndicators && DisplaySphereIndicators)
            {
                MBDebug.RenderDebugSphere(
                    new Vec3(_position.X, _position.Y, Agent.Mission.Scene.GetGroundHeightAtPosition(_position)),
                    _radius,
                    uint.MaxValue,
                    true,
                    0.05f);
            }
        }

        public override void OnAgentRemoved()
        {
            base.OnAgentRemoved();

            _triggerTime = 0d;
            _radius = 0f;
            _position = Vec3.Zero;
        }
    }
}