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
        [ConsoleCommand("addvip", "Add VIP permissions to DB")]
        public void CommandAddVIP(CCSPlayerController? player, CommandInfo info)
        {
            var client = player.EntityIndex!.Value.Value;
            var SteamIDC = info.ArgByIndex(2);
            var TimeSec = info.ArgByIndex(1);
            if (!AdminManager.PlayerHasPermissions(player, "@css/root"))
            {
                player.PrintToChat($" {Config.Prefix} You are not admin..");
                return;
            }
            else if (SteamIDC == null || SteamIDC == "" || !IsInt(SteamIDC))
            {
                player.PrintToChat($" {Config.Prefix} You must add SteamID. Example {ChatColors.Lime}/addvip <Time in seconds> 77777777{ChatColors.Default}, must be added in int.");
                return;
            }
            else if (TimeSec == null || TimeSec == "" || !IsInt(TimeSec))
            {
                player.PrintToChat($" {Config.Prefix} You must add Time in seconds. Example {ChatColors.Lime}/addvip <Time in seconds> 77777777{ChatColors.Default}, must be added in int.");
                player.PrintToChat($" {Config.Prefix} Or if you wanna add forever VIP type {ChatColors.Lime}/addvip 0 77777777{ChatColors.Default}.");

                return;
            }
            else
            {
                var TimeToUTC = DateTime.UtcNow.AddSeconds(Convert.ToInt32(TimeSec)).GetUnixEpoch();


                var timeRemaining = DateTimeOffset.FromUnixTimeSeconds(TimeToUTC) - DateTimeOffset.UtcNow;
                var timeRemainingFormatted =
                $"{timeRemaining.Days}d {timeRemaining.Hours:D2}:{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}";

                MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);
                MySqlQueryValue values = new MySqlQueryValue()
                .Add("steam_id", SteamIDC)
                .Add("end", $"{DateTime.UtcNow.AddSeconds(Convert.ToInt32(TimeSec)).GetUnixEpoch()}");
                MySql.Table("users").Insert(values);
                player.PrintToChat($" {ChatColors.Lime}=========================================");
                player.PrintToChat($" {Config.Prefix} Player with steamid {ChatColors.Lime}{SteamIDC}{ChatColors.Default} has been added.");
                player.PrintToChat($" {Config.Prefix} Ending time is {ChatColors.Lime}{timeRemainingFormatted}{ChatColors.Default}.");
                player.PrintToChat($" {ChatColors.Lime}=========================================");
                Server.PrintToConsole($"VIP Plugin - Admin {player.PlayerName} add new VIP with steamid {SteamIDC}, end time is {timeRemainingFormatted}");

            }
        }
        [ConsoleCommand("ak", "Give a VIP Ak47")]
        public void CommandVIP_ak(CCSPlayerController? player, CommandInfo info)
        {
            var client = player.EntityIndex!.Value.Value;
            if (!player.IsValid || !player.PlayerPawn.IsValid)
            {
                return;
            }
            if (IsVIP[client] == 0)
            {
                player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.MustBeVIP}");
                return;
            }

            if (Round < 3)
            {
                player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.MustBeThird}");
            }
            else
            {
                if (Used[client] == 1)
                {
                    player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.OnceUse}");
                }
                else
                {

                    player.GiveNamedItem("weapon_ak47");
                    player.PrintToChat($" {Config.Prefix} You got AK-47.");
                    Used[client] = 1;
                    LastUsed[client] = 1;
                }
            }
        }
        [ConsoleCommand("pack1", "Give you Pack 1")]
        public void CommandVIP_pack2(CCSPlayerController? player, CommandInfo info)
        {
            var client = player.EntityIndex!.Value.Value;
            if (!player.IsValid || !player.PlayerPawn.IsValid)
            {
                return;
            }
            if (IsVIP[client] == 0)
            {
                player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.MustBeVIP}");
                return;
            }

            if (Round < 3)
            {
                player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.MustBeThird}");
            }
            else
            {
                if (Used[client] == 1)
                {
                    player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.OnceUse}");
                }
                else
                {
                    RemoveWeapons(player);

                    player.GiveNamedItem("weapon_ak47");
                    player.GiveNamedItem("weapon_deagle");
                    player.GiveNamedItem("weapon_healthshot");

                    player.GiveNamedItem("weapon_molotov");
                    player.GiveNamedItem("weapon_smokegrenade");
                    player.GiveNamedItem("weapon_hegrenade");
                    player.PrintToChat($" {Config.Prefix} You got pack number 1.");
                    Used[client] = 1;
                    LastUsed[client] = 2;
                }
            }
        }
        [ConsoleCommand("pack2", "Give you Pack 2")]
        public void CommandVIP_pack1(CCSPlayerController? player, CommandInfo info)
        {
            var client = player.EntityIndex!.Value.Value;
            if (!player.IsValid || !player.PlayerPawn.IsValid)
            {
                return;
            }
            if (IsVIP[client] == 0)
            {
                player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.MustBeVIP}");
                return;
            }

            if (Round < 3)
            {
                player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.MustBeThird}");
            }
            else
            {
                if (Used[client] == 1)
                {
                    player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.OnceUse}");
                }
                else
                {
                    RemoveWeapons(player);

                    player.GiveNamedItem("weapon_m4a1");
                    player.GiveNamedItem("weapon_deagle");
                    player.GiveNamedItem("weapon_healthshot");

                    player.GiveNamedItem("weapon_molotov");
                    player.GiveNamedItem("weapon_smokegrenade");
                    player.GiveNamedItem("weapon_hegrenade");
                    player.PrintToChat($"{Config.Prefix} You got pack number 2.");
                    Used[client] = 1;
                    LastUsed[client] = 3;
                }
            }
        }
        [ConsoleCommand("guns_off", "Turn off giving automaticlly weapons")]
        public void CommandGUNS_off(CCSPlayerController? player, CommandInfo info)
        {
            var client = player.EntityIndex!.Value.Value;
            if (!player.IsValid || !player.PlayerPawn.IsValid)
            {
                return;
            }
            if (IsVIP[client] == 0)
            {
                player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.MustBeVIP}");
                return;
            }
            if (LastUsed[client] >= 1)
            {
                LastUsed[client] = 0;
                player.PrintToCenter($"You turn off automatically weapon..");
            }
        }
        [ConsoleCommand("m4", "Give a VIP M4A1")]
        public void CommandVIP_m4(CCSPlayerController? player, CommandInfo info)
        {
            var client = player.EntityIndex!.Value.Value;
            if (!player.IsValid || !player.PlayerPawn.IsValid)
            {
                return;
            }
            if (IsVIP[client] == 0)
            {
                player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.MustBeVIP}");
                return;
            }

            if (Round < 3)
            {
                player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.MustBeThird}");
            }
            else
            {
                if (Used[client] == 1)
                {
                    player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.OnceUse}");
                }
                else
                {
                    player.GiveNamedItem("weapon_m4a1");
                    player.PrintToChat($" {Config.Prefix} You got M4A1.");
                    Used[client] = 1;
                    LastUsed[client] = 4;


                }
            }
        }
    }
}
