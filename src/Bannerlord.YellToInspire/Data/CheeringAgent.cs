using System;

using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.Data
{
    internal sealed record CheeringAgent(WeakReference<Agent> Agent, double InitialTime, double TimeDelay);
}