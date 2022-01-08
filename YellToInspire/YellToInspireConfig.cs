using Newtonsoft.Json;

namespace YellToInspire;

public class YellToInspireConfig
{
	[JsonProperty("yellKey")]
	public int YellKey { get; set; }

	[JsonProperty("abilityRadius")]
	public float AbilityRadius { get; set; }

	[JsonProperty("abilityCooldown")]
	public double AbilityCooldown { get; set; }

	[JsonProperty("alliedMoraleGain")]
	public float AlliedMoraleGain { get; set; }

	[JsonProperty("enemyFleeMoraleThreshold")]
	public float EnemyFleeMoraleThreshold { get; set; }

	[JsonProperty("minResponseDelay")]
	public float MinResponseDelay { get; set; }

	[JsonProperty("maxResponseDelay")]
	public float MaxResponseDelay { get; set; }

	[JsonProperty("enemyChanceToFlee")]
	public float EnemyChanceToFlee { get; set; }

	[JsonProperty("baseLeadershipExpPerAlly")]
	public float BaseLeadershipExpPerAlly { get; set; }

	[JsonProperty("baseRogueryExpPerEnemy")]
	public float BaseRogueryExpPerEnemy { get; set; }

	[JsonProperty("radiusIncreasePerLevel")]
	public float RadiusIncreasePerLevel { get; set; }

	[JsonProperty("cooldownDecreasePerLevel")]
	public float CooldownDecreasePerLevel { get; set; }

	[JsonProperty("moraleGainIncreasePerLevel")]
	public float MoraleGainIncreasePerLevel { get; set; }

	[JsonProperty("chanceToFleeIncreasePerLevel")]
	public float ChanceToFleeIncreasePerLevel { get; set; }

	[JsonProperty("fleeingEnemiesReturn")]
	public bool FleeingEnemiesReturn { get; set; }

	[JsonProperty("showSphereIndicators")]
	public bool ShowSphereIndicators { get; set; }

	[JsonProperty("showDetailedMessage")]
	public bool ShowDetailedMessage { get; set; }

	[JsonProperty("enableRandomCheerDelay")]
	public bool EnableRandomCheerDelay { get; set; }

	[JsonProperty("enableCheerAnimation")]
	public bool EnableCheerAnimation { get; set; }

	[JsonProperty("enableEnemyReaction")]
	public bool EnableEnemyReaction { get; set; }
}
