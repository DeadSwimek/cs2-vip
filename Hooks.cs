using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Reflection;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Menu;
using static CounterStrikeSharp.API.Core.Listeners;
using System.Runtime.Intrinsics.Arm;
using static System.Runtime.InteropServices.JavaScript.JSType;

using Nexd.MySQL;
using System.Runtime.ExceptionServices;
using CounterStrikeSharp.API.Core.Attributes;
using Microsoft.Extensions.Logging;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Memory;
using System.Security.Cryptography;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;


namespace VIP
{
    public partial class VIP
    {
        private readonly nint Handle;
        public CHandle<CBaseEntity> EndEntity => Schema.GetDeclaredClass<CHandle<CBaseEntity>>(this.Handle, "CBeam", "m_hEndEntity");
        private HookResult OnPlayerChat(CCSPlayerController? player, CommandInfo info)
        {
            if (!Config.EnableVIPPrefix)
            {
                return HookResult.Continue;
            }
            var client = player.Index;
                var message = info.GetArg(1);
                string message_first = info.GetArg(1);

                if (player == null || !player.IsValid || player.IsBot || message == null || message == "")
                    return HookResult.Continue;
                if (message_first.Substring(0, 1) == "/" || message_first.Substring(0, 1) == "!" || message_first.Substring(0, 1) == "rtv")
                    return HookResult.Continue;
                var GetTag = "";
                if (IsVIP[client] == 1)
                {
                    GetTag = $" {ChatColors.Lime}VIP {ChatColors.Default}»";
                    var isAlive = player.PawnIsAlive ? "" : "-DEAD-";

                    Server.PrintToChatAll(ReplaceTags($"{isAlive} {GetTag} {ChatColors.Red}{player.PlayerName} {ChatColors.Default}: {ChatColors.Lime}{message}"));
                }
                else
                {
                return HookResult.Continue;
                }
                return HookResult.Handled;
        }
        private HookResult OnPlayerChatTeam(CCSPlayerController? player, CommandInfo info)
        {
            if (!Config.EnableVIPPrefix)
            {
                return HookResult.Continue;
            }
            var client = player.Index;
                var message = info.GetArg(1);
                string message_first = info.GetArg(1);

                if (player == null || !player.IsValid || player.IsBot || message == null || message == "")
                    return HookResult.Continue;
                if (message_first.Substring(0, 1) == "/" || message_first.Substring(0, 1) == "!" || message_first.Substring(0, 1) == "rtv")
                    return HookResult.Continue;
                var GetTag = "";
                if (IsVIP[client] == 1)
                {
                    GetTag = $" {ChatColors.Lime}VIP {ChatColors.Default}»";
                    var isAlive = player.PawnIsAlive ? "" : "-DEAD-";
                    for (int i = 1; i <= Server.MaxPlayers; i++)
                    {
                        CCSPlayerController? pc = Utilities.GetPlayerFromIndex(i);
                        if (pc == null || !pc.IsValid || pc.IsBot || pc.TeamNum != player.TeamNum) continue;
                        pc.PrintToChat(ReplaceTags($"{isAlive}(TEAM) {GetTag} {ChatColors.Red}{player.PlayerName} {ChatColors.Default}: {ChatColors.Lime}{message}"));
                    }
                }
                else
                {
                    return HookResult.Continue;
                }
            return HookResult.Handled;
        }
        [GameEventHandler]
        public HookResult OnClientConnect(EventPlayerConnectFull @event, GameEventInfo info)
        {
            CCSPlayerController player = @event.Userid;

            if (player == null || !player.IsValid || player.IsBot)
                return HookResult.Continue;


            var client = player.Index;
            Used[client] = 0;
            LastUsed[client] = 0;
            IsVIP[client] = 0;
            HaveGroup[client] = 0;
            ConnectedPlayers++;
            LoadPlayerData(player);

            return HookResult.Continue;
        }
        [GameEventHandler]
        public HookResult OnClientDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
        {
            CCSPlayerController player = @event.Userid;

            if (player == null || !player.IsValid || player.IsBot)
                return HookResult.Continue;

            ConnectedPlayers--;
            return HookResult.Continue;
        }
        [GameEventHandler]
        public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            Server.PrintToConsole("This plugins is created by DeadSwim / https://madgames.eu");

            foreach (var l_player in Utilities.GetPlayers())
            {
                CCSPlayerController player = l_player;
                var client = player.Index;
                if (IsVIP[client] == 1)
                {
                    RespawnUsed[client] = 0;
                }
            }
            if (Config.DisablePackWeaponAfter20Sec)
            {
                AddTimer(20.0f, () =>
                        Disabled20Sec = true
                );
            }

