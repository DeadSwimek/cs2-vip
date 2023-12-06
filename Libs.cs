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
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CSTimer = CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Admin;
using System.Drawing;
using System.Globalization;

namespace VIP
{
    public partial class VIP
    {
        static void WriteColor(string message, ConsoleColor color)
        {
            var pieces = Regex.Split(message, @"(\[[^\]]*\])");

            for (int i = 0; i < pieces.Length; i++)
            {
                string piece = pieces[i];

                if (piece.StartsWith("[") && piece.EndsWith("]"))
                {
                    Console.ForegroundColor = color;
                    piece = piece.Substring(1, piece.Length - 2);
                }

                Console.Write(piece);
                Console.ResetColor();
            }

            Console.WriteLine();
        }
        static public bool is_vip(CCSPlayerController? player)
        {
            if(player == null)
            {
                Server.PrintToConsole($"VIP Plugin - [ERROR] Canno't get a player");
                return false;
            }
            var client = player.Index;
            if (IsVIP[client] == 0)
            {
                return false;
            }
            return true;
        }
        static public void GiveItem(CCSPlayerController? player, string Item)
        {
            if (player.IsValid || player.PawnIsAlive)
            {
                player.GiveNamedItem(Item);
                WriteColor($"VIP Plugin - [*SUCCESS*] Giving item {Item} to player {player.PlayerName}", ConsoleColor.Green);
            }
            else
            {
                WriteColor($"VIP Plugin - [*ERROR - GiveItem *] player {player.PlayerName} is not valid or alive!", ConsoleColor.Red);
            }
        }
        static public int get_vip_group(CCSPlayerController? player)
        {
            if(player == null)
            {
                WriteColor("VIP Plugin - *[ERROR - get_vip_group]* Cannot get a player index..", ConsoleColor.Red);
                return 100;
            }
            var client = player.Index;

            if (HaveGroup[client] == null)
            {
                WriteColor("VIP Plugin - *[ERROR  - get_vip_group]* Cannot get a player index..", ConsoleColor.Red);
                return 100;
            }
            if (IsVIP[client] == 0)
            {
                return -1;
            }
            if (HaveGroup[client] == 0)
            {
                return 0;
            }
            if (HaveGroup[client] == 1)
            {
                return 1;
            }
            if (HaveGroup[client] == 2)
            {
                return 2;
            }
            return 100;
        }
        
        public string get_name_group(CCSPlayerController? player)
        {
            if (player == null)
            {
                WriteColor("VIP Plugin - *[ERROR - get_name_group]* Cannot get a player index..", ConsoleColor.Red);
                return "ERROR";
            }
            if (get_vip_group(player) == 0)
            {
                WriteColor($"VIP Plugin - *[SUCCESS]* Set to player {player.PlayerName} VIP group [{Config.GroupsNames.Group1}]", ConsoleColor.Green);
                return Config.GroupsNames.Group1;
            }
            else if (get_vip_group(player) == 1)
            {
                WriteColor($"VIP Plugin - *[SUCCESS]* Set to player {player.PlayerName} VIP group [{Config.GroupsNames.Group2}]", ConsoleColor.Green);
                return Config.GroupsNames.Group2;
            }
            else if (get_vip_group(player) == 2)
            {
                WriteColor($"VIP Plugin - *[SUCCESS]* Set to player {player.PlayerName} VIP group [{Config.GroupsNames.Group3}]", ConsoleColor.Green);
                return Config.GroupsNames.Group3;
            }
            WriteColor("VIP Plugin - *[ERROR  - get_name_group]* Cannot get a player index..", ConsoleColor.Red);
            return "Not Exist";
        }
        static public int get_hp(CCSPlayerController? player)
        {
            if (player == null || !player.PawnIsAlive)
            {
                return 100;
            }
            return player.PlayerPawn.Value.Health;
        }
        static public int get_money(CCSPlayerController? player)
        {
            if (player == null || !player.PawnIsAlive)
            {
                return 100;
            }
            return player.InGameMoneyServices.Account;
        }
        static public bool is_alive (CCSPlayerController? player)
        {
            if (!player.PawnIsAlive)
            {
                return false;
            }
            else
            {
                return true;
            }
            return false;
        }
        static public void set_hp (CCSPlayerController? player, int hp)
        {
            if (player == null || !player.PawnIsAlive)
            {
                return;
            }
            player.PlayerPawn.Value.Health = hp;
            Server.PrintToConsole($"Iam setting up {hp} to player {player.PlayerName}");
        }
        static public void set_armor(CCSPlayerController? player, int armor)
        {
            if (player == null || !player.PawnIsAlive)
            {
                return;
            }
            player.PlayerPawn.Value.ArmorValue = armor;
        }
        static public void set_money(CCSPlayerController? player, int money)
        {
            if (player == null || !player.PawnIsAlive)
            {
                return;
            }
            player.InGameMoneyServices.Account = money;
        }
    }
}
