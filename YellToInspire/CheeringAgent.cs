using TaleWorlds.MountAndBlade;

namespace YellToInspire;

public class CheeringAgent
{
	public Agent _agent { get; set; }

	public double _initialTime { get; set; }

	public double _timeDelay { get; set; }

	public CheeringAgent(Agent agent, double initialTime, double delay)
	{
		_agent = agent;
		_initialTime = initialTime;
		_timeDelay = delay;
	}
}