            if (GameRules().WarmupPeriod)
            {
                WriteColor($"VIP Plugin - *[GAMERULES]* Warmup dosen't real Round, set on 0.", ConsoleColor.Yellow);

                Round = 0;
            }
            if (GameRules().OvertimePlaying == 1)
            {
                WriteColor($"VIP Plugin - *[GAMERULES]* Overtime dosen't real Round, set on 0.", ConsoleColor.Yellow);

                Round = 0;
            }
            if (GameRules().SwitchingTeamsAtRoundReset)
            {
                WriteColor($"VIP Plugin - *[GAMERULES]* Halftime/switch sites dosen't real Round, set on 0.", ConsoleColor.Yellow);
                WriteColor($"VIP Plugin - *[GAMERULES]* Restarting rounds number to zero..", ConsoleColor.Yellow);
                WriteColor($"VIP Plugin - *[GAMERULES]* Removing all weapons to players and giving C4, Knife, Glock, HKP2000.", ConsoleColor.Yellow);

                Round = 0;
                DisableGiving = true;
                foreach (var l_player in Utilities.GetPlayers())
                {
                    CCSPlayerController player = l_player;
                    var client = player.Index;
                    
                    if (IsVIP[client] == 1)
                    {
                        LastUsed[client] = 0;
                        int?[] HaveC4 = new int?[65];

                        if (CheckIsHaveWeapon("c4", player) == true)
                        {
                            HaveC4[client] = 1;
                        }
                        RemoveWeapons(player);
                        if(player.TeamNum == ((byte)CsTeam.Terrorist))
                        {
                            player.GiveNamedItem("weapon_glock");
                            if (HaveC4[client] == 1)
                            {
                                player.GiveNamedItem("weapon_c4");
                                HaveC4[client] = 0;
                            }
                        }
                        if (player.TeamNum == ((byte)CsTeam.CounterTerrorist))
                        {
                            player.GiveNamedItem("weapon_hkp2000");
                        }
                    }
                }
            }

            Round++;
            WriteColor($"VIP Plugin - Added new round count, now is [{Round}].", ConsoleColor.Magenta);

            Bombplanted = false;
            Bomb = false;
            Disabled20Sec = false;
            if (Round < 2)
            {
                DisableGiving = false;
            }

