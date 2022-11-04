namespace Bannerlord.YellToInspire.Data
{
    public record GameplayType(GameplaySystem Type)
    {
        public override string ToString() => Type switch
        {
            GameplaySystem.Killing => "{=Nhd58Fg2b6}Killing",
            GameplaySystem.Cooldown => "{=jsdfYfdFG4}Cooldown",
        };
    }
}