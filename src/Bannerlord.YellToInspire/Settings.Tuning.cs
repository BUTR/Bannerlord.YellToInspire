
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;

namespace Bannerlord.YellToInspire
{
    public sealed partial class Settings
    {
        [SettingPropertyFloatingInteger("{=OSbrbCqXUR}Base Ability Radius", 0f, 100f, "#0 units", Order = 0, RequireRestart = false, HintText = "{=4eLA2GbEcp}Determines the base radius of your yell ability in meters, also the value for Custom Battles.")]
        [SettingPropertyGroup("{=BYgxByyQR5}Tuning", GroupOrder = 3)]
        public float BaseAbilityRadius { get; set; } = 10f;

        [SettingPropertyFloatingInteger("{=p2qRWZ7Ezg}Ability Radius Increase per Level", 0f, 10f, "0.00 units", Order = 1, RequireRestart = false, HintText = "{=f0iSyhJJ9H}The base amount (which is multiplied by attribute and focus bonus) of Leadership exp gained per ally affected by Inspire. Determines how much extra ability radius in meters per level in Leadership.")]
        [SettingPropertyGroup("{=BYgxByyQR5}Tuning", GroupOrder = 3)]
        public float AbilityRadiusIncreasePerLevel { get; set; } = 0.1f;


        [SettingPropertyFloatingInteger("{=xEeTLJBPXY}Base Allied Morale Gain", 0f, 100f, "#0", Order = 2, RequireRestart = false, HintText = "{=lYflwGnkSY}Determines the base anount of how much morale each friendly unit gains per yell, also the value for Custom Battles.")]
        [SettingPropertyGroup("{=BYgxByyQR5}Tuning", GroupOrder = 3)]
        public float BaseAlliedMoraleGain { get; set; } = 0f;

        [SettingPropertyFloatingInteger("{=UIOWxJ8oHq}Allied Morale Gain Increase per Level", 0f, 1f, "0.00", Order = 3, RequireRestart = false, HintText = "{=KFuKPM6vlD}Determines how much extra morale gain per level in Leadership.")]
        [SettingPropertyGroup("{=BYgxByyQR5}Tuning", GroupOrder = 3)]
        public float AlliedMoraleGainIncreasePerLevel { get; set; } = 0.05f;


        [SettingPropertyFloatingInteger("{=MVuF5MdRAd}Leadership Exp per Ally", 0f, 100f, "#0 exp", Order = 4, RequireRestart = false, HintText = "{=DOR2xn2cYG}The base amount (which is multiplied by attribute and focus bonus) of Leadership exp gained per ally affected by Inspire.")]
        [SettingPropertyGroup("{=BYgxByyQR5}Tuning", GroupOrder = 3)]
        public float LeadershipExpPerAlly { get; set; } = 1f;

        [SettingPropertyFloatingInteger("{=mnVFi42L7r}Roguery Exp per Enemy", 0f, 100f, "#0 exp", Order = 5, RequireRestart = false, HintText = "{=z8P8DzRpwc}The base amount (which is multiplied by attribute and focus bonus) of Roguery exp gained per enemy affected by Inspire.")]
        [SettingPropertyGroup("{=BYgxByyQR5}Tuning", GroupOrder = 3)]
        public float RogueryExpPerEnemy { get; set; } = 1f;


        [SettingPropertyFloatingInteger("{=t8iYNduG6l}Base Enemy Chance to Flee", 0f, 1f, "0.00%", Order = 6, RequireRestart = false, HintText = "{=9k0gcHzZiB}The % chance an enemy will flee if affected by a yell and their morale is low enough.")]
        [SettingPropertyGroup("{=BYgxByyQR5}Tuning", GroupOrder = 3)]
        public float BaseEnemyChanceToFlee { get; set; } = 0.5f;

        [SettingPropertyFloatingInteger("{=lqtQwtLcWt}Chance to Flee Increase per Level", 0f, 100f, "#0%", Order = 7, RequireRestart = false, HintText = "{=BadCyQmGTs}Determines how much extra % chance to make enemies flee per level in Roguery. 1/100 of a %.")] //
        [SettingPropertyGroup("{=BYgxByyQR5}Tuning", GroupOrder = 3)]
        public float ChanceToFleeIncreasePerLevel { get; set; } = 15f;