            return HookResult.Continue;
        }
        [GameEventHandler]
        public HookResult OnClientSpawn(EventPlayerSpawn @event, GameEventInfo info)
        {
            CCSPlayerController player = @event.Userid;
            if (player == null || !player.IsValid || player.TeamNum == 1)
            {
                return HookResult.Continue;
            }
            else
            {
                var client = player.Index;

                Give_Values(player);
                if (Config.EnableDoubbleJump)
                {
                    if (Config.CommandOnGroup.DoubbleJump > HaveGroup[client])
                    {
                        return HookResult.Continue;
                    }
                    HaveDoubble[client] = 1;
                }
                else
                {
                    HaveDoubble[client] = 0;
                }
                if (Round < Config.MinimumRoundToUseCommands && LastUsed[client] >= 1)
                {
                    player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.MustBeThird}");
                    return HookResult.Stop;
                }
                if (LastUsed[client] >= 1)
                {
                    if (Config.Messages.AllowCenterMessages)
                    {
                        AddTimer(2.0f, () =>
                        player.PrintToCenterHtml($" {Config.TranslationClass.Autoguns}")
                        );
                    }
                    if (!Config.Messages.AllowCenterMessages)
                    {
                        player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.Autoguns}");
                    }
                }
                if (LastUsed[client] == 1)
                {
                    if (CheckIsHaveWeapon("ak47", player) == false)
                    {
                        player.GiveNamedItem("weapon_ak47");
                    }
                    player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.WeaponAK}");
                    Used[client] = 1;
                }
                else if (LastUsed[client] == 2)
                {

                    // Weapons
                    if (CheckIsHaveWeapon($"{Config.Pack1Settings.Pistol}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack1Settings.Pistol}");
                    }
                    if (CheckIsHaveWeapon($"{Config.Pack1Settings.Gun}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack1Settings.Gun}");
                    }
                    if (CheckIsHaveWeapon($"{Config.Pack1Settings.Acceroies}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack1Settings.Acceroies}");

                    }
                    // Granades
                    if (CheckIsHaveWeapon($"{Config.Pack1Settings.Acceroies_2}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack1Settings.Acceroies_2}");
                    }
                    if (CheckIsHaveWeapon($"{Config.Pack1Settings.Acceroies_3}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack1Settings.Acceroies_3}");
                    }
                    if (CheckIsHaveWeapon($"{Config.Pack1Settings.Acceroies_4}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack1Settings.Acceroies_4}");
                    }
                    player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.Pack1}");
                    Used[client] = 1;
                }
                else if (LastUsed[client] == 3)
                {
                    if (CheckIsHaveWeapon($"{Config.Pack2Settings.Pistol}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack2Settings.Pistol}");
                    }
                    if (CheckIsHaveWeapon($"{Config.Pack2Settings.Gun}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack2Settings.Gun}");
                    }
                    if (CheckIsHaveWeapon($"{Config.Pack2Settings.Acceroies}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack2Settings.Acceroies}");

                    }
                    // Granades
                    if (CheckIsHaveWeapon($"{Config.Pack2Settings.Acceroies_2}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack2Settings.Acceroies_2}");
                    }
                    if (CheckIsHaveWeapon($"{Config.Pack2Settings.Acceroies_3}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack2Settings.Acceroies_3}");
                    }
                    if (CheckIsHaveWeapon($"{Config.Pack2Settings.Acceroies_4}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack2Settings.Acceroies_4}");
                    }
                    player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.Pack2}");
                    Used[client] = 1;
                }
                else if (LastUsed[client] == 10)
                {
                    if (CheckIsHaveWeapon($"{Config.Pack3Settings.Pistol}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack3Settings.Pistol}");
                    }
                    if (CheckIsHaveWeapon($"{Config.Pack3Settings.Gun}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack3Settings.Gun}");
                    }
                    if (CheckIsHaveWeapon($"{Config.Pack3Settings.Acceroies}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack3Settings.Acceroies}");

                    }
                    // Granades
                    if (CheckIsHaveWeapon($"{Config.Pack3Settings.Acceroies_2}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack3Settings.Acceroies_2}");
                    }
                    if (CheckIsHaveWeapon($"{Config.Pack3Settings.Acceroies_3}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack3Settings.Acceroies_3}");
                    }
                    if (CheckIsHaveWeapon($"{Config.Pack3Settings.Acceroies_4}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{Config.Pack3Settings.Acceroies_4}");
                    }
                    player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.Pack3}");
                    Used[client] = 1;
                }
                else if (LastUsed[client] == 4)
                {
                    if (CheckIsHaveWeapon("m4a1", player) == false)
                    {
                        player.GiveNamedItem("weapon_m4a1");
                    }
                    player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.WeaponM4A1}");
                    Used[client] = 1;
                }
                else if (LastUsed[client] == 5)
                {
                    if (CheckIsHaveWeapon("m4a1_silencer", player) == false)
                    {
                        player.GiveNamedItem("weapon_m4a1_silencer");
                    }
                    player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.WeaponM4A1S}");
                    Used[client] = 1;
                }
                else if (LastUsed[client] == 6)
                {
                    if (CheckIsHaveWeapon("awp", player) == false)
                    {
                        player.GiveNamedItem("weapon_awp");
                    }
                    player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.WeaponAWP}");
                    Used[client] = 1;
                }
                //player.PrintToChat($"{Config.Prefix} You can use /ak for give AK47 or /m4 for give M4A1");
            }
            return HookResult.Continue;
        }
        [GameEventHandler]
        public HookResult OnBombPlanted(EventBombPlanted @event, GameEventInfo info)
        {
            var c4list = Utilities.FindAllEntitiesByDesignerName<CC4>("weapon_c4");
            var c4 = c4list.FirstOrDefault();
            var planted = new CBombTarget(NativeAPI.GetEntityFromIndex(@event.Site));


            var player = @event.Userid;
            Bomb = true;
            bombtime = 40.0f;
            if (Config.Bombinfo)
            {
                if (Config.CommandOnGroup.BombInfo > get_vip_group(player))
                {
                    return HookResult.Continue;
                }
                if (planted.IsBombSiteB)
                {
                    SitePlant = "B";
                }
                else
                {
                    SitePlant = "A";
                }
                var timer = AddTimer(1.0f, () =>
                {
                    if (bombtime == 0)
                    {
                        return;
                    }

                    bombtime = bombtime - 1.0f;
                }, TimerFlags.REPEAT);
            }
            AddTimer(35.0f, () =>
            {
                Bombplanted = true;
                WriteColor("VIP Plugin - *[WARNING]* Now players canno't get rewards form kills..", ConsoleColor.Yellow);
                if (Config.DetonateRewards)
                {
                    var moneyServices = player.InGameMoneyServices;
                    moneyServices.Account = moneyServices.Account + Config.RewardsClass.DetonateMoney;
                }
            });

            return HookResult.Continue;
        }
        [GameEventHandler]
        public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            if(Bombplanted == true)
            {
                return HookResult.Stop;
            }
            CCSPlayerController player = @event.Userid;
            CCSPlayerController attacker = @event.Attacker;
            var PawnValueAttacker = attacker.PlayerPawn.Value;
            var MoneyValueAttacker = attacker.InGameMoneyServices;
            var attacker_entity = attacker.Index;
            var player_entity = player.Index;
            if (player == null || !player.IsValid)
                return HookResult.Continue;
            if (attacker == null || !attacker.IsValid)
                return HookResult.Continue;
            if (IsVIP[attacker_entity] == 0)
                return HookResult.Continue;
            if (player != attacker)
            {
                

                if (Config.GiveHPAfterKill || Config.GiveMoneyAfterKill)
                    {
                        if (Config.AllowKillMessages)
                        {
                            Server.PrintToChatAll($" {Config.Prefix} Player {ChatColors.Lime}{player.PlayerName}{ChatColors.Default} is killed by {ChatColors.Lime}{attacker.PlayerName}{ChatColors.Default}.");
                        }
                    }
                    if (Config.GiveMoneyAfterKill)
                    {
                        var AttackerMoneys = MoneyValueAttacker.Account;
                        MoneyValueAttacker.Account = AttackerMoneys + Config.RewardsClass.KillMoney;
                        attacker.PrintToChat($" {Config.Prefix} You got {ChatColors.Lime}+{Config.RewardsClass.KillMoney} ${ChatColors.Default} for kill player {ChatColors.LightRed}{player.PlayerName}{ChatColors.Default}, enjoy.");
                    }
                    if (Config.GiveHPAfterKill)
                    {
                        var health_attacker = attacker.PlayerPawn.Value.Health;
                        @event.Attacker.PlayerPawn.Value.Health = @event.Attacker.PlayerPawn.Value.Health + Config.RewardsClass.KillHP;
                        Server.PrintToConsole($"VIP Plugins - Here is bug from valve https://discord.com/channels/1160907911501991946/1160907912445710482/1175583981387927602");
                        attacker.PrintToChat($" {Config.Prefix} You got {ChatColors.Lime}+{Config.RewardsClass.KillHP} HP{ChatColors.Default} for kill player {ChatColors.LightRed}{player.PlayerName}{ChatColors.Default}, enjoy.");
                    }

            }
            return HookResult.Continue;
        }
        [GameEventHandler(HookMode.Post)]

        public HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
        {
            CCSPlayerController player = @event.Userid;
            CCSPlayerController attacker = @event.Attacker;

            var client = player.Index;


            if (player.Connected != PlayerConnectedState.PlayerConnected || !player.PlayerPawn.IsValid || !@event.Userid.IsValid)
                return HookResult.Continue;
            if (IsVIP[client] == 0)
            {
                return HookResult.Continue;
            }
            if (!Config.EnableFalldamage)
            {
                if (@event.Hitgroup == 0 && @event.Weapon != "inferno" && @event.Weapon != "hegrenade" && @event.Weapon != "knife")
                {
                    if (@event.DmgHealth >= 100)
                    {
                        WriteColor($"VIP Plugin - *[WARNING]* Player {player.PlayerName} died on map drop or bomb detonate (Weapon: {@event.Weapon} HiTG: {@event.Hitgroup}).", ConsoleColor.Yellow);

                    }
                    else
                    {
                        player.PlayerPawn.Value.Health = player.PlayerPawn.Value.Health += @event.DmgHealth;
                        WriteColor($"VIP Plugin - *[WARNING]* Player {player.PlayerName} registred a FallDamage (Weapon: {@event.Weapon} HiTG: {@event.Hitgroup})!", ConsoleColor.Yellow);
                    }
                }
            }
            if (Config.KnifeDMGEnable)
            {
                if (@event.Weapon == "knife")
                {
                    if (@event.Userid.PlayerPawn.Value.Health + @event.DmgHealth <= 100)
                    {
                        @event.Userid.PlayerPawn.Value.Health = @event.Userid.PlayerPawn.Value.Health += @event.DmgHealth;
                        if (@event.Attacker.IsValid)
                        {
                            attacker.PrintToChat($" {Config.Prefix} You canno't hit {ChatColors.Lime}VIP {ChatColors.Default}player with Knife!");
                        }
                    }
                    else
                    {
                        @event.Userid.PlayerPawn.Value.Health = 100;
                    }
                }
            }
            @event.Userid.PlayerPawn.Value.VelocityModifier = 1;
            return HookResult.Continue;
        }
    }
}
