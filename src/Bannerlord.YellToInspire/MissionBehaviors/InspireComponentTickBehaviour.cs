using Bannerlord.YellToInspire.MissionBehaviors.AgentComponents;

using System.Linq;

using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors
{
    /// <summary>
    /// Provides a workaround for <see cref="AgentComponent"/> to receive the <see cref="MissionBehavior.OnMissionTick"/> call.
    /// For some reason the <see cref="AgentComponent"/> only has <see cref="AgentComponent.OnTickAsAI"/>,
    /// so if we added it, the Player won't be handled.
    /// </summary>
    internal sealed class InspireComponentTickBehaviour : MissionBehavior
    {
        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);

            // Can't believe I need to create my own tick method
            var activeAgents = Mission.Agents;
            for (var i = 0; i < activeAgents.Count; i++)
            {
                var components = activeAgents[i].Components;
                for (var j = 0; j < components.Count; j++)
                {
                    if (components[j] is IAgentComponentOnTick agentComponent)
                    {
                        agentComponent.OnTick(dt);
                    }
                }
            }
        }
    }
}