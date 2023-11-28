
# If you wanna you can support me on this link - https://www.paypal.com/paypalme/deadswim

![image](https://cdn.discordapp.com/attachments/1176537272724758648/1176537272980607138/vip.png)

This is a very simple vipguns plugin for [CounterStrikeSharp](https://docs.cssharp.dev/).
Just install [package](https://github.com/connercsbn/SimpleAdmin/releases/) to `game/csgo/addons/counterstrikesharp/plugins/` and you should be good to go. 

### Videos/Images shows

[Detonation viewer](https://madgames.eu/YcXJ.webm) [/vip Design](https://cdn.discordapp.com/attachments/1140558464599470170/1178973482843906098/image.png)

Advantages for VIP:

`-` Can enable noknifedamage.

`-` Special VIP tag in chat.

`-` Pack of guns for free.

`-` Set 115 HP on RoundStart.

`-` Set 100 Arrmor on RoundStart.

`-` Welcome messages.

`-` Translation file in config.

`-` Connect to database.

`-` Doubble jump.

`-` Reservation slots.

`-` Colored smokes.

`-` VIPs can be added from keys.

`-` You can take /testvip.

`-` VIP can be respawned by command /respawn.

TODO List:

`-` Add AutoBhop for VIP.

`-` Add colored skins/model.

`-` Contact me for the premium version.

## css_testvip
`Usage: /testvip`
## css_generatevip
`Usage: css_generatevip <Time In Seconds>`
## css_activator
`Usage: /activator <YOUR_TOKEN>`
## css_addvip
`Usage: /addvip <Time In Seconds> <STEAMID64>`
## css_respawn
`Usage: /respawn`
Give a VIP to player on steamid.
## css_weapon
`Usage: /weapon <Number of weapon>`
List of packages:

1 - AK-47

2 - M4A1

3 - M4A1-S

## css_pack
`Usage: /pack <Number of pack>`
List of packages:

1 - AK-47, Deagle, Healthshot, Molotov, Smoke, He

2 - M4A1, Deagle, Healthshot, Molotov, Smoke, He

## css_guns_off
`Usage: /guns_off`
Turn of automatically weapons giving

```
{
  "Prefix": " \u0001[\u0004MadGames.eu\u0001]",
  "GiveHPAfterKill": true, // If true, giving rewards for kills (HP)
  "GiveMoneyAfterKill": true, // If ture, giving rewards for kill (Money)
  "EnableVIPPrefix": true, // If true, enable VIP tag
  "EnableVIPAcceries": true, // If true, enable start with 1200$, 100 Armor and 110 HP.
  "EnableVIPColoredSmokes": true, // If true, enable colored smokes for VIP
  "EnableFalldamage": false, // If false, VIP dosen't giving fall damage
  "RespawnAllowed": true, // If true, enable command to /respawn
  "DetonateRewards": true, // If true, VIPs player get rewards for detonation
  "EnableDoubbleJump": true, // If true, give VIP player doubble jump
  "KnifeDMGEnable": false, // If false, VIP player dosen't give DMG from knife
  "WelcomeMessageEnable": true, // If true, type in the chat welcome message
  "ReservedSlotsForVIP": 1, // Work only with reservedmethod1, reservation slots from max players
  "ReservedMethod": 1, // 0 = Disable, 1 = Reservation slots from max player, 2 = kicking non vip players
  "Bombinfo": true, // If true, show for VIPs players detonation time
  "DisablePackWeaponAfter20Sec": false, // If false, VIPs players can take anytime guns/packs
  "WelcomeMessage": "Welcom on server you are BEST VIP!", 
  "DBDatabase": "database",
  "DBUser": "user",
  "DBPassword": "password",
  "DBHost": "localhost",
  "DBPort": 3306,
  "translation": {
    "OnceUse": " This command you can use \u0007only once\u0001 on round!",
    "MustBeVIP": " This command are allowed only for \u0006VIP\u0001!",
    "MustBeThird": " Must be a \u0007Third\u0001 round, to use this command!",
    "Pack1": " You got a Packages number one.",
    "Pack2": " You got a Packages number two.",
    "WeaponAK": " You got a weapon AK-47.",
    "WeaponM4A1": " You got a weapon M4A1.",
    "WeaponM4A1S": " You got a weapon M4A1-S.",
    "WeaponAWP": " You got a weapon AWP.",
    "Autoguns": " \u003Cfont color:\u0027green\u0027\u003EIf you wanna turn off automaticall weapon type\u003C/font\u003E\u003Cfont color:\u0027red\u0027\u003E /guns_off\u003C/font\u003E",
    "MustFirst20Sec": " You can use this command only in \u0007first 20 Seconds\u0001.",
    "MustBeAlive": " You can use this command only when \u0007you are alive\u0001!"
  },
  "money": {
    "FirstSpawnMoney": 1200,
    "SpawnArmor": 100,
    "SpawnHP": 110,
    "KillHP": 10,
    "KillMoney": 300,
    "DetonateMoney": 300
  },
  "pack1": {
    "Gun": "ak47",
    "Pistol": "deagle",
    "Acceroies": "healthshot",
    "Acceroies_2": "molotov",
    "Acceroies_3": "smokegrenade",
    "Acceroies_4": "hegrenade"
  },
  "pack2": {
    "Gun": "m4a1",
    "Pistol": "deagle",
    "Acceroies": "healthshot",
    "Acceroies_2": "molotov",
    "Acceroies_3": "smokegrenade",
    "Acceroies_4": "hegrenade"
  },
  "ConfigVersion": 1
}
```

## Required Permissions

Permissions using CounterStrikeSharp's [admin framework](https://docs.cssharp.dev/features/admin-framework/)

| Command      | Permission   |
| ------------ | ------------ |
| `css_generatevip`    | @css/root     |
| `css_addvip`    | @css/root     |
| `css_weapon`    | Database add     |
| `css_respawn`    | Database add     |
| `css_pack`  | Database add   |
| `css_guns_off`  | NONE   |
| `css_activator`  | NONE   |
| `css_testvip`  | NONE   |
