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
using System.Text;
using CounterStrikeSharp.API.Modules.Timers;
using System.Security.Cryptography;
using CounterStrikeSharp.API.Modules.Memory;
using System.Threading.Channels;
using System.Reflection.Metadata;
using System.Net;

namespace VIP;
[MinimumApiVersion(130)]

public static class GetUnixTime
{
    public static int GetUnixEpoch(this DateTime dateTime)
    {
        var unixTime = dateTime.ToUniversalTime() -
                       new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return (int)unixTime.TotalSeconds;
    }
}
public partial class VIP : BasePlugin, IPluginConfig<ConfigVIP>
{
    public override string ModuleName => "VIP";
    public override string ModuleAuthor => "DeadSwim";
    public override string ModuleDescription => "Advanced VIP system based on database.";
    public override string ModuleVersion => "V. 1.5.0";
    private string DatabaseConnectionString = string.Empty;
    private static readonly int?[] IsVIP = new int?[65];
    private static readonly int?[] HaveGroup = new int?[65];
    private static readonly int?[] Used = new int?[65];
    private static readonly int?[] LastUsed = new int?[65];
    private static readonly int?[] RespawnUsed = new int?[64];
    private static readonly int?[] HaveDoubble = new int?[64];
    private static readonly int?[] HaveReservation = new int?[64];
    private static readonly int?[] allow_bombinfo = new int?[64];

    private static readonly bool?[] allowedHit = new bool?[64];
    private static readonly int?[] damage = new int?[64];
    private static readonly int?[] armor = new int?[64];
    private static readonly string?[] damaged_player = new string?[64];

    //User settings
    private static readonly int?[] UserBhop = new int?[64];
    private static readonly int?[] UserSmoke = new int?[64];
    private static readonly int?[] UserHit = new int?[64];

    private static readonly int[] J = new int[Server.MaxPlayers];
    private static readonly PlayerFlags[] LF = new PlayerFlags[Server.MaxPlayers];
    private static readonly PlayerButtons[] LB = new PlayerButtons[Server.MaxPlayers];


    public ConfigVIP Config { get; set; }

    public int Round;
    public int ConnectedPlayers;
    public bool Bombplanted;
    public bool DisableGiving;
    public bool Bomb;
    public float bombtime;
    public bool Disabled20Sec;
    public string SitePlant;
    public CounterStrikeSharp.API.Modules.Timers.Timer? timer_ex;
    public CounterStrikeSharp.API.Modules.Timers.Timer? timer_twenty;
    public CounterStrikeSharp.API.Modules.Timers.Timer? timer_hitshow;

    public void OnConfigParsed(ConfigVIP config)
    {
        Config = config;
        if (config.DBHost.Length < 1 || Config.DBUser.Length < 1 || Config.DBPassword.Length < 1)
        {
            throw new Exception("You need to setup Database credentials in config!");

        }
    }
    private bool IsInt(string sVal)
    {
        foreach (char c in sVal)
        {
            int iN = (int)c;
            if ((iN > 57) || (iN < 48))
                return false;
        }
        return true;
    }


    public override void Load(bool hotReload)
    {
        Console.WriteLine($"{Config.Prefix} VIP Plugins started, by deadswim");
        try
        {
            MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);



            MySql.ExecuteNonQueryAsync(@$"CREATE TABLE IF NOT EXISTS `{Config.DBPrefix}_users` (`id` INT AUTO_INCREMENT PRIMARY KEY, `steam_id` VARCHAR(32) UNIQUE NOT NULL, `end` INT(11) NOT NULL, `group` INT(11) NOT NULL, UNIQUE (`steam_id`));");
            MySql.ExecuteNonQueryAsync(@$"CREATE TABLE IF NOT EXISTS `{Config.DBPrefix}_users_test_vip` (`id` INT AUTO_INCREMENT PRIMARY KEY, `steam_id` VARCHAR(32) UNIQUE NOT NULL, `used` INT(11) NOT NULL, `group` INT(11) NOT NULL, UNIQUE (`steam_id`));");
            MySql.ExecuteNonQueryAsync(@$"CREATE TABLE IF NOT EXISTS `{Config.DBPrefix}_users_key_vip` (`id` INT AUTO_INCREMENT PRIMARY KEY, `token` VARCHAR(32) UNIQUE NOT NULL, `end` INT(11) NOT NULL, `group` INT(11) NOT NULL, UNIQUE (`token`));");
            MySql.ExecuteNonQueryAsync(@$"CREATE TABLE IF NOT EXISTS `freevip` (`id` INT AUTO_INCREMENT PRIMARY KEY, `steam_id` VARCHAR(32) UNIQUE NOT NULL, `end` INT(11) NOT NULL, `group` INT(11) NOT NULL, UNIQUE (`steam_id`));");

            WriteColor($"VIP Plugin - *[MySQL {Config.DBHost} Connected]", ConsoleColor.Green);


        }
        catch (Exception ex)
        {
            Server.PrintToConsole($"Error while loading: {ex.Message}");
        }
        RegisterListener<Listeners.OnMapStart>(name =>
        {
            Round = 0;
        });
        if (Config.DBUser.Length > 4)
        {

        }
        // Load VIP Prefix
        AddCommandListener("say", OnPlayerChat);
        AddCommandListener("say_team", OnPlayerChatTeam);
        // Load Commands

