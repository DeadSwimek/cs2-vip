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
using System.Threading;

namespace VIP
{
    public partial class VIP
    {
        [ConsoleCommand("css_settings", "User settings")]
        public void opensettings(CCSPlayerController? player, CommandInfo info)
        {
            // Check if player is null
            if (player == null || IsVIP[player.Index] != 1)
            {
                return;
            }

            settings_menu_open(player);
        }
        [ConsoleCommand("css_addfakeplayer", "Add fake client for testing reservation slots")]
        public void addfakeplayer(CCSPlayerController? player, CommandInfo info)
        {
            if (AdminManager.PlayerHasPermissions(player, "@css/root"))
            {
                ConnectedPlayers++;
                Server.PrintToConsole($"You successful add new fake player on server/debugmode. Now are connected players {ConnectedPlayers}");
            }
        }
        [ConsoleCommand("css_addfakeround", "Add fake round for testing weapons")]
        public void addfakeround(CCSPlayerController? player, CommandInfo info)
        {
            if (AdminManager.PlayerHasPermissions(player, "@css/root"))
            {
                Round = 5;
                Server.PrintToConsole($"You successful add new round on server/debugmode. Now are round {Round}");
            }
        }
        [ConsoleCommand("css_testvip", "Test VIP")]
        public void CommandTESTVIP(CCSPlayerController? player, CommandInfo info)
        {
            if (player == null || !player.IsValid)
            {
                // Handle invalid player
                return;
            }

            if (Config.TestVIP.EnableTestVIP)
            {
                MySqlDb? mySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);

                if (mySql == null)
                {
                    // Handle MySQL initialization failure
                    return;
                }

                MySqlQueryResult result = mySql.Table($"{Config.DBPrefix}_users_test_vip")
                    .Where(MySqlQueryCondition.New("steam_id", "=", player.SteamID.ToString()))
                    .Select();

                if (result.Rows == 0)
                {
                    int TimeSec = Config.TestVIP.TimeOfVIP;
                    var TimeToUTC = DateTime.UtcNow.AddSeconds(Convert.ToInt32(TimeSec)).GetUnixEpoch();
                    var timeofvip = DateTime.UtcNow.AddSeconds(Convert.ToInt32(TimeSec)).GetUnixEpoch();
                    MySqlQueryValue _Tvalues = new MySqlQueryValue()
                        .Add("steam_id", $"{player.SteamID}")
                        .Add("used", $"{timeofvip}")
                        .Add("`group`", $"0");

                    mySql.Table($"{Config.DBPrefix}_users_test_vip").Insert(_Tvalues);

                    var timeRemaining = DateTimeOffset.FromUnixTimeSeconds(TimeToUTC) - DateTimeOffset.UtcNow;
                    var timeRemainingFormatted =
                        $"{timeRemaining.Days}d {timeRemaining.Hours:D2}:{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}";

                    MySqlQueryValue values = new MySqlQueryValue()
                        .Add("steam_id", $"{player.SteamID}")
                        .Add("end", $"{timeofvip}")
                        .Add("`group`", $"0");

                    mySql.Table($"{Config.DBPrefix}_users").Insert(values);
                    var client = player.Index;
                    LoadPlayerData(player);

                    player.PrintToChat($" {Config.Prefix} You have activated {ChatColors.Red}TEST VIP. {ChatColors.Default} Ending in: {ChatColors.Red}{timeRemainingFormatted}{ChatColors.Default}.");
                    Server.PrintToConsole($"VIP Plugin - Player {player.PlayerName} used TEST VIP with steamid {player.SteamID}, end time is {timeRemainingFormatted}");
                }
                else
                {
                    var timeRemaining = DateTimeOffset.FromUnixTimeSeconds(result.Get<int>(0, "used")) - DateTimeOffset.UtcNow;
                    var timeRemainingFormatted =
                        $"{timeRemaining.Days}d {timeRemaining.Hours:D2}:{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}";

                    player.PrintToChat($" {Config.Prefix} You already used VIP Test.");
                }
            }
            else
            {
                player.PrintToChat($" {Config.Prefix} Test VIP is {ChatColors.Red}disabled{ChatColors.Default}.");
            }
        }
        [ConsoleCommand("css_brvip", "Activate VIP from Tokens")]
        public void CommandActivator(CCSPlayerController? player, CommandInfo info)
        {
            try
            {
                var token = info.ArgByIndex(1);
                if (token == null || token == "" || IsInt(token))
                    return;

                // Graceful degradation for invalid player
                if (player == null || !player.IsValid)
                {
                    // Log the error and provide a default behavior or skip further processing
                    Logger.LogError("Invalid player in CommandActivator");
                    return;
                }

                if (is_vip(player))
                {
                    player.PrintToChat($" {Config.Prefix} You already have {ChatColors.Green}VIP{ChatColors.Default} features, you {ChatColors.Red}can not activate{ChatColors.Default} this VIP!");
                    return;
                }

                MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);

                // Avoid null references by checking if MySQL is successfully initialized
                if (MySql == null)
                {
                    // Log the error and provide a default behavior or skip further processing
                    Logger.LogError("Failed to initialize MySQL in CommandActivator");
                    return;
                }

                MySqlQueryResult result = MySql.Table($"{Config.DBPrefix}_users_key_vip").Where(MySqlQueryCondition.New("token", "=", $"{token}")).Select();

                if (result.Rows == 1)
                {
                    var timeofvip = result.Get<int>(0, "end");
                    var group_int = result.Get<int>(0, "group");

                    // Graceful degradation for invalid timeofvip
                    if (result.Get<int>(0, "end") == 0)
                    {
                        timeofvip = 0;
                    }
                    else
                    {
                        timeofvip = result.Get<int>(0, "end");
                    }

                    var client = player.Index;

                    MySqlQueryValue _Tvalues = new MySqlQueryValue()
                        .Add("steam_id", $"{player.SteamID}")
                        .Add("end", $"{timeofvip}")
                        .Add("`group`", $"{group_int}");
                    MySql.Table($"{Config.DBPrefix}_users").Insert(_Tvalues);

                    player.PrintToChat($" {Config.Prefix} {Localizer["Activator"]}");

                    // Provide additional user feedback for special cases
                    if (result.Get<int>(0, "end") == 0)
                    {
                        player.PrintToChat($" {Config.Prefix} {Localizer["ForeverVIP"]}");
                    }

                    LoadPlayerData(player);
                    MySql.Table($"{Config.DBPrefix}_users_key_vip").Where($"token = '{token}'").Delete();
                }
                else
                {
                    player.PrintToChat($" {Config.Prefix} Token{ChatColors.Default}: {ChatColors.Red}{token}{ChatColors.Default} does not exist!");
                }
            }
            catch (Exception ex)
            {
                // Log the exception for further investigation
                Logger.LogError($"An unexpected error occurred in CommandActivator: {ex.Message}");
                // Provide a generic error message to the user
                player?.PrintToChat($" {Config.Prefix} An unexpected error occurred. Please contact support.");
            }
        }
        [ConsoleCommand("css_generatevip", "Generate new VIP token")]
        public void CommandGenerateVIP(CCSPlayerController? player, CommandInfo info)
        {
            if (player != null) return;
            var TimeSec = info.ArgByIndex(1);
            var Group = info.ArgByIndex(2);
            if (TimeSec == null || TimeSec == "" || !IsInt(TimeSec) || Group == null || Group == "" || !IsInt(Group))
            {
                Server.PrintToConsole($"==========================================");
                Server.PrintToConsole($" {Config.Prefix} You must add days: css_generatevip <DAYS> <GROUP>, must be added in int.");
                Server.PrintToConsole($" {Config.Prefix} If you wanna give forever VIP: css_generatevip 0 0");
                Server.PrintToConsole($" {Config.Prefix} <------------> List's of Groups <------------>");
                Server.PrintToConsole($" {Config.Prefix} < Group '0' > {Config.GroupsNames.Group1} < Group '0' >");
                Server.PrintToConsole($" {Config.Prefix} < Group '1' > {Config.GroupsNames.Group2} < Group '1' >");
                Server.PrintToConsole($" {Config.Prefix} < Group '2' > {Config.GroupsNames.Group3} < Group '2' >");
                Server.PrintToConsole($" {Config.Prefix} <------------> List's of Groups <------------>");
                Server.PrintToConsole($"==========================================");

                return;
            }
            var TimeToUTC = DateTime.UtcNow.AddDays(Convert.ToInt32(TimeSec)).GetUnixEpoch();
            var timeofvip = 0;
            if (TimeSec == "0")
            {
                timeofvip = 0;
            }
            else
            {
                timeofvip = DateTime.UtcNow.AddDays(Convert.ToInt32(TimeSec)).GetUnixEpoch();
            }

            var token = CreatePassword(20);
            var group_int = Group;

            var timeRemaining = DateTimeOffset.FromUnixTimeSeconds(TimeToUTC) - DateTimeOffset.UtcNow;
                var timeRemainingFormatted =
                $"{timeRemaining.Days}d {timeRemaining.Hours:D2}:{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}";

            MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);
            MySqlQueryValue values = new MySqlQueryValue()
            .Add("token", token)
            .Add("end", $"{timeofvip}")
            .Add("`group`", group_int);
            MySql.Table($"{Config.DBPrefix}_users_key_vip").Insert(values);

            Server.PrintToConsole($"==========================================");
            Server.PrintToConsole($"You generate new VIP Token");
            Server.PrintToConsole($"Token: {token}");
            Server.PrintToConsole($"Ending (days): {TimeSec}");
            Server.PrintToConsole($"Group ID: {Group}");
            Server.PrintToConsole($"==========================================");



        }
        [ConsoleCommand("css_addvip", "Add new VIP")]
        public void CommandAddVIP(CCSPlayerController? player, CommandInfo info)
        {
            if (player == null)
            {
                // Perform error handling or return early if player is null
                // For example:
                Server.PrintToConsole("VIP Plugin - Error: Player is null.");
                return;
            }

            var Group = info.ArgByIndex(3);
            var SteamIDC = info.ArgByIndex(2);
            var TimeSec = info.ArgByIndex(1);

            if (!AdminManager.PlayerHasPermissions(player, "@css/root"))
            {
                player.PrintToChat($" {Config.Prefix} You are not admin..");
                return;
            }
            else if (SteamIDC == null || SteamIDC == "" || !IsInt(SteamIDC))
            {
                player.PrintToChat($" {Config.Prefix} You must add SteamID. Example {ChatColors.Lime}/addvip <Time in days> 77777777 <GROUP>{ChatColors.Default}, must be added in int.");
                player.PrintToChat($" {Config.Prefix} Or if you wanna add forever VIP type {ChatColors.Lime}/addvip 0 77777777{ChatColors.Default}.");
                return;
            }
            else if (TimeSec == null || TimeSec == "" || !IsInt(TimeSec))
            {
                player.PrintToChat($" {Config.Prefix} You must add Time in days. Example {ChatColors.Lime}/addvip <Time in days> 77777777 <GROUP>{ChatColors.Default}, must be added in int.");
                player.PrintToChat($" {Config.Prefix} Or if you wanna add forever VIP type {ChatColors.Lime}/addvip 0 77777777{ChatColors.Default}.");

                return;
            }
            else if (Group == null || Group == "" || !IsInt(Group))
            {
                player.PrintToChat($" {Config.Prefix} You must add Group (Exist: 0, 1). Example {ChatColors.Lime}/addvip <Time in days> 77777777 <GROUP>{ChatColors.Default}, must be added in int.");
                player.PrintToChat($" {Config.Prefix} Or if you wanna add forever VIP type {ChatColors.Lime}/addvip 0 77777777 0{ChatColors.Default}.");
                player.PrintToChat($" {Config.Prefix} {ChatColors.Lime}<------------>{ChatColors.Default} List's of Groups {ChatColors.Lime}<------------>");
                player.PrintToChat($" {Config.Prefix} {ChatColors.Lime}< Group '0' >{ChatColors.Default} {Config.GroupsNames.Group1} {ChatColors.Lime}< Group '0' >");
                player.PrintToChat($" {Config.Prefix} {ChatColors.Lime}< Group '1' >{ChatColors.Default} {Config.GroupsNames.Group2} {ChatColors.Lime}< Group '1' >");
                player.PrintToChat($" {Config.Prefix} {ChatColors.Lime}< Group '2' >{ChatColors.Default} {Config.GroupsNames.Group3} {ChatColors.Lime}< Group '2' >");
                player.PrintToChat($" {Config.Prefix} {ChatColors.Lime}<------------>{ChatColors.Default} List's of Groups {ChatColors.Lime}<------------>");


                return;
            }
            else
            {
                var TimeToUTC = DateTime.UtcNow.AddDays(Convert.ToInt32(TimeSec)).GetUnixEpoch();
                var timeofvip = 0;
                if (TimeSec == "0")
                {
                    timeofvip = 0;
                }
                else
                {
                    timeofvip = DateTime.UtcNow.AddDays(Convert.ToInt32(TimeSec)).GetUnixEpoch();
                }


                var timeRemaining = DateTimeOffset.FromUnixTimeSeconds(TimeToUTC) - DateTimeOffset.UtcNow;
                var timeRemainingFormatted =
                $"{timeRemaining.Days}d {timeRemaining.Hours:D2}:{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}";
                var group_int = Group;

                MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);
                MySqlQueryValue values = new MySqlQueryValue()
                .Add("steam_id", $"{SteamIDC}")
                .Add("end", $"{timeofvip}")
                .Add("`group`", group_int);
                MySql.Table($"{Config.DBPrefix}_users").Insert(values);
                player.PrintToChat($" {ChatColors.Lime}=========================================");
                player.PrintToChat($" {Config.Prefix} Player with steamid {ChatColors.Lime}{SteamIDC}{ChatColors.Default} has been added.");
                player.PrintToChat($" {Config.Prefix} Ending time is {ChatColors.Lime}{timeRemainingFormatted}{ChatColors.Default}.");
                player.PrintToChat($" {ChatColors.Lime}=========================================");
                Server.PrintToConsole($"VIP Plugin - Admin {player.PlayerName} add new VIP with steamid {SteamIDC}, end time is {timeRemainingFormatted}");

            }
        }
        [ConsoleCommand("css_vips", "Load all VIPs on server")]
        public void CommandVIPList(CCSPlayerController? controller, CommandInfo info)
        {
            if (controller == null)
            {
                // Handle the case where controller is null
                Server.PrintToConsole("VIP Plugin - Error: Controller is null.");
                return;
            }

            int vips = 0;
            controller.PrintToChat($" {ChatColors.Green}===!-!==={ChatColors.Lime} VIP {ChatColors.Default}List {ChatColors.Green}===!-!===");
            foreach (var player in Utilities.GetPlayers().Where(player => player is { IsBot: false, IsValid: true }).Where(player => IsVIP[player.Index] == 1))
            {
                if (player != null)
                {
                    vips++;
                    controller.PrintToChat($" [{ChatColors.Green}{player.SteamID}{ChatColors.Default}] {ChatColors.Orange}{player.PlayerName}");
                }
            }
            controller.PrintToChat($" {ChatColors.Green}► Numbers of VIPs{ChatColors.Default} {ChatColors.Purple}{vips}{ChatColors.Default} {ChatColors.Green}◄ Numbers of VIPs");
            controller.PrintToChat($" {ChatColors.Green}===!-!==={ChatColors.Lime} VIP {ChatColors.Default}List {ChatColors.Green}===!-!===");
        }
        [ConsoleCommand("css_vip", "Info about VIP")]
        public void CommandVIPInfo(CCSPlayerController? player, CommandInfo info)
        {
            if (player == null)
            {
                // Handle the case where player is null
                Server.PrintToConsole("VIP Plugin - Error: Player is null.");
                return;
            }

            MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);

            MySqlQueryResult result = MySql?.Table($"{Config.DBPrefix}_users")?.Where(MySqlQueryCondition.New("steam_id", "=", player.SteamID?.ToString()))?.Select();
            var status = "";
            var formating = "";
            int status_i = 0;

            if (result?.Rows == 1)
            {
                var client = player.Index;
                var timeRemaining = DateTimeOffset.FromUnixTimeSeconds(result.Get<int>(0, "end")) - DateTimeOffset.UtcNow;
                var nowtimeis = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var timeRemainingFormatted =
                    $"{timeRemaining.Days}d {timeRemaining.Hours}:{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}";
                status = $" {ChatColors.Green}Active";
                formating = $" {ChatColors.Green}{timeRemainingFormatted}";
                IsVIP[client] = 1;
                LoadPlayerData(player);
                status_i = 1;

                if (result.Get<int>(0, "end") != 0)
                {
                    status = $" {ChatColors.Green}Active";
                }

                if (result.Get<int>(0, "end") == 0)
                {
                    formating = $" {ChatColors.Green}Never ending";
                }
                status_i = 1;
            }
            else
            {
                status = $" {ChatColors.Red} Inactive";
                status_i = 0;
            }

            player.PrintToChat($" {ChatColors.Green}-+-+-+-+-+-+ {ChatColors.Gold}✪ BRUTALCI VIP ✪ {ChatColors.Green}+-+-+-+-+-+-");
            player.PrintToChat($" {ChatColors.Gold}» {ChatColors.Default}You have {status}{ChatColors.Default} VIP Status.");
            if (status_i == 1)
            {
                player.PrintToChat($" {ChatColors.Gold}» {ChatColors.Default}Ending in: {ChatColors.Lime}{formating}{ChatColors.Default}.");
                player.PrintToChat($" {ChatColors.Gold}» {ChatColors.Default}{ChatColors.Lime}VIP {ChatColors.Default}Group: {ChatColors.Red}{get_name_group(player)}{ChatColors.Default}.");
                player.PrintToChat($" {ChatColors.Gold}▼ {ChatColors.Lime}Available Commands{ChatColors.Gold} ▼");
                if (get_vip_group(player) >= Config.CommandOnGroup.Weapons)
                {
                    player.PrintToChat($" {ChatColors.Gold}» {ChatColors.Default}Select Weapon: {ChatColors.Red}!guns {ChatColors.Default}ak, m4, m4a4, awp{ChatColors.Gold}");
                }
                if (get_vip_group(player) >= Config.CommandOnGroup.Pack)
                {
                    player.PrintToChat($" {ChatColors.Gold}» {ChatColors.Default}Select Weapon Pack: {ChatColors.Red}!wp {ChatColors.Default}ak, m4, m4a4{ChatColors.Gold}");
                }
                if (get_vip_group(player) >= Config.CommandOnGroup.Respawn || Config.RespawnAllowed)
                {
                    player.PrintToChat($" {ChatColors.Gold}► {ChatColors.Default}Respawn on spawn  : {ChatColors.Lime}/respawnvip{ChatColors.Gold} ◄");
                }
                player.PrintToChat($" {ChatColors.Gold}» {ChatColors.Default}Turn off Weapon Loadout: {ChatColors.Red}!gunsoff");

            }
            player.PrintToChat($" {ChatColors.Green}-+-+-+-+-+-+{ChatColors.Red} www.BRUTALCI.info {ChatColors.Green}+-+-+-+-+-+-");
        }
        static bool IsTimeBetween8PMAnd8AM() // ty k4ryu <3
        {
            // Get the current time
            DateTime currentTime = DateTime.Now;

            // Define the start and end times for the range (8 PM to 8 AM)
            TimeSpan startTime = TimeSpan.FromHours(20); // 8 PM
            TimeSpan endTime = TimeSpan.FromHours(8);    // 8 AM

            if (endTime < startTime)
            {
                // The range spans midnight, so we need to check if the current time
                // is less than the end time or greater than the start time.
                return currentTime.TimeOfDay >= startTime || currentTime.TimeOfDay <= endTime;
            }
            else
            {
                // The range does not span midnight, so we can simply check if the
                // current time is between the start and end times.
                return currentTime.TimeOfDay >= startTime && currentTime.TimeOfDay <= endTime;
            }
        }
        [ConsoleCommand("css_freevip", "Free VIP")]
        public void CommandFREEVIP(CCSPlayerController? player, CommandInfo info)
        {
            if (Config.TestVIP.EnableFreeVIP)
            {
                if (is_vip(player))
                {
                    player?.PrintToChat($" {Config.Prefix} You already have {ChatColors.Green}VIP{ChatColors.Default} features, you {ChatColors.Red}can not activate{ChatColors.Default} this VIP!");
                    return;
                }

                if (IsTimeBetween8PMAnd8AM())
                {
                    int TimeSec = Config.TestVIP.FreeVIPTime;
                    var TimeToUTC = DateTime.UtcNow.AddSeconds(Convert.ToInt32(TimeSec)).GetUnixEpoch();
                    var timeofvip = DateTime.UtcNow.AddSeconds(Convert.ToInt32(TimeSec)).GetUnixEpoch();
                    MySqlQueryValue _Tvalues = new MySqlQueryValue();
                    MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);
                    var timeRemaining = DateTimeOffset.FromUnixTimeSeconds(TimeToUTC) - DateTimeOffset.UtcNow;
                    var timeRemainingFormatted =
                        $"{timeRemaining.Days}d {timeRemaining.Hours:D2}:{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}";

                    MySqlQueryValue values = new MySqlQueryValue()
                        .Add("steam_id", $"{player?.SteamID?.ToString()}")
                        .Add("end", $"{timeofvip}")
                        .Add("`group`", $"0");
                    MySql.Table($"{Config.DBPrefix}_users").Insert(values);

                    var client = player?.Index;
                    LoadPlayerData(player);

                    player?.PrintToChat($" {Config.Prefix} You have activated {ChatColors.Red}FREE VIP. {ChatColors.Default} Ending in: {ChatColors.Red}{timeRemainingFormatted}{ChatColors.Default}."); ;
                    Server.PrintToConsole($"VIP Plugin - Player {player?.PlayerName} used FREE VIP with steamid {player?.SteamID}, end time is {timeRemainingFormatted}");
                }
                else
                {
                    player?.PrintToChat($" {Config.Prefix} You can't use {ChatColors.Red}FREE VIP {ChatColors.Default}before 20h.");
                }
            }
        }

        [ConsoleCommand("css_respawnvip", "Command to respawn player")]

        
        public void CommandRespawn(CCSPlayerController? player, CommandInfo info)
        {
            var client = player.Index;
            if (Config.RespawnAllowed)
            {
                if (IsVIP[client] == 0)
                {
                    player.PrintToChat($" {Config.Prefix} {Localizer["MustBeVIP"]}");
                    return;
                }
                if (Config.CommandOnGroup.Respawn >= get_vip_group(player))
                {
                    player.PrintToChat($" {Config.Prefix} {Localizer["LowGroup"]}");
                    return;
                }
                if (Round < Config.MinimumRoundToUseCommands)
                {
                    player.PrintToChat($" {Config.Prefix} {Localizer["MustThierd", Config.MinimumRoundToUseCommands]}");
                    return;
                }
                if (player.PawnIsAlive)
                {
                    player.PrintToChat($" {Config.Prefix} {Localizer["MustDeath"]}");
                    return;
                }
                if (RespawnUsed[client] == 1)
                {
                    if (Config.Messages.AllowCenterMessages)
                    {
                        player.PrintToCenterHtml($" <font color='red'>{Localizer["RespawnUse"]}</font>");
                    }
                    else
                    {
                        player.PrintToChat($" {Config.Prefix} {Localizer["RespawnUse"]}");
                    }
                    return;
                }
                if (Config.Messages.AllowCenterMessages)
                {
                    player.PrintToCenterHtml($" <font color='green'>{Localizer["RespawnUsed"]}</font>");
                }
                else
                {
                    player.PrintToChat($" {Config.Prefix} {Localizer["RespawnUsed"]}");
                }

                
                player.PlayerPawn.Value.Respawn();

                RespawnUsed[client] = 1;
            }
        }
        [ConsoleCommand("css_gunsoff", "Disable weapon loadout")]

        public void CommandGUNS_off(CCSPlayerController? player, CommandInfo info)
        {
            var client = player.Index;
            if (!player.IsValid || !player.PlayerPawn.IsValid)
            {
                return;
            }
            if (IsVIP[client] == 0)
            {
                player.PrintToChat($" {Config.Prefix} {Localizer["MustBeVIP"]}");
                return;
            }

            if (LastUsed[client] >= 1)
            {
                Used[client] = 0;
                LastUsed[client] = 0;
                if (Config.Messages.AllowCenterMessages)
                {
                    player.PrintToCenter($"{Localizer["TurnedOff"]}");
                }else
                {
                    player.PrintToChat($" {Config.Prefix} {Localizer["TurnedOff"]}");
                }
            }
        }
        [ConsoleCommand("css_guns", "Select a Weapon from commands")]

        public void SelectWeapon(CCSPlayerController? player, CommandInfo info)
        {
            var PackagesID = info.ArgByIndex(1);
            var client = player.Index;
            if (PackagesID == null || PackagesID == "")
            {
                player.PrintToChat($" {Config.Prefix} Please select a weapon.");
                player.PrintToChat($" Weapon (AK-47). Type:{ChatColors.Red}!guns ak{ChatColors.Default}.");
                player.PrintToChat($" Weapon (M4A4). Type:{ChatColors.Red}!guns m4a4{ChatColors.Default}.");
                player.PrintToChat($" Weapon (M4A1-S). Type:{ChatColors.Red}!guns m4{ChatColors.Default}.");
                player.PrintToChat($" Weapon (AWP). Type:{ChatColors.Red}!guns awp{ChatColors.Default}.");
                return;
            }
            if (!player.IsValid || !player.PlayerPawn.IsValid)
            {
                return;
            }
            if (!player.PawnIsAlive)
            {
                player.PrintToChat($" {Config.Prefix} {Localizer["MustBeAlive"]}");
                return;
            }
            if (IsVIP[client] == 0)
            {
                player.PrintToChat($" {Config.Prefix} {Localizer["MustBeVIP"]}");
                return;
            }
            if (Config.CommandOnGroup.Weapons > get_vip_group(player))
            {
                player.PrintToChat($" {Config.Prefix} {Localizer["LowGroup"]}");
                return;
            }
            if (Disabled20Sec)
            {
                player.PrintToChat($" {Config.Prefix} {Localizer["Must20Sec"]}");
                return;
            }

            if (Round < Config.MinimumRoundToUseCommands)
            {
                player.PrintToChat($" {Config.Prefix} {Localizer["MustThierd", Config.MinimumRoundToUseCommands]}");
                return;
            }
            if (Used[client] == 1)
            {
                player.PrintToChat($" {Config.Prefix} {Localizer["OnceUse"]}");
                return;
            }
            string weaponName = PackagesID.ToLower();
            
            if (weaponName == "ak")
            {
                Used[client] = 1;
                LastUsed[client] = 1;
                if (CheckIsHaveWeapon("ak47", player) == false)
                {
                    player.GiveNamedItem("weapon_ak47");
                }
                Server.PrintToChatAll($" {Config.Prefix} {ChatColors.Gold}{player.PlayerName} {ChatColors.Default}got a weapon {ChatColors.Lime}AK47");
            }
            else if (weaponName == "m4a4")
            {
                Used[client] = 1;
                LastUsed[client] = 4;
                if (CheckIsHaveWeapon("m4a1", player) == false)
                {
                    player.GiveNamedItem("weapon_m4a1");
                }
                Server.PrintToChatAll($" {Config.Prefix} {ChatColors.Gold}{player.PlayerName} {ChatColors.Default}got a weapon {ChatColors.Lime}M4A4");
            }
            else if (weaponName == "m4")
            {
                Used[client] = 1;
                LastUsed[client] = 5;
                if (CheckIsHaveWeapon("m4a1_silencer", player) == false)
                {
                    player.GiveNamedItem("weapon_m4a1_silencer");
                }
                Server.PrintToChatAll($" {Config.Prefix} {ChatColors.Gold}{player.PlayerName} {ChatColors.Default}got a weapon {ChatColors.Lime}M4A1-S");
            }
            else if (weaponName == "awp")
            {
                Used[client] = 1;
                LastUsed[client] = 6;
                if (CheckIsHaveWeapon("awp", player) == false)
                {
                    player.GiveNamedItem("weapon_awp");
                }
                Server.PrintToChatAll($" {Config.Prefix} {ChatColors.Gold}{player.PlayerName} {ChatColors.Default}got a weapon {ChatColors.Lime}AWP");
            }
            else
            {
                Used[client] = 0;
                LastUsed[client] = 0;
                player.PrintToChat($" {Config.Prefix} Please select a weapon.");
                player.PrintToChat($" Weapon (AK-47). Type: {ChatColors.Red}!guns ak{ChatColors.Default}.");
                player.PrintToChat($" Weapon (M4A4). Type: {ChatColors.Red}!guns m4a4{ChatColors.Default}.");
                player.PrintToChat($" Weapon (M4A1-S). Type: {ChatColors.Red}!guns m4{ChatColors.Default}.");
                player.PrintToChat($" Weapon (AWP). Type: {ChatColors.Red}!guns awp{ChatColors.Default}.");
            }

        }
        [ConsoleCommand("css_wp", "Select a packages of weapons")]

        public void PackagesWeapons(CCSPlayerController? player, CommandInfo info)
        {
            var PackagesID = info.ArgByIndex(1);
            var client = player.Index;
            if (PackagesID == null || PackagesID == "")
            {
                player.PrintToChat($" {Config.Prefix} Available Weapon Packs:{ChatColors.Lime}!wp {ChatColors.Lime}ak{ChatColors.Default}, {ChatColors.Lime}m4{ChatColors.Default}, {ChatColors.Lime}m4a4{ChatColors.Default}.");
                return;
            }
            if (!player.IsValid || !player.PlayerPawn.IsValid)
            {
                return;
            }
            if (!player.PawnIsAlive)
            {
                player.PrintToChat($" {Config.Prefix} {Localizer["MustBeAlive"]}");
                return;
            }
            if (IsVIP[client] == 0)
            {
                player.PrintToChat($" {Config.Prefix} {Localizer["MustBeVIP"]}");
                return;
            }
            if (Config.CommandOnGroup.Pack > get_vip_group(player))
            {
                player.PrintToChat($" {Config.Prefix} {Localizer["LowGroup"]}");
                return;
            }
            if (Disabled20Sec)
            {
                player.PrintToChat($" {Config.Prefix} {Localizer["Must20Sec"]}");
                return;
            }

            if (Round < Config.MinimumRoundToUseCommands)
            {
                player.PrintToChat($" {Config.Prefix} {Localizer["MustThierd", Config.MinimumRoundToUseCommands]}");
                return;
            }
            if (Used[client] == 1)
            {
                player.PrintToChat($" {Config.Prefix} {Localizer["OnceUse"]}");
                return;
            }
            string weaponName = PackagesID.ToLower();

            if (weaponName == "ak")
            {
                Used[client] = 1;
                LastUsed[client] = 2;
                player.PrintToChat($" {Config.Prefix} {Localizer["Packages_one"]}");
                Server.PrintToChatAll($" {Config.Prefix} {ChatColors.Red}{player.PlayerName} {ChatColors.Default}has used weapon pack!");
                foreach(var weapon in Config.Pack1Settings.Weapons)
                {
                    if (CheckIsHaveWeapon($"{weapon}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{weapon}");
                    }
                }
            }
            else if (weaponName == "m4")
            {
                Used[client] = 1;
                LastUsed[client] = 3;
                player.PrintToChat($" {Config.Prefix} {Localizer["Package_two"]}");
                Server.PrintToChatAll($" {Config.Prefix} {ChatColors.Red}{player.PlayerName} {ChatColors.Default}has used weapon pack!");
                foreach (var weapon in Config.Pack2Settings.Weapons)
                {
                    if (CheckIsHaveWeapon($"{weapon}", player) == false)
                    {
                        player.GiveNamedItem($"weapon_{weapon}");
                    }
                }
            }
            else if (weaponName == "m4a4")
            {
                if (Config.Pack3Settings.Allowed)
                {
                    Used[client] = 1;
                    LastUsed[client] = 10;
                    player.PrintToChat($" {Config.Prefix} {Localizer["Package_three"]}");
                    Server.PrintToChatAll($" {Config.Prefix} {ChatColors.Red}{player.PlayerName} {ChatColors.Default}has used weapon pack!");
                    foreach (var weapon in Config.Pack3Settings.Weapons)
                    {
                        if (CheckIsHaveWeapon($"{weapon}", player) == false)
                        {
                            player.GiveNamedItem($"weapon_{weapon}");
                        }
                    }
                }
            }
            else
            {
                Used[client] = 0;
                LastUsed[client] = 0;
                player.PrintToChat($" {Config.Prefix} Available Weapon Packs: {ChatColors.Lime}!wp {ChatColors.Lime}ak{ChatColors.Default}, {ChatColors.Lime}m4{ChatColors.Default}, {ChatColors.Lime}m4a4{ChatColors.Default}.");
            }
        }
    }
}
