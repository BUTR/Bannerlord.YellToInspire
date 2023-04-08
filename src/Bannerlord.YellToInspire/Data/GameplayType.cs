namespace Bannerlord.YellToInspire.Data
{
    public record GameplayType(GameplaySystem Type)
    {
        public override string ToString() => Type switch
        {
            GameplaySystem.Killing => Strings.DropdownKilling,
            GameplaySystem.Cooldown => Strings.DropdownCooldown,
        };
    }
}