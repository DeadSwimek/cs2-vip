
##### Lists of my plugins
> [VIP](https://github.com/DeadSwimek/cs2-vip), [VIP Premium](https://github.com/DeadSwimek/cs2-vip-premium), [SpecialRounds](https://github.com/DeadSwimek/cs2-specialrounds), [Countdown](https://github.com/DeadSwimek/cs2-countdown), [CTBans](https://github.com/DeadSwimek/cs2-ctban), [HideAdmin](https://github.com/DeadSwimek/cs2-hideadmin)


                


**PREMIUM VERSION https://github.com/DeadSwimek/cs2-vip-premium** / [MY DISCORD SERVER](https://discord.gg/WNK777rhwg)
> If you wanna you can support me on this link - **https://www.paypal.com/paypalme/deadswim**
![](https://camo.githubusercontent.com/6f4dcc3ce2ec908ab308be1f42581be46c9bb46cc9958637cc6044f640ed835f/68747470733a2f2f63646e2e646973636f72646170702e636f6d2f6174746163686d656e74732f313137363533373237323732343735383634382f313137363533373237323938303630373133382f7669702e706e67)

### Features

- Can enable noknifedamage.
- Special VIP tag in chat. 
- Pack of guns for free.
- Set 115 HP on RoundStart.
- Set 100 Arrmor on RoundStart.
- Welcome messages.
- Translation file in config.
- Connect to database.
- Doubble jump.
- Reservation slots.
- Colored smokes.
- VIPs can be added from keys.
- You can take /testvip.
- VIP can be respawned by command /respawn.
- Rewards after bomb detonate.
- Configurated packages.
- 3 VIPs groups.
- AntiTeam flash
- AutoBHOP


# Donators
***GreeNyTM*** Value **200 CZK**

# Commands
**css_testvip**

`Usage: /testvip` Automatical giving group 0 (GROUP 1)

**css_generatevip**

`Usage: css_generatevip <Time In days> <Group 0,1,2>`

**css_activator**

`Usage: /activator <YOUR_TOKEN>`

**css_addvip**

`Usage: /addvip <Time In Days> <STEAMID64> <Group 0,1,2>`

**css_respawn**

`Usage: /respawn` Respawn a player.

**css_settings**

`Usage: /settings` Open menu with settings

**css_vips**

`Usage: /vips` Type you all online VIPs players.

**css_weapon**

`Usage: /weapon <Number of weapon>` List of packages:

| ID      | Weapon   |
| ------------ | ------------ |
| `1`    | AK-47     |
| `2`    | M4A1     |
| `3`    | M4A1-S     |
| `4`    | AWP     |

**css_pack**

`Usage: /pack <Number of pack>` Configuration in config

**css_guns_off**

`Usage: /guns_off` Turn of automatically weapons giving

| Command      | Permission   |
| ------------ | ------------ |
| `css_generatevip`    | @css/root     |
| `css_addvip`    | @css/root     |
| `css_weapon`    | Database add     |
| `css_respawn`    | Database add     |
| `css_pack`  | Database add   |
| `css_settings`  | Database add   |
| `css_guns_off`  | NONE   |
| `css_activator`  | NONE   |
| `css_testvip`  | NONE   |

#Config

```JSON
{
  "Prefix": " \u0001[\u0004MadGames.eu\u0001]",
  "GiveHPAfterKill": true, // If true, giving rewards for kills (HP)
  "GiveMoneyAfterKill": true, // If ture, giving rewards for kill (Money)
  "AllowKillMessages": true, // If true, send messages after player kill player.
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
  "MinimumRoundToUseCommands": 3, // Minimum round to use VIPs commands
  "DefusedReward": true, // If true, VIPs player got rewards for defuse
  "EnableShowDamage": true, // If true, VIPs players got center message with damage info and player name
  "GroupToVip": "#css/vip", // Name of CSS group what u want add to VIPs players
  "DBDatabase": "database",
  "DBUser": "user",
  "DBPassword": "password",
  "DBHost": "localhost",
  "DBPort": 3306,
  "SpawnItems": [
      "weapon_healthshot",
      "item_assaultsuit"
  ],
  "money": {
    "FirstSpawnMoney": 1200,
    "SpawnArmor": 100,
    "SpawnHP": 110,
    "KillHP": 10,
    "KillMoney": 300,
    "DetonateMoney": 300
  },
  "pack1": {
    "Weapons": [
      "ak47",
      "deagle",
      "healthshot",
      "molotov",
      "smokegrenade",
      "hegrenade"
    ]
  },
  "pack2": {
    "Weapons": [
      "m4a1",
      "deagle",
      "healthshot",
      "molotov",
      "smokegrenade",
      "hegrenade"
    ]
  },
  "pack3": {
    "Allowed": false,
    "Weapons": [
      "m4a1",
      "deagle",
      "healthshot",
      "molotov",
      "smokegrenade",
      "hegrenade"
    ]
  },
  "GroupsNames": {
    "Group1": "VIP",
    "Group2": "VIP II",
    "Group3": "VIP III"
  },
  "CommandOnGroup": {
    "Respawn": 0, // Id of group 0,1 (0 is Group1, 1 is Group2, 2 is Group3)
    "Pack": 0,  // Id of group 0,1 (0 is Group1, 1 is Group2, 2 is Group3)
    "Weapons": 0,  // Id of group 0,1 (0 is Group1, 1 is Group2, 2 is Group3)
    "Acceries": 0,  // Id of group 0,1 (0 is Group1, 1 is Group2, 2 is Group3)
    "Smoke": 0,  // Id of group 0,1 (0 is Group1, 1 is Group2, 2 is Group3)
    "DoubbleJump": 1,  // Id of group 0,1 (0 is Group1, 1 is Group2, 2 is Group3)
    "BombInfo": 0,  // Id of group 0,1 (0 is Group1, 1 is Group2, 2 is Group3)
    "ReservedSlots": 0,  // Id of group 0,1 (0 is Group1, 1 is Group2, 2 is Group3)
    "Flash": 0, //  Id of group 0,1 (0 is Group1, 1 is Group2, 2 is Group3)
    "BHop": 0 //  Id of group 0,1 (0 is Group1, 1 is Group2, 2 is Group3)
  },
  "Messages": {
    "AllowCenterMessages": true // If is true, server print to center messages
  },
  "TestVIP": {
    "EnableTestVIP": true, // If is true, player can take testvip
    "TimeOfVIP": 3600 // Time of Test VIP (Seconds)
  },
  "ConfigVersion": 1
}
```

