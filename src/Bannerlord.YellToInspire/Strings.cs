using TaleWorlds.Localization;

namespace Bannerlord.YellToInspire
{
    public static class Strings
    {
        public static readonly string InspireName = "{=X0lXr5OeSQ}Inspire";
        public static readonly string InspireResolveName = "{=fTNsdClmjc}Inspire Resolve";


        public static readonly TextObject InspireResolveDescription = new(@"{=cGR5dN440y}
Friendly fleeing units under the effect of Inspire regain their resolve and return to the fight!");
        //public static readonly TextObject InspireHasteDescription = new(@"{=NmyOWk1YDa}
        //Friendly units under the effect of Inspire gain a short temporary speed boost!");
        // public static readonly TextObject InspireFortitudeDescription = new(@"{=YwMTY7RbUv}
        //Friendly units under the effect of Inspire gain a small temporary health boost!");

        public static readonly TextObject InspireBasicDescription = new(@"{=c2MjU4ZqPQ}
Press {INPUT} to belt out a mighty yell that inspires allies{NEWLINE}
and instills fear in the hearts of your enemies.{NEWLINE}
Nearby allies are given a small morale boost, and{NEWLINE}
nearby enemies with low morale have a chance of fleeing!{NEWLINE}
Radius, allied morale gain, and cooldown scale with Leadership.{NEWLINE}
Enemy flee chance scales with Roguery.{NEWLINE}{NEWLINE}
Current radius size: {RADIUS} meters{NEWLINE}
Allied morale gain: +{MORALE} morale {NEWLINE}
Enemy flee chance: {FLEE}% {NEWLINE}
Cooldown: {COOLDOWN} seconds");

        public static readonly string DropdownKilling = "{=Nhd58Fg2b6}Killing";
        public static readonly string DropdownCooldown = "{=jsdfYfdFG4}Cooldown";

        public static readonly string HotKeyYellToInspire = "{=3F9hwn8h4W}Yell To Inspire";
        public static readonly string HotKeyYellToInspireDescription = "{=u68iVSY338}Yell To Inspire.";

        public static readonly TextObject MessageCooldown = new("{=FsbQpeMJYJ}Your ability is still on cooldown for {TIME} second(s)!");

        public static readonly TextObject AbilityReady = new("{=Zt8Qbxh3HP}Your Inspire ability is ready!");
        public static readonly TextObject AbilityNotReady = new("{=MFZQek5Upy}You are not ready to use Inspire yet!");

        public static readonly TextObject[] AbilityPhrases =
        {
            new("{=OG3KT8KQvR}A primal bellow echos forth from the depths of your soul!"),
            new("{=0SKEk61Zj0}Your banshee howl pierces the battlefield!"),
            new("{=HT9YJgKEl2}You let out a deafening warcry!"),
            new("{=Nr6docL7MZ}You explode with a thunderous roar!")
        };

        public static readonly TextObject AlliesInspired = new("{=gqr3o3aOfu}Allied unit(s) that were inspired: {INSPIRED}");
        public static readonly TextObject AlliesRestored = new("{=5fapWaqKVw}Allied unit(s) that are returning to battle: {RETURNING}");
        public static readonly TextObject EnemiesScared = new("{=Ldui1l6RMK}Enemy unit(s) that had their courage tested: {SCARED}");
        public static readonly TextObject EnemiesFled = new("{=8aQ7tO6TbA}Enemy unit(s) that are fleeing: {FLED}");
        public static readonly TextObject EnemiesRestored = new("{=zy6xciLnky}Enemy unit(s) that are returning to battle: {RETURNING}");
    }
}