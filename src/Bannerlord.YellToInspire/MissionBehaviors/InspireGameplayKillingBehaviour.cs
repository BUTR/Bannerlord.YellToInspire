using Bannerlord.YellToInspire.MissionBehaviors.AgentComponents;

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors
{
    public class InspireGameplayKillingBehaviour : MissionBehavior
    {
        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public override void OnAgentBuild(Agent agent, Banner banner)
        {
            base.OnAgentBuild(agent, banner);

            if (agent.Character is not { IsHero: true}) return;
            if (!agent.IsHuman) return;

            agent.AddComponent(new SphereIndicatorAgentComponent(agent));
            agent.AddComponent(new InspireNearAgentsAgentComponent(agent));
            agent.AddComponent(new InspireKillingStateAgentComponent(agent));
            agent.AddComponent(new InspireKillingAIAgentComponent(agent));
        }

        public override void OnAgentRemoved(Agent? affectedAgent, Agent? affectorAgent, AgentState agentState, KillingBlow blow)
        {
            base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);

            if (affectedAgent is null) return;

            if (affectorAgent?.GetComponent<InspireKillingStateAgentComponent>() is { } inspireStateAgentComponent)
                inspireStateAgentComponent.OnRemovedAgent(affectedAgent, agentState, blow);
        }

        protected override void OnAgentControllerChanged(Agent agent, Agent.ControllerType oldController)
        {
            base.OnAgentControllerChanged(agent, oldController);

            if (agent.Controller == Agent.ControllerType.Player)
            {
                if (agent.GetComponent<InspireKillingAIAgentComponent>() is { } aiAgentComponent)
                    agent.RemoveComponent(aiAgentComponent);
                agent.AddComponent(new InspireKillingPlayerAgentComponent(agent));
            }

            if (agent.Controller == Agent.ControllerType.AI)
            {
                if (agent.GetComponent<InspireKillingPlayerAgentComponent>() is { } playerAgentComponent)
                    agent.RemoveComponent(playerAgentComponent);
                agent.AddComponent(new InspireKillingAIAgentComponent(agent));
            }
        }
    }
}