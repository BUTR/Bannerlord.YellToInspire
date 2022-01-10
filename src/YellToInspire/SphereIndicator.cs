using System;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace YellToInspire
{
    public class AbilitySphereIndicator
    {
        /// <summary>
        /// Will be set if it's a campaign
        /// </summary>
        private static Hero? Hero => CharacterObject.PlayerCharacter?.HeroObject;

        private Vec3 Position { get; set; } = Vec3.Zero;
        private float SphereCurrentRadius { get; set; } = 0f;

        private double Start { get; set; } = 0d;
        private double SphereLifetime { get; set; } = 3d;

        private bool DisplaySphereIndicators => Math.Abs(SphereCurrentRadius - 0f) > float.Epsilon;
        private double PassedSinceStart => MissionTime.Now.ToSeconds - Start;
        private bool AbilityReady => Start == 0 || PassedSinceStart > Settings.Instance?.AbilityCooldown(Hero);

        public void Trigger()
        {
            if (Settings.Instance is not { } settings) return;

            SphereCurrentRadius = settings.ShowSphereIndicators ? float.Epsilon * 2f : 0f;
            Position = Mission.Current.MainAgent.Position;
            Start = MissionTime.Now.ToSeconds;
        }

        public void Reset()
        {
            Start = 0d;
            SphereCurrentRadius = 0f;
            Position = Vec3.Zero;
        }

        public void Tick(float dt)
        {
            if (Settings.Instance is not { } settings) return;

            if (!AbilityReady)
            {
                SphereCurrentRadius = PassedSinceStart > SphereLifetime
                    ? MBMath.Lerp(SphereCurrentRadius, 0, 0.1f)
                    : MBMath.Lerp(SphereCurrentRadius, settings.AbilityRadius(Hero), 0.1f);
            }

            if (DisplaySphereIndicators)
            {
                MBDebug.RenderDebugSphere(
                    new Vec3(Position.X, Position.Y, Mission.Current.Scene.GetGroundHeightAtPosition(Position)),
                    SphereCurrentRadius,
                    uint.MaxValue,
                    true,
                    0.05f);
            }
        }
    }
}