        [SettingPropertyFloatingInteger("{=gvqobBynZ4}Enemy Flee Morale Threshold", 0f, 100f, "#0", Order = 8, RequireRestart = false, HintText = "{=fKTW3QsIOh}Determines how low morale needs to be for an enemy unit to flee.")]
        [SettingPropertyGroup("{=BYgxByyQR5}Tuning", GroupOrder = 3)]
        public float EnemyFleeMoraleThreshold { get; set; } = 15f;



        [SettingPropertyFloatingInteger("{=L5BB55wpAZ}Min Response Delay", 0f, 10f, "0.00 sec", Order = 9, RequireRestart = false, HintText = "{=RyxKXEQgm6}The minimum amount of random time in seconds before a unit responds to your yell.")]
        [SettingPropertyGroup("{=BYgxByyQR5}Tuning", GroupOrder = 3)]
        public float MinResponseDelay { get; set; } = 0.7f;

        [SettingPropertyFloatingInteger("{=ykC6USItRT}Max Response Delay", 0f, 10f, "0.00 sec", Order = 9, RequireRestart = false, HintText = "{=nAwgC95YKZ}The maximum amount of random time in seconds before a unit responds to your yell.")]
        [SettingPropertyGroup("{=BYgxByyQR5}Tuning", GroupOrder = 3)]
        public float MaxResponseDelay { get; set; } = 2.2f;


        //[SettingPropertyBool("{=I7z9HhTKKJ}Fled Enemies Return", Order = 10, RequireRestart = false, HintText = "{=n6ViryinCm}Determines if fleeing enemies return to battle, like a taunt of sorts.")]
        //[SettingPropertyGroup("{=BYgxByyQR5}Tuning", GroupOrder = 3)]
        //public bool FledEnemiesReturn { get; set; } = false;


        [SettingPropertyBool("{=8LnTLK7Eap}Show Sphere Indicators", Order = 11, RequireRestart = false, HintText = "{=pkSxma77ZI}Show sphere showing the area affecting soldiers.")]
        [SettingPropertyGroup("{=BYgxByyQR5}Tuning", GroupOrder = 3)]
        public bool ShowSphereIndicators { get; set; } = true;


        [SettingPropertyBool("{=kDA1RynjWe}Show Detailed Message", Order = 12, RequireRestart = false, HintText = "")]
        [SettingPropertyGroup("{=BYgxByyQR5}Tuning", GroupOrder = 3)]
        public bool ShowDetailedMessage { get; set; } = true;

        [SettingPropertyBool("{=oIjsBtyRi6}Show Detailed Message Text", Order = 13, RequireRestart = false, HintText = "{=qiJ4Uud7qw}Displays message containing a rundown of the number of friendly and enemy units affected per ability use.")]
        [SettingPropertyGroup("{=BYgxByyQR5}Tuning", GroupOrder = 3)]
        public bool ShowDetailedMessageText { get; set; } = true;


        [SettingPropertyBool("{=MBlehjhwnx}Enable Cheer Random Delay", Order = 14, RequireRestart = false, HintText = "{=CTT0d1Q6Q7}Determines if units individually respond to yells with a small random amount of delay.")]
        [SettingPropertyGroup("T{=BYgxByyQR5}uning", GroupOrder = 3)]
        public bool EnableCheerRandomDelay { get; set; } = true;

        [SettingPropertyBool("{=GCpn22ub5U}Enable Cheer Animation", Order = 15, RequireRestart = false, HintText = "{=cPUY6zLpa6}Determines if the player and allies units play cheering animations.")]
        [SettingPropertyGroup("{=BYgxByyQR5}Tuning", GroupOrder = 3)]
        public bool EnableCheerAnimation { get; set; } = true;


        [SettingPropertyBool("{=EYQuao3mup}Enable Enemy Reaction", Order = 16, RequireRestart = false, HintText = "{=YncLuM2Sps}Determines whether enemies verbally respond in fear to your yells.")]
        [SettingPropertyGroup("{=BYgxByyQR5}Tuning", GroupOrder = 3)]
        public bool EnableEnemyReaction { get; set; } = true;
    }
}