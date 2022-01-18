using Bannerlord.YellToInspire.Data;

using System;
using System.Collections.Generic;
using System.Linq;

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.YellToInspire.Components
{
    internal sealed class BattlefieldManager : IDisposable
    {
        private static readonly ActionIndexCache[] CheerActions =
        {
            ActionIndexCache.Create("act_command"),
            ActionIndexCache.Create("act_command_follow")
        };

        private readonly List<WeakReference<Agent>> _affectedAgents = new();
        private readonly List<CheeringAgent> _cheeringAgents = new();
        private bool _unitsResponded = true;
        private double _abilityStart = 0d;
        private Agent? _causingAgent;


        public void Reset()
        {
            _abilityStart = 0d;
            _unitsResponded = true;
            _affectedAgents.Clear();
            _cheeringAgents.Clear();
        }

        public void Trigger(Agent causingAgent)
        {
            _abilityStart = MissionTime.Now.ToSeconds;
            _causingAgent = causingAgent;
        }

        public void Tick(float _)
        {
            if (_cheeringAgents.Count > 0)
                StopCheeringAgentsAfterDelay();

            if (!_unitsResponded)
            {
                if (_affectedAgents.Count == 0)
                    _unitsResponded = true;
                else
                    DelayAndReact();
            }
        }

        private void DelayAndReact()
        {
            if (Settings.Instance is not { } settings) return;
            if (_causingAgent is null) return;

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
                if (_abilityStart + delay > MissionTime.Now.ToSeconds)
                    continue;

                if (agent.Team == _causingAgent.Team)
                {
                    agent.SetWantsToYell();
                    if (!Mission.Current.GetNearbyEnemyAgents(agent.Position.AsVec2, 8f, agent.Team).Any() && settings.EnableCheerAnimation)
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

            if (_affectedAgents.Count == 0)
                _unitsResponded = true;
        }

        public void DelayedAgentReactions(List<WeakReference<Agent>> agents)
        {
            _affectedAgents.Clear();
            if (agents.Count == 0) return;

            _affectedAgents.AddRange(agents);

            _unitsResponded = false;
        }

        public void Cheer(Agent agent)
        {
            agent.SetActionChannel(1, CheerActions[MBRandom.RandomInt(CheerActions.Length)]);
            _cheeringAgents.Add(new CheeringAgent(new(agent), MissionTime.Now.ToSeconds, 1.5));
        }

        private void StopCheeringAgentsAfterDelay()
        {
            if (Mission.Current is null)
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
                    if (!Mission.Current.GetNearbyEnemyAgents(agent.Position.AsVec2, 8f, agent.Team).Any())
                        continue;
                }

                agent.SetActionChannel(1, ActionIndexCache.act_none, true);
                _cheeringAgents.Remove(cheeringAgent);
            }
        }

        public void Dispose()
        {
            _affectedAgents.Clear();
            _cheeringAgents.Clear();
        }
    }
}