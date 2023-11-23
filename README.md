# VIPGuns

This is a very simple vipguns plugin for [CounterStrikeSharp](https://docs.cssharp.dev/).
Just install [package](https://github.com/connercsbn/SimpleAdmin/releases/) to `game/csgo/addons/counterstrikesharp/plugins/` and you should be good to go. 

Advantages for VIP:

`-` Can enable noknifedamage.

`-` Special VIP tag in chat.

`-` Pack of guns for free.

`-` Set 115 HP on RoundStart.

`-` Set 100 Arrmor on RoundStart.

`-` Welcome messages.

`-` Translation file in config.

`-` Connect to database.

TODO List:

`-` Add colored skins/model.

`-` Contact me for the premium version.

## css_addvip
`Usage: /addvip <Time In Seconds> <STEAMID64>`
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

## Required Permissions

Permissions using CounterStrikeSharp's [admin framework](https://docs.cssharp.dev/features/admin-framework/)

| Command      | Permission   |
| ------------ | ------------ |
| `css_addvip`    | @css/root     |
| `css_weapon`    | Database add     |
| `css_pack`  | Database add   |
| `css_guns_off`  | NONE   |
