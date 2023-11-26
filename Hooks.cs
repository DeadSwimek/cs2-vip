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
namespace VIP
{
    public partial class VIP
    {
        private HookResult OnPlayerChat(CCSPlayerController? player, CommandInfo info)
        {
            if (Config.EnableVIPPrefix)
            {
                var client = player.EntityIndex!.Value.Value;
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
                }

                var isAlive = player.PawnIsAlive ? "" : "-DEAD-";

                Server.PrintToChatAll(ReplaceTags($"{isAlive} {GetTag} {ChatColors.Red}{player.PlayerName} {ChatColors.Default}: {ChatColors.Lime}{message}"));
                return HookResult.Handled;
            }
            else
            {
                return HookResult.Continue;
            }
        }
        private HookResult OnPlayerChatTeam(CCSPlayerController? player, CommandInfo info)
        {
            if (Config.EnableVIPPrefix)
            {
                var client = player.EntityIndex!.Value.Value;
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
                }

                var isAlive = player.PawnIsAlive ? "" : "-DEAD-";
                for (int i = 1; i <= Server.MaxPlayers; i++)
                {
                    CCSPlayerController? pc = Utilities.GetPlayerFromIndex(i);
                    if (pc == null || !pc.IsValid || pc.IsBot || pc.TeamNum != player.TeamNum) continue;
                    pc.PrintToChat(ReplaceTags($"{isAlive}(TEAM) {GetTag} {ChatColors.Red}{player.PlayerName} {ChatColors.Default}: {ChatColors.Lime}{message}"));
                }
                return HookResult.Handled;
            }
            else
            {
                return HookResult.Continue;
            }
        }
        [GameEventHandler]
        public HookResult OnClientConnect(EventPlayerConnectFull @event, GameEventInfo info)
        {
            CCSPlayerController player = @event.Userid;

            if (player == null || !player.IsValid || player.IsBot)
                return HookResult.Continue;


            var client = player.EntityIndex!.Value.Value;
            Used[client] = 0;
            LastUsed[client] = 0;
            IsVIP[client] = 0;
            ConnectedPlayers++;

            var slots = Server.MaxPlayers;
            slots = slots - Config.ReservedSlotsForVIP;
            if (Config.ReservedMethod == 1)
            {
                if (ConnectedPlayers >= slots)
                {
                    if (IsVIP[client] == 1)
                    {
                        Server.PrintToConsole($"VIP Plugins - Player {player.PlayerName} use the Reservated slot!");
                        return HookResult.Continue;
                    }
                    else
                    {
                        Server.ExecuteCommand($"kickid {player.UserId} 'Server is full, this slot(s) is reserved for VIP!'");
                        Server.PrintToConsole($"VIP Plugins - Player {player.PlayerName} is kicked from the server, bcs slot are for VIP!");
                    }
                }
            }
            if (Config.ReservedMethod == 2)
            {
                if (ConnectedPlayers == Server.MaxPlayers)
                {
                    foreach (var l_player in Utilities.GetPlayers().Where(player => IsVIP[client] == 0 || AdminManager.PlayerHasPermissions(player, "@css/chat")))
                    {
                        var el_player = l_player.EntityIndex!.Value.Value;
                        if (l_player.IsValid)
                        {
                            if (IsVIP[el_player] == 0)
                            {
                                Server.PrintToChatAll($" {Config.Prefix}Player {ChatColors.Lime}{l_player.PlayerName} {ChatColors.Default}has been kicked, bcs {ChatColors.Lime}VIP{ChatColors.Default} need to connect.");
                                Server.ExecuteCommand($"kickid {l_player.UserId} 'VIP Access!'");
                            }
                        }
                    }
                }
            }
              
            LoadPlayerData(player);
            if (IsVIP[client] == 1)
            {
                IsVIP[client] = 1;
                if (Config.WelcomeMessageEnable)
                {
                    player.PrintToChat($" {Config.WelcomeMessage}");
                }
            }

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
            if (GameRules().WarmupPeriod)
            {
                Server.PrintToConsole("VIP Plugins - Warmup dosen't real Round, set on 0.");
                Round = 0;
            }
            else if (GameRules().OvertimePlaying == 1)
            {
                Server.PrintToConsole("VIP Plugins - Overtime dosen't real Round, set on 0.");
                Round = 0;
            }
            else if (GameRules().SwitchingTeamsAtRoundReset)
            {
                Server.PrintToConsole("VIP Plugins - Halftime/switch sites dosen't real Round, set on 0.");
                Round = 0;
            }
            else
            {
                Round++;
                Server.PrintToConsole($"VIP Plugins - Added new round count, now is {ConsoleColor.Yellow} {Round}.");
            }
            Bombplanted = false;
            return HookResult.Continue;
        }
        [GameEventHandler]
        public HookResult OnClientSpawn(EventPlayerSpawn @event, GameEventInfo info)
        {
            CCSPlayerController player = @event.Userid;

            if (player.UserId != null)
            {
                var client = player.EntityIndex!.Value.Value;

                Used[client] = 0;
                Give_Values(player);
                if (LastUsed[client] == 1)
                {
                    if (CheckIsHaveWeapon("ak47", player) == false)
                    {
                        player.GiveNamedItem("weapon_ak47");
                    }
                    player.PrintToChat($" {Config.Prefix} You got automatically AK-47, if you wanna turn off type /guns_off.");
                    Used[client] = 1;
                }
                else if (LastUsed[client] == 2)
                {

                    // Weapons
                    if (CheckIsHaveWeapon("deagle", player) == false)
                    {
                        player.GiveNamedItem("weapon_deagle");
                    }
                    if (CheckIsHaveWeapon("ak47", player) == false)
                    {
                        player.GiveNamedItem("weapon_ak47");
                    }
                    if (CheckIsHaveWeapon("healthshot", player) == false)
                    {
                        player.GiveNamedItem("weapon_healthshot");

                    }
                    // Granades
                    if (CheckIsHaveWeapon("molotov", player) == false)
                    {
                        player.GiveNamedItem("weapon_molotov");
                    }
                    if (CheckIsHaveWeapon("smokegrenade", player) == false)
                    {
                        player.GiveNamedItem("weapon_smokegrenade");
                    }
                    if (CheckIsHaveWeapon("hegrenade", player) == false)
                    {
                        player.GiveNamedItem("weapon_hegrenade");
                    }
                    player.PrintToChat($" {Config.Prefix} You got automatically Pack 2, if you wanna turn off type /guns_off.");
                    Used[client] = 1;
                }
                else if (LastUsed[client] == 3)
                {
                    if (CheckIsHaveWeapon("deagle", player) == false)
                    {
                        player.GiveNamedItem("weapon_deagle");
                    }
                    if (CheckIsHaveWeapon("m4a1", player) == false)
                    {
                        player.GiveNamedItem("weapon_m4a1");
                    }
                    if (CheckIsHaveWeapon("healthshot", player) == false)
                    {
                        player.GiveNamedItem("weapon_healthshot");

                    }
                    // Granades
                    if (CheckIsHaveWeapon("molotov", player) == false)
                    {
                        player.GiveNamedItem("weapon_molotov");
                    }
                    if (CheckIsHaveWeapon("smokegrenade", player) == false)
                    {
                        player.GiveNamedItem("weapon_smokegrenade");
                    }
                    if (CheckIsHaveWeapon("hegrenade", player) == false)
                    {
                        player.GiveNamedItem("weapon_hegrenade");
                    }
                    player.PrintToChat($" {Config.Prefix} You got automatically Pack 1, if you wanna turn off type /guns_off.");
                    Used[client] = 1;
                }
                else if (LastUsed[client] == 4)
                {
                    if (CheckIsHaveWeapon("m4a1", player) == false)
                    {
                        player.GiveNamedItem("weapon_m4a1");
                    }
                    player.PrintToChat($" {Config.Prefix} You got automatically M4A1, if you wanna turn off type /guns_off.");
                    Used[client] = 1;
                }
                else if (LastUsed[client] == 5)
                {
                    if (CheckIsHaveWeapon("m4a1_silencer", player) == false)
                    {
                        player.GiveNamedItem("weapon_m4a1_silencer");
                    }
                    player.PrintToChat($" {Config.Prefix} You got automatically M4A1, if you wanna turn off type /guns_off.");
                    Used[client] = 1;
                }
                //player.PrintToChat($"{Config.Prefix} You can use /ak for give AK47 or /m4 for give M4A1");
            }

            return HookResult.Continue;
        }
        [GameEventHandler]
        public HookResult OnBombPlanted(EventBombPlanted @event, GameEventInfo info)
        {
            var player = @event.Userid;
            AddTimer(35.0f, () =>
            {
                Bombplanted = true;
                Server.PrintToConsole("VIP Plugin - Now you cannot get rewards from kills..");
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
            var attacker_entity = attacker.EntityIndex!.Value.Value;
            var player_entity = player.EntityIndex!.Value.Value;
            if (player == null || !player.IsValid)
                return HookResult.Continue;
            if (attacker == null || !attacker.IsValid)
                return HookResult.Continue;
            if (player != attacker)
            {
                    if (Config.GiveHPAfterKill || Config.GiveMoneyAfterKill)
                    {
                        Server.PrintToChatAll($" {Config.Prefix} Player {ChatColors.Lime}{player.PlayerName}{ChatColors.Default} is killed by {ChatColors.Lime}{attacker.PlayerName}{ChatColors.Default}.");
                    }
                    if (Config.GiveHPAfterKill)
                    {
                        // Sometimes giving, sometimes no, Valve :)
                        PawnValueAttacker.Health += 10;
                        Server.PrintToConsole($"VIP Plugins - Here is bug from valve https://discord.com/channels/1160907911501991946/1160907912445710482/1175583981387927602");
                        attacker.PrintToChat($" {Config.Prefix} You got {ChatColors.Lime}+10 HP{ChatColors.Default} for kill player {ChatColors.LightRed}{player.PlayerName}{ChatColors.Default}, enjoy.");
                        return HookResult.Continue;
                    }
                    if (Config.GiveMoneyAfterKill)
                    {
                        var AttackerMoneys = MoneyValueAttacker.Account;
                        MoneyValueAttacker.Account = AttackerMoneys + 300;
                        attacker.PrintToChat($" {Config.Prefix} You got {ChatColors.Lime}+300 ${ChatColors.Default} for kill player {ChatColors.LightRed}{player.PlayerName}{ChatColors.Default}, enjoy.");
                        return HookResult.Continue;
                    }
                
            }
            return HookResult.Continue;
        }
        [GameEventHandler(HookMode.Post)]

        public HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
        {
            CCSPlayerController player = @event.Userid;
            CCSPlayerController attacker = @event.Attacker;

            var client = player.EntityIndex!.Value.Value;

            if (player.Connected != PlayerConnectedState.PlayerConnected || !player.PlayerPawn.IsValid || !@event.Userid.IsValid)
                return HookResult.Continue;
            if (IsVIP[client] == 0)
            {
                player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.MustBeVIP}");
                return HookResult.Continue;
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
