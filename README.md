# Bannerlord.YellToInspire

Rewritten several times and server as an example of a gameplay mod.  
  
Several points:  
* The game follows a pattern where an `AgentComponent` holds a state and the necessary functions to alter the state.
`CommonAIComponent` is an example of that. Other components reference it and alter. We are doing the same, having a state and two
`AgentComponent` that handle AI and Player interactions.  
* From my understanding, the game devs didn't intend to handle Player input in the `AgentComponent` `Tick` method, as `OnTickAsAI`
will be triggered on non Player controlled Agents. My workaround is to create a `MissionBehavior` that checks if the 
`AgentComponent` implements `IAgentComponentOnTick` and calls `OnTick`. From what I understand, the Player input
should be handled in `MissionBehavior`, but it's inconsistent then with the AI handling.  
* A more interesting way to get MCM settings is to inject a `MissionBehavior` with a method to get the settings,
thus avoiding the static access, which is in general a good thing.  
* HotKey handling via the game intended way is interesting. ButterLib only provides static endpoints, because of that we need
`Class.Current` properties to alter state from the HotKey. The game intended way gives the ability to alter the state
directly from the instances of `MissionBehavior` and `AgentComponent`.  
* `module_strings.xml` was really hard to implement. There's a lot of undocumented behavior you need to find to provide
localization support fot game provided HotKeys.
* `ModuleInfoHelper.GetModuleByType` gives a great opportunity to get the assemblies Module Id without hardcoding it.
* You need to add `_RGL_KEEP_ASSERTS` `DefineConstants` to keep using the `MBDebug` class.
* You can't really make your mod 'modder friendly'. I assume that the best would be to make public all classes that
expose logic that could be modifiable, mark all properties and methods virtual, replace all private members as protected.
I don't think that you should expose implementation detail classes as long as they won't interfere with the abiility to override code.
  
It has an interesting history. First based on [Xorberax's Press V to Yell](https://www.nexusmods.com/mountandblade2bannerlord/mods/154),
then reimagined with [Yell To Inspire - New Leadership Perks and Abilities](https://www.nexusmods.com/mountandblade2bannerlord/mods/477),
then ported [Yell To Inspire (Updated)](https://www.nexusmods.com/mountandblade2bannerlord/mods/2155) and now having an
'enterprise grade rewrite ©' with the addition of usage of MCM.  
