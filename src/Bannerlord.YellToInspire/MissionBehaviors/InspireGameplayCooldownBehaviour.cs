using Bannerlord.YellToInspire.MissionBehaviors.AgentComponents;

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors
{
    public class InspireGameplayCooldownBehaviour : MissionBehavior
    {
        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public override void OnAgentBuild(Agent agent, Banner banner)
        {
            base.OnAgentBuild(agent, banner);

            if (agent.Character is not { IsHero: true}) return;
            if (!agent.IsHuman) return;

            agent.AddComponent(new SphereIndicatorAgentComponent(agent));
            agent.AddComponent(new InspireNearAgentsAgentComponent(agent));
            agent.AddComponent(new InspireCooldownStateAgentComponent(agent));
            agent.AddComponent(new InspireCooldownAIAgentComponent(agent));
        }

        protected override void OnAgentControllerChanged(Agent agent, Agent.ControllerType oldController)
        {
            base.OnAgentControllerChanged(agent, oldController);

            if (agent.Controller == Agent.ControllerType.Player)
            {
                if (agent.GetComponent<InspireCooldownAIAgentComponent>() is { } aiAgentComponent)
                    agent.RemoveComponent(aiAgentComponent);
                agent.AddComponent(new InspireCooldownPlayerAgentComponent(agent));
            }

            if (agent.Controller == Agent.ControllerType.AI)
            {
                if (agent.GetComponent<InspireCooldownPlayerAgentComponent>() is { } playerAgentComponent)
                    agent.RemoveComponent(playerAgentComponent);
                agent.AddComponent(new InspireCooldownAIAgentComponent(agent));
            }
        }
    }
}