        // Load Smoke colors
        RegisterListener<Listeners.OnEntitySpawned>(OnEntitySpawned);
        RegisterListener<Listeners.OnClientAuthorized>((index, id) =>
        {
            var player = Utilities.GetPlayerFromSlot(index);

            Authorization_Client(player);
        });

        RegisterListener<Listeners.OnTick>(() =>
        {
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                var ent = NativeAPI.GetEntityFromIndex(i);
                if (ent == 0)
                    continue;

                var client = new CCSPlayerController(ent);
                if (client == null || !client.IsValid)
                    continue;
                if (IsVIP[client.Index] == 0)
                    return;
                OnTick(client);
                if (UserBhop[client.Index] != 1)
                {
                    TryBhop(client);
                }

                if (allowedHit[client.Index] == true)
                {
                    var clienti = client.Index;
                    if (UserHit[clienti] != 1)
                        return;
                    client.PrintToCenterHtml(
                        $"<font color='green'>Player :</font> <font color='gold'>{damaged_player[clienti]}</font><br>" +
                        $"<font color='green'>HP :</font> <font color='red'>-{damage[clienti]}</font> <font color='green'>AP :</font> <font color='red'>-{armor[clienti]}</font>"
                        );
                }
                if (!Config.Bombinfo)
                    return;
                if (Bomb)
                {
                    if (allow_bombinfo[client.Index] == 1)
                    {
                        if (bombtime >= 25)
                        {
                            client.PrintToCenterHtml(
                            $"<font color='gray'>C4: </font> <font class='fontSize-l' color='green'>{bombtime}</font>" +
                            $"<font color='gray'> SITE: </font> <font class='fontSize-l' color='green'>[{SitePlant}]</font>");
                        }
                        else if (bombtime >= 10)
                        {
                            client.PrintToCenterHtml(
                            $"<font color='green'>C4: </font> <font class='fontSize-l' color='orange'>{bombtime}</font>" +
                            $"<font color='gray'> SITE: </font> <font class='fontSize-l' color='orange'>[{SitePlant}]</font>");
                        }
                        else if (bombtime >= 5)
                        {
                            client.PrintToCenterHtml(
                            $"<font color='gold'>C4:</font> <font class='fontSize-l' color='red'>{bombtime}</font>" +
                            $"<font color='gold'> SITE: </font> <font class='fontSize-l' color='red'>[{SitePlant}]</font>");
                        }
                        else if (bombtime >= 0)
                        {
                            client.PrintToCenterHtml(
                            $"<font color='gold'>C4:</font> <font class='fontSize-l' color='red'>{bombtime}</font>" +
                            $"<font color='gold'> SITE: </font> <font class='fontSize-m' color='red'>[{SitePlant}]</font><br>" +
                            $"<font class='fontSize-s' color='white'>www.</font><font class='fontSize-s' color='red'>BRUTALCI</font><font class='fontSize-s' color='white'>.info</font>");
                        }
                        else if (bombtime == 0)
                        {
                            Bomb = false;
                        }
                    }
                }
            }
        });

        RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
        if (hotReload)
        {
            RegisterListener<Listeners.OnMapStart>(name =>
            {
                ConnectedPlayers = 0;
                Round = 0;
            });

        }
    }
    public string CreatePassword(int length)
    {
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        StringBuilder res = new StringBuilder();
        Random rnd = new Random();
        while (0 < length--)
        {
            res.Append(valid[rnd.Next(valid.Length)]);
        }
        return res.ToString();
    }

    public void TryBhop(CCSPlayerController controller)
    {
        if (!controller.PawnIsAlive)
            return;
        var buttons = controller.Buttons;
        var client = controller.Index;
        var PP = controller.PlayerPawn.Value;
        var flags = (PlayerFlags)PP!.Flags;
        if (IsVIP[client] != 1)
            return;
        if (UserBhop[client] != 1)
            return;
        if (Config.CommandOnGroup.ReservedSlots > get_vip_group(controller))
        {
            if ((flags & PlayerFlags.FL_ONGROUND) != 0 && (buttons & PlayerButtons.Jump) != 0)
                PP!.AbsVelocity.Z = 300;
        }
    }
    public void Authorization_Client(CCSPlayerController player)
    {
        var client = player.Index;
        LoadPlayerData(player);
        var slots = Server.MaxPlayers;
        slots = slots - Config.ReservedSlotsForVIP;

        int connected = 0;
        foreach (var player_l in Utilities.GetPlayers().Where(player => player is { IsBot: false, IsValid: true }))
        {
            if (player_l.UserId != 65535)
            {
                connected++;
            }
        }
        ConnectedPlayers = connected;

        WriteColor($"VIP PLugins - Player [{player.PlayerName}] try to connect on server, player on server: [{ConnectedPlayers}].", ConsoleColor.Green);


        WriteColor($"---------------------------------", ConsoleColor.Green);
        WriteColor($"Actual players on Server : {ConnectedPlayers}", ConsoleColor.Green);
        WriteColor($"Actual maxplayers on Server : {Server.MaxPlayers}", ConsoleColor.Green);
        WriteColor($"Can be connected : {slots}", ConsoleColor.Green);
        WriteColor($"---------------------------------", ConsoleColor.Green);
        if (Config.ReservedMethod == 1)
        {
            if (ConnectedPlayers >= slots)
            {
                if (IsVIP[client] == 1 || AdminManager.PlayerHasPermissions(player, "@css/reservation"))
                {
                    WriteColor($"VIP PLugins - Player [{player.PlayerName}] connecting to server, trying too use [Reserved slot]", ConsoleColor.Green);

                    if (HaveReservation[client] == 1)
                    {
                        Server.PrintToConsole($"VIP Plugins - Player {player.PlayerName} used the Reservated slot!");
                        return;
                    }
                    else
                    {
                        Server.ExecuteCommand($"kickid {player.UserId} \"This slot is reserved for VIPs\"");
                        Server.PrintToConsole($"VIP Plugins - Player {player.PlayerName} is kicked from the server, This slot is reserved for VIPs!");
                    }
                }
                else
                {
                    Server.ExecuteCommand($"kickid {player.UserId} \"This slot is reserved for VIPs\"");
                    Server.PrintToConsole($"VIP Plugins - Player {player.PlayerName} is kicked from the server, This slot is reserved for VIPs!");
                }
            }
        }

        else if (Config.ReservedMethod == 2)
        {
            bool kicked = false;
            if (ConnectedPlayers == Server.MaxPlayers)
            {
                if (IsVIP[client] == 1 || AdminManager.PlayerHasPermissions(player, "@css/reservation"))
                {
                    foreach (var l_player in Utilities.GetPlayers())
                    {
                        if (l_player.UserId != 65535)
                        {
                            CCSPlayerController player_res = l_player;

                            var el_player = player_res.Index;
                            WriteColor($"VIP PLugins - Player [{player.PlayerName}] connecting to server, trying too use [Reserved slot]", ConsoleColor.Green);
                            if (kicked == false)
                            {
                                if (IsVIP[el_player] != 1)
                                {
                                    kicked = true;
                                    Server.PrintToChatAll($" {Config.Prefix}Player {ChatColors.Lime}{player_res.PlayerName} {ChatColors.Default}has been kicked. {ChatColors.Lime}VIP{ChatColors.Default} player is connecting.");
                                    Server.ExecuteCommand($"kickid {player_res.UserId}");
                                }
                            }
                        }
                    }
                }
                else
                {
                    WriteColor($"VIP Plugin - [*Server is full*] player {player.PlayerName} has been kicked from server.", ConsoleColor.Yellow);
                    Server.ExecuteCommand($"kickid {player.UserId}");
                }
            }
            kicked = false;
        }
        else if (Config.ReservedMethod == 0)
        {
            return;
        }
    }
    public static void OnTick(CCSPlayerController controller)
    {
        if (!controller.PawnIsAlive)
            return;
        var pawn = controller.Pawn.Value;
        var flags = (PlayerFlags)pawn.Flags;
        var client = controller.Index;
        var buttons = controller.Buttons;

        if (IsVIP[client] == 0)
            return;
        if (HaveDoubble[client] != 0)
        {
            if ((LF[client] & PlayerFlags.FL_ONGROUND) != 0 && (flags & PlayerFlags.FL_ONGROUND) == 0 &&
                (LB[client] & PlayerButtons.Jump) == 0 && (buttons & PlayerButtons.Jump) != 0)
            {
                J[client]++;
            }
            else if ((flags & PlayerFlags.FL_ONGROUND) != 0)
            {
                J[client] = 0;
            }
            else if ((LB[client] & PlayerButtons.Jump) == 0 && (buttons & PlayerButtons.Jump) != 0 && J[client] <= 1)
            {
                J[client]++;
                pawn.AbsVelocity.Z = 320;
            }
        }

        LF[client] = flags;
        LB[client] = buttons;
    }
    internal static CCSGameRules GameRules()
    {
        return Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules!;
    }
    public void LoadPlayerData(CCSPlayerController player)
    {
        MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);

        MySqlQueryResult result = MySql!.Table($"{Config.DBPrefix}_users").Where(MySqlQueryCondition.New("steam_id", "=", player.SteamID.ToString())).Select();
        if (result.Rows == 1)
        {
            var client = player.Index;
            IsVIP[client] = 1;
            HaveGroup[client] = result.Get<int>(0, "group");
            UserBhop[client] = 1;
            UserHit[client] = 1;
            UserSmoke[client] = 1;
            if (Config.CommandOnGroup.ReservedSlots > get_vip_group(player))
            {
                HaveReservation[client] = 1;
            }
            else
            {
                HaveReservation[client] = 0;
            }
            player.Clan = get_name_group(player);
            AdminManager.AddPlayerToGroup(player, Config.GroupToVip);

            var timeRemaining = DateTimeOffset.FromUnixTimeSeconds(result.Get<int>(0, "end")) - DateTimeOffset.UtcNow;
            var nowtimeis = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var timeRemainingFormatted =
            $"{timeRemaining.Days}d {timeRemaining.Hours}:{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}";
            WriteColor($"VIP Plugin - Player [{player.PlayerName} ({player.SteamID})] has VIP. Remaining time of VIP [{timeRemainingFormatted}]", ConsoleColor.Green);

            // Checking if is still time to VIP
            if (result.Get<int>(0, "end") != 0)
            {
                if (result.Get<int>(0, "end") < nowtimeis)
                {
                    WriteColor($"VIP Plugin - Player [{player.PlayerName} ({player.SteamID})] VIP expires today..", ConsoleColor.Red);

                    MySql.Table($"{Config.DBPrefix}_users").Where(MySqlQueryCondition.New("steam_id", "=", player.SteamID.ToString())).Delete();
                    IsVIP[client] = 0;
                }
            }
            else
            {
                WriteColor($"VIP Plugin - Player [{player.PlayerName} ({player.SteamID})] has permanent VIP.", ConsoleColor.Green);
            }
        }
        else
        {
            var client = player.Index;
            HaveReservation[client] = 0;
            IsVIP[client] = 0;
            HaveGroup[client] = null;
            allow_bombinfo[player.Index] = 0;
            WriteColor($"VIP Plugin - Player [{player.PlayerName} ({player.SteamID})] is not VIP.", ConsoleColor.Yellow);
        }
    }
    public static void RemoveWeapons(CCSPlayerController? player)
    {
        foreach (var weapon in player.PlayerPawn.Value.WeaponServices!.MyWeapons)
        {
            if (weapon is { IsValid: true, Value.IsValid: true })
            {
                if (!weapon.Value.DesignerName.Contains("bayonet") || !weapon.Value.DesignerName.Contains("knife"))
                { continue; }
                weapon.Value.Remove();
                Server.PrintToConsole($"{player.PlayerName} remove weapon {weapon.Value.DesignerName}");
            }
        }
    }
    private bool CheckIsHaveWeapon(string weapon_name, CCSPlayerController? pc)
    {
        if (pc == null || !pc.IsValid)
            return false;

        var pawn = pc.PlayerPawn.Value?.WeaponServices!;
        foreach (var weapon in pawn.MyWeapons)
        {
            if (weapon is { IsValid: true, Value.IsValid: true } && weapon.Value.DesignerName.Contains($"{weapon_name}"))
            {
                    // WriteColor($"VIP Plugin - Requested weapon is [weapon_{weapon_name}]", ConsoleColor.Cyan);
                    // WriteColor($"VIP Plugin - {pc.PlayerName} has weapon with name [{weapon.Value.DesignerName}]", ConsoleColor.Cyan);
                    WriteColor($"VIP Plugin - {pc.PlayerName} has requested weapon -> [{weapon.Value.DesignerName}]", ConsoleColor.Cyan);
                    return true;
            }
        }
        return false;
    }
    private void OnEntitySpawned(CEntityInstance entity)
    {
        if (!Config.EnableVIPColoredSmokes) return;
        if (entity.DesignerName != "smokegrenade_projectile") return;

        var smokeGrenadeEntity = new CSmokeGrenadeProjectile(entity.Handle);
        if (smokeGrenadeEntity.Handle == IntPtr.Zero) return;

        Server.NextFrame(() =>
        {
            var entityIndex = smokeGrenadeEntity.Thrower.Value.Controller.Value.Index;

            if (entityIndex == null) return;
            if (IsVIP[entityIndex] == 0) return;
            if (UserSmoke[entityIndex] == 0) return;
            if (Config.CommandOnGroup.Smoke > HaveGroup[entityIndex]) return;
            smokeGrenadeEntity.SmokeColor.X = Random.Shared.NextSingle() * 255.0f;
            smokeGrenadeEntity.SmokeColor.Y = Random.Shared.NextSingle() * 255.0f;
            smokeGrenadeEntity.SmokeColor.Z = Random.Shared.NextSingle() * 255.0f;
        });
    }


    private string ReplaceTags(string message) // THX https://github.com/daffyyyy/CS2-Tags/blob/main/CS2-Tags.cs
    {
        if (message.Contains('{'))
        {
            string modifiedValue = message;
            foreach (FieldInfo field in typeof(ChatColors).GetFields())
            {
                string pattern = $"{{{field.Name}}}";
                if (message.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                {
                    modifiedValue = modifiedValue.Replace(pattern, field.GetValue(null)!.ToString(), StringComparison.OrdinalIgnoreCase);
                }
            }
            return modifiedValue;
        }

        return message;
    }

    private void Give_Values(CCSPlayerController controller)
    {
        if (controller == null || !controller.IsValid || controller.IsBot)
            return;

        var client = controller.Index;
        if (IsVIP[client] == 1)
        {
            if (Config.EnableVIPAcceries)
            {
                if (Config.CommandOnGroup.Acceries > get_vip_group(controller)) return;
                //controller.PlayerPawn.Value.Health = Config.RewardsClass.SpawnHP;
                set_armor(controller, Config.RewardsClass.SpawnArmor);
                controller.PlayerPawn.Value.Health = Config.RewardsClass.SpawnHP;

                if (get_money(controller) <= 800)
                {
                    set_money(controller, Config.RewardsClass.FirstSpawnMoney);
                }
                if (controller.TeamNum == 3)
                {
                    GiveItem(controller, "item_defuser");
                }
                if (LastUsed[client] != 2 || LastUsed[client] != 3)
                {
                    foreach (var weapon in Config.SpawnItems)
                    {
                        if (CheckIsHaveWeapon($"{weapon}", controller) == false)
                        {
                            controller.GiveNamedItem($"{weapon}");
                        }
                    }
                }
            }
        }

    }
    // Database settings
}
