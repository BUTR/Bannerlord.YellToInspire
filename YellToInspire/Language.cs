using Newtonsoft.Json;

namespace YellToInspire;

public class Language
{
	[JsonProperty("useCustomFlavorText")]
	public bool UseCustomFlavorText { get; set; }

	[JsonProperty("customFlavorText")]
	public string[] CustomFlavorText { get; set; }

	[JsonProperty("useCustomDetailedText")]
	public bool UseCustomDetailedText { get; set; }

	[JsonProperty("alliesInspiredCustomDetailedText")]
	public string AlliesInspiredCustomDetailedText { get; set; }

	[JsonProperty("alliesRestoredCustomDetailedText")]
	public string AlliesRestoredCustomDetailedText { get; set; }

	[JsonProperty("enemiesScaredCustomDetailedText")]
	public string EnemiesScaredCustomDetailedText { get; set; }

	[JsonProperty("enemiesFledCustomDetailedText")]
	public string EnemiesFledCustomDetailedText { get; set; }

	[JsonProperty("enemiesRestoredCustomDetailedText")]
	public string EnemiesRestoredCustomDetailedText { get; set; }
}
