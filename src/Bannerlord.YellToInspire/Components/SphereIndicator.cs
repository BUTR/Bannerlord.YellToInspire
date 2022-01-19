using System;

using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.Components
{
    internal sealed class AbilitySphereIndicator : IDisposable
    {
        private Agent? _causingAgent;

        private Vec3 _position = Vec3.Zero;
        private float _radius = 0f;

        private double _abilityStart = 0d;
        private double _lifetime = 3d;

        private bool DisplaySphereIndicators => Math.Abs(_radius - 0f) > float.Epsilon;
        private double PassedSinceAbilityStart => MissionTime.Now.ToSeconds - _abilityStart;

        public void Trigger(Agent causingAgent)
        {
            if (Settings.Instance is not { } settings) return;

            _causingAgent = causingAgent;
            _radius = settings.ShowSphereIndicators ? float.Epsilon * 2f : 0f;
            _position = Mission.Current.MainAgent.Position;
            _abilityStart = MissionTime.Now.ToSeconds;
        }

        public void Reset()
        {
            _abilityStart = 0d;
            _radius = 0f;
            _position = Vec3.Zero;
            MBDebug.ClearRenderObjects();
        }

        public void Tick(float _)
        {
            if (Settings.Instance is not { } settings) return;

            if (_abilityStart != 0 && PassedSinceAbilityStart <= settings.AbilityCooldown(_causingAgent?.Character))
            {
                _radius = PassedSinceAbilityStart > _lifetime
                    ? MBMath.Lerp(_radius, 0, 0.1f)
                    : MBMath.Lerp(_radius, settings.AbilityRadius(_causingAgent?.Character), 0.1f);
            }

            if (settings.ShowSphereIndicators && DisplaySphereIndicators)
            {
                MBDebug.RenderDebugSphere(
                    new Vec3(_position.X, _position.Y, Mission.Current.Scene.GetGroundHeightAtPosition(_position)),
                    _radius,
                    uint.MaxValue,
                    true,
                    0.05f);
            }
        }

        public void Dispose()
        {
            MBDebug.ClearRenderObjects();
        }
    }
}