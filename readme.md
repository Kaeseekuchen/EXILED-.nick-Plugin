<h1 align="center">SimpleNick</h1>

<p align="center">
  <img src="https://i.imgur.com/aeuFgzb.png" alt="SimpleNick" width="400">
</p>

<h3 align="center">A <a href="https://store.steampowered.com/app/700330/SCP_Secret_Laboratory/">SCP: Secret Laboratory</a> plugin allowing to easily change your/others nicknames.</h3>

## Features

- **Set nicknames**: Players can change their own nickname.  
- **Reset nicknames**: Players can reset their nickname to their original or last saved nickname.  
- **Manage nicknames of other players**: Administrators can change or reset other players' nicknames.  
- **Nickname restriction**: Administrators can prevent specific players from changing their nickname.  
- **Remove nickname restriction**: Administrators can lift nickname restrictions for players.  
- **Persistent blocked players**: Blocked players are stored in a file to keep restrictions across server restarts.  
- **Permission system**: Only players with the appropriate permissions can modify nicknames.  

## Installation

1. Download the latest release of `SimpleNick.dll`.  
2. Place the `.dll` file into the `.config/EXILED/Plugins` folder of your SCP: SL server.  
3. Restart the server.  

## Commands

- `.nick <name>` - Change your nickname.  
- `.nick reset` - Reset your nickname to your original or last saved nickname.  
- `.nick {playerid} <name/reset/exclude/include>` - Manage another player’s nickname.  
  - `reset`: Reset the player's nickname.  
  - `exclude`: Block the player from changing their nickname.  
  - `include`: Allow the player to change their nickname again.  

## Permissions

- `nick.self` - Allows a player to change their own nickname.  
- `nick.manage` - Allows a player to manage other players' nicknames.  

## Configuration

Blocked players are saved in `blockednick.yml` inside the server's `.config/EXILED/Configs` folder.  

**More support can be found on a [Discord](https://discord.gg/UFytdZxeeR) server**
