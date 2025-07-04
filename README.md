
##### Lists of my plugins
> [VIP](https://github.com/DeadSwimek/cs2-vip), [VIP Premium](https://github.com/DeadSwimek/cs2-vip-premium), [SpecialRounds](https://github.com/DeadSwimek/cs2-specialrounds), [Countdown](https://github.com/DeadSwimek/cs2-countdown), [CTBans](https://github.com/DeadSwimek/cs2-ctban), [HideAdmin](https://github.com/DeadSwimek/cs2-hideadmin)
> [New Gift Collector!](https://github.com/DeadSwimek/cs2-gifts)


                


**PREMIUM VERSION https://github.com/DeadSwimek/cs2-vip-premium** / [MY DISCORD SERVER](https://discord.gg/WNK777rhwg)
> If you wanna you can support me on this link - **https://www.paypal.com/paypalme/deadswim**
![](https://camo.githubusercontent.com/6f4dcc3ce2ec908ab308be1f42581be46c9bb46cc9958637cc6044f640ed835f/68747470733a2f2f63646e2e646973636f72646170702e636f6d2f6174746163686d656e74732f313137363533373237323732343735383634382f313137363533373237323938303630373133382f7669702e706e67)

### Required
https://github.com/schwarper/CS2MenuManager
https://github.com/schwarper/cs2-tags
https://github.com/schwarper/cs2-store

### Features
- Special VIP tag in chat/score. 
- Pack of guns for free.
- Bonus Credits in shop for kill,plant,defuse
- Set HP on RoundStart, Value can be edited in config file.
- Translation files.
- Connect to database.
- VIPs can be added from keys.
- Configurated packages.
- 3 VIPs groups.
- Guns selector
- Reloading VIP user
- AutoBHOP
- No Knife Damage
- No Fall Damage
- Kill's quake
- Models in database can select by /models
- Tags in database can selected by /settings


# Donators
***GreeNyTM*** Value **200 CZK**

| Command      | Permission   |
| ------------ | ------------ |
| `css_generatevip`    | @css/root     |
| `css_addmodel`    | @css/root     |
| `css_addvip`    | @css/root     |
| `css_addtag`   | @css/root      | 
| `css_guns`    | Database add     |
| `css_vip`    | Database add     |
| `css_settings`    | Database add     |
| `css_models`    | Database add     |
| `css_guns_off`  | NONE   |
| `css_activator`  | NONE   |
| `css_reloadvip` | NONE  |

#Config

```JSON
{
  "Token": "YOUR_TOKEN",
  "Prefix": " \u0001[\u0004MadGames.eu\u0001]",
  "DBDatabase": "database",
  "DBUser": "user",
  "DBPassword": "password",
  "DBHost": "localhost",
  "DBPort": 3306,

  "SettingsCommand": "css_settings",

  "VIPTag_Chat": "{Green} VIP{Grey}\u00BB ",
  "VIPTag_Score": "\u2605",
  "MVIPTag_Chat": "{Green} MVIP{Grey}\u00BB ",
  "MVIPTag_Score": "\u2042",

  "NadeEnable": true,
  "NadeModel": "particles/ui/hud/ui_map_def_utility_trail.vpcf",
  "TrailParticle": "particles/ui/status_levels/ui_status_level_8_energycirc.vpcf",
  // red, green, blue
  "NadeColor": "red",

  "Enable_Credits": true,
  "Credits_Kill": 50,
  "Enable_Credits_Plant": true,
  "Credits_Plant": 50,
  "Enable_Credits_Defuse": true,
  "Credits_Defuse": 50,

  "EnabledTags": true,
  "EnabledBhop": true,
  "EnabledTrials": true,
  "EnabledShotTrials": true,
  "EnabledGuns": true,
  "EnabledFallDamage": true,
  "EnabledNoKnifeDamage": true,
  "EnabledModels": true,
  "EnabledDoubbleJump": true,
  "EnabledInstantReload": true,

  "AllowPreCacheResources": true,

  "EnabledQuake": false,
  "QuakeTeamKiller": "sounds/madgamessounds/quake/teamkiller.vsnd_c",
  "QuakeHeadShot": "sounds/madgamessounds/quake/headshot.vsnd_c",
  "Sounds": [
      {
        "quake_kill": 1,
        "path": "sounds/madgamessounds/quake/firstblood.vsnd_c"
      },
      {
        "quake_kill": 2,
        "path": "sounds/madgamessounds/quake/doublekill.vsnd_c"
      },
      {
        "quake_kill": 3,
        "path": "sounds/madgamessounds/quake/triplekill.vsnd_c"
      }
  ],
  "VIPs": [
    {
      "permission": "healthshot",
      "value": 1
    },
    {
      "permission": "store_credit",
      "value": 1
    },
    {
      "permission": "shotlaser",
      "value": 0
    },
    {
      "permission": "trials",
      "value": 1
    },
    {
      "permission": "nade",
      "value": 0
    },
    {
      "permission": "guns",
      "value": 1
    },
    {
      "permission": "bhop",
      "value": 1
    },
    {
      "permission": "bomb",
      "value": 1
    },
    {
      "permission": "health",
      "value": 1
    },
    {
      "permission": "falldmg",
      "value": 1
    },
    {
      "permission": "knife",
      "value": 1
    },
    {
      "permission": "jump",
      "value": 1
    },
    {
      "permission": "mvip",
      "value": 0
    },
    {
      "permission": "tag",
      "value": 1
    },
    {
      "permission": "reloading",
      "value": 0
    }
  ],

  "StartHealth": 110,

  "MinRoundForGuns": 2,
  "ConfigVersion": 1
}
```

