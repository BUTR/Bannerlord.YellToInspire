namespace Bannerlord.YellToInspire.Data
{
    public record TroopStatistics
    {
        public static TroopStatistics Empty { get; } = new();
            
        public int Inspired { get; set; }
        public int Retreating { get; set; }
        public int Nearby { get; set; }
        public int Fled { get; set; }
    }
}