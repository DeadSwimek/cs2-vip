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


namespace VIP
{
    public partial class VIP
    {
        static public bool is_vip(CCSPlayerController? player)
        {
            if(player == null)
            {
                Server.PrintToConsole($"VIP Plugin - {ConsoleColor.Red}[ERROR] Canno't get a player");
                return false;
            }
            var client = player.EntityIndex!.Value.Value;
            if (IsVIP[client] == 0)
            {
                return false;
            }
            return true;
        }
        static public int get_vip_group(CCSPlayerController? player)
        {
            if(player == null)
            {
                Server.PrintToConsole($"VIP Plugin - {ConsoleColor.Red}[ERROR] Canno't get a player");
                return -1;
            }
            var client = player.EntityIndex!.Value.Value;
            if (IsVIP[client] == 0)
            {
                return -1;
            }
            var vip_group = HaveGroup[client];
            return (int)vip_group;
        }
        
        public string get_name_group(CCSPlayerController? player)
        {
            if (player == null)
            {
                Server.PrintToConsole($"VIP Plugin - {ConsoleColor.Red}[ERROR] Canno't get a player");
                return "ERROR";
            }
            if (get_vip_group(player) == 0)
            {
                return Config.GroupsNames.Group1;
            }
            else if (get_vip_group(player) == 1)
            {
                return Config.GroupsNames.Group2;
            }
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
            if (player == null || !player.PawnIsAlive)
            {
                return false;
            }
            return true;
        }
        static public void set_hp (CCSPlayerController? player, int hp)
        {
            if (player == null || !player.PawnIsAlive)
            {
                return;
            }
            player.PlayerPawn.Value.Health += hp;
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
