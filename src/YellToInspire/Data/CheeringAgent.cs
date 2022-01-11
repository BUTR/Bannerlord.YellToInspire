using System;

using TaleWorlds.MountAndBlade;

namespace YellToInspire.Data
{
    internal sealed record CheeringAgent(WeakReference<Agent> Agent, double InitialTime, double TimeDelay);
}