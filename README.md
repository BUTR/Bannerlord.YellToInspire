# Bannerlord.YellToInspire
<p align="center">
  <a href="https://github.com/BUTR/Bannerlord.YellToInspire" alt="Lines Of Code">
    <img src="https://aschey.tech/tokei/github/BUTR/Bannerlord.YellToInspire?category=code" />
  </a>
  <a href="https://www.codefactor.io/repository/github/butr/bannerlord.yelltoinspire">
    <img src="https://www.codefactor.io/repository/github/butr/bannerlord.yelltoinspire/badge" alt="CodeFactor" />
  </a>
  <a href="https://codeclimate.com/github/BUTR/Bannerlord.YellToInspire/maintainability">
    <img alt="Code Climate maintainability" src="https://img.shields.io/codeclimate/maintainability-percentage/BUTR/Bannerlord.YellToInspire">
  </a>
  <a title="Crowdin" target="_blank" href="https://crowdin.com/project/yell-to-inspire">
    <img src="https://badges.crowdin.net/yell-to-inspire/localized.svg">
  </a>
  </br>
  <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/3638" alt="NexusMods YellToInspire">
    <img src="https://img.shields.io/badge/NexusMods-YellToInspire-yellow.svg" />
  </a>
  <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/3638" alt="NexusMods YellToInspire">
    <img src="https://img.shields.io/endpoint?url=https%3A%2F%2Fnexusmods-version-pzk4e0ejol6j.runkit.sh%3FgameId%3Dmountandblade2bannerlord%26modId%3D3638" />
  </a>
  <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/3638" alt="NexusMods YellToInspire">
    <img src="https://img.shields.io/endpoint?url=https%3A%2F%2Fnexusmods-downloads-ayuqql60xfxb.runkit.sh%2F%3Ftype%3Dunique%26gameId%3D3174%26modId%3D3638" />
  </a>
  <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/3638" alt="NexusMods YellToInspire">
    <img src="https://img.shields.io/endpoint?url=https%3A%2F%2Fnexusmods-downloads-ayuqql60xfxb.runkit.sh%2F%3Ftype%3Dtotal%26gameId%3D3174%26modId%3D3638" />
  </a>
  <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/3638" alt="NexusMods YellToInspire">
    <img src="https://img.shields.io/endpoint?url=https%3A%2F%2Fnexusmods-downloads-ayuqql60xfxb.runkit.sh%2F%3Ftype%3Dviews%26gameId%3D3174%26modId%3D3638" />
  </a>
  </br>
  <a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2884365680">
    <img alt="Steam Yell To Inspire" src="https://img.shields.io/badge/Steam-Yell%20To%20Inspire-blue.svg" />
  </a>
  <a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2884365680">
    <img alt="Steam Downloads" src="https://img.shields.io/steam/downloads/2884365680?label=Downloads&color=blue">
  </a>
  <a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2884365680">
    <img alt="Steam Views" src="https://img.shields.io/steam/views/2884365680?label=Views&color=blue">
  </a>
  <a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2884365680">
    <img alt="Steam Subscriptions" src="https://img.shields.io/steam/subscriptions/2884365680?label=Subscriptions&color=blue">
  </a>
  <a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2884365680">
    <img alt="Steam Favorites" src="https://img.shields.io/steam/favorites/2884365680?label=Favorites&color=blue">
  </a>
  </br>
  <img src="https://staticdelivery.nexusmods.com/mods/3174/images/3638/3638-1642520726-61812109.png" width="800">
</p>

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
