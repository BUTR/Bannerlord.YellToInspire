using System;
using System.Collections.Generic;
using System.Linq;

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.MissionBehaviors.AgentComponents
{
    /// <summary>
    /// Manages the actual cheering of Agents provided to <see cref="Trigger"/>.
    /// </summary>
    public class InspireNearAgentsAgentComponent : InspireBaseAgentComponent, IAgentComponentOnTick
    {
        protected sealed record CheeringAgent(WeakReference<Agent> Agent, double InitialTime, double TimeDelay);

        protected static readonly ActionIndexCache[] CheerActions =
        {
            ActionIndexCache.Create("act_command"),
            ActionIndexCache.Create("act_command_follow")
        };


        protected readonly List<WeakReference<Agent>> _affectedAgents = new();
        protected readonly List<CheeringAgent> _cheeringAgents = new();

        protected double _triggerTime = 0d;

        public InspireNearAgentsAgentComponent(Agent agent) : base(agent) { }

        public virtual void Trigger(IEnumerable<WeakReference<Agent>> affectedAgents)
        {
            if (Settings is not { } settings) return;

            _affectedAgents.AddRange(affectedAgents);

            _triggerTime = MissionTime.Now.ToSeconds;

            if (!Agent.Mission.GetNearbyEnemyAgents(Agent.Position.AsVec2, 8f, Agent.Team).Any() && settings.EnableCheerAnimation)
            {
                Cheer(Agent);
            }
        }

        public virtual void OnTick(float _)
        {
            if (_cheeringAgents.Count > 0)
                StopCheeringAgentsAfterDelay();

            if (_affectedAgents.Count > 0)
            {
                DelayAndReact();
            }
        }

        public override void OnAgentRemoved()
        {
            base.OnAgentRemoved();

            _affectedAgents.Clear();
            _cheeringAgents.Clear();
            _triggerTime = 0f;
        }

        protected virtual void DelayAndReact()
        {
            if (Settings is not { } settings) return;

            foreach (var weakReference in _affectedAgents.ToList())
            {
                if (!weakReference.TryGetTarget(out var agent))
                {
                    _affectedAgents.Remove(weakReference);
                    continue;
                }

                if (!agent.IsActive())
                {
                    _affectedAgents.Remove(weakReference);
                    continue;
                }

                var delay = settings.EnableCheerRandomDelay ? MBRandom.RandomFloatRanged(settings.MinResponseDelay, settings.MaxResponseDelay) : 1f;
                if (_triggerTime + delay > MissionTime.Now.ToSeconds)
                    continue;

                if (agent.Team == Agent.Team)
                {
                    agent.SetWantsToYell();
                    if (!Agent.Mission.GetNearbyEnemyAgents(agent.Position.AsVec2, 8f, agent.Team).Any() && settings.EnableCheerAnimation)
                    {
                        Cheer(agent);
                    }
                }
                else if (settings.EnableEnemyReaction)
                {
                    agent.MakeVoice(SkinVoiceManager.VoiceType.Fear, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
                }
                _affectedAgents.Remove(weakReference);
            }
        }

        protected virtual void Cheer(Agent agent)
        {
            agent.SetActionChannel(1, CheerActions[MBRandom.RandomInt(CheerActions.Length)]);
            _cheeringAgents.Add(new CheeringAgent(new(agent), MissionTime.Now.ToSeconds, 1.5));
        }

        protected virtual void StopCheeringAgentsAfterDelay()
        {
            if (Agent.Mission is null)
            {
                _cheeringAgents.Clear();
                return;
            }

            foreach (var cheeringAgent in _cheeringAgents.ToList())
            {
                if (!cheeringAgent.Agent.TryGetTarget(out var agent))
                {
                    _cheeringAgents.Remove(cheeringAgent);
                    continue;
                }

                if (cheeringAgent.InitialTime + cheeringAgent.TimeDelay > MissionTime.Now.ToSeconds)
                {
                    if (!Agent.Mission.GetNearbyEnemyAgents(agent.Position.AsVec2, 8f, agent.Team).Any())
                        continue;
                }

                agent.SetActionChannel(1, ActionIndexCache.act_none, true);
                _cheeringAgents.Remove(cheeringAgent);
            }
        }
    }
}