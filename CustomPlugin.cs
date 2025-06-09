using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using System.Reflection;
using static CounterStrikeSharp.API.Core.Listeners;
using System;
using System.Drawing;
using System.Text;
using Nexd.MySQL;
using Microsoft.Extensions.Logging;
using CounterStrikeSharp.API.Modules.Cvars;
using System.Numerics;
using StoreApi;
using TagsApi;
using static StoreApi.Store;
using System.Net;
using Newtonsoft.Json;
using System.Text.RegularExpressions;



namespace CustomPlugin;

public static class GetUnixTime
{
    public static int GetUnixEpoch(this DateTime dateTime)
    {
        var unixTime = dateTime.ToUniversalTime() -
                       new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return (int)unixTime.TotalSeconds;
    }
}

public partial class CustomPlugin : BasePlugin, IPluginConfig<ConfigBan>
{
    public override string ModuleName => "VIP";
    public override string ModuleAuthor => "DeadSwim";
    public override string ModuleDescription => "VIP";
    public override string ModuleVersion => "V. 1.0.0";

    public bool Bomb;
    public bool active_bool;
    public float bombtime;
    public int Round;
    public string ?SitePlant;
    public CounterStrikeSharp.API.Modules.Timers.Timer? timer_ex;
    public CounterStrikeSharp.API.Modules.Timers.Timer? timer_ac;
    private static readonly int?[] UserTrialColor = new int?[64];
    private static readonly int?[] CreditEnable = new int?[64];
    private static readonly int?[] ShotsEnable = new int?[64];
    private static readonly int?[] KillCount = new int?[64];
    private static readonly int?[] Selected = new int?[64];
    private static readonly int?[] Selected_round = new int?[64];
    private static readonly int?[] Selectedr = new int?[64];

    private static readonly int?[] Trials = new int?[64];
    private static readonly int?[] NadeEnable = new int?[64];
    private static readonly int?[] NadeTrials = new int?[64];
    private static readonly int?[] CSStore = new int?[64];
    private static readonly int?[] LaserShot = new int?[64];
    private static readonly int?[] Healthshot = new int?[64];
    private static readonly int?[] Guns = new int?[64];
    private static readonly int?[] Bhop = new int?[64];
    private static readonly int?[] Health = new int?[64];
    private static readonly int?[] Models = new int?[64];
    private static readonly int?[] Bomb_a = new int?[64];
    private static readonly int?[] MVIP = new int?[64];
    private static readonly int?[] Timestamp = new int?[64];


    private static readonly bool?[] allowedHit = new bool?[64];
    private static readonly int?[] damage = new int?[64];
    private static readonly int?[] armor = new int?[64];
    private static readonly string?[] damaged_player = new string?[64];
    private static readonly int?[] UserHit = new int?[64];

    private static readonly int?[] J = new int?[64];
    private static readonly PlayerFlags[] LF = new PlayerFlags[64];
    private static readonly CounterStrikeSharp.API.PlayerButtons[] LB = new CounterStrikeSharp.API.PlayerButtons[64];

    public bool IsOld;

    public IStoreApi? StoreApi { get; set; }
    public ITagApi? TagsApi { get; set; }


    public required ConfigBan Config { get; set; }


    public void OnConfigParsed(ConfigBan config)
    {
        Config = config;
    }

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        StoreApi = IStoreApi.Capability.Get() ?? throw new Exception("StoreApi could not be located."); // Used for VIP Prémium
        TagsApi = ITagApi.Capability.Get() ?? throw new Exception("TagsApi could not be located.");
    }
    public override void Load(bool hotReload)
    {

        CreateDatabase();
        Console.WriteLine("VIP System, created by DeadSwim");
        RegisterListener<Listeners.OnMapStart>(name =>
        {
            //change_cvar("bot_quota", "2");
            //change_cvar("bot_join_after_player", "0");
            active_bool = false;
            Round = 0;
        });
        //RegisterListener<OnEntityCreated>(OnEntityCreated); Only in Prémium
        RegisterListener<Listeners.OnTick>(() =>
        {
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                var ent = NativeAPI.GetEntityFromIndex(i);
                if (ent == 0)
                    continue;
                var client = new CCSPlayerController(ent);
                TryBhop(client);
                OnTick(client);
                if (Bomb_a[client.Index] == 1)
                {
                    if (Bomb)
                    {
                        if (bombtime >= 25)
                        {
                            client.PrintToCenterHtml(
                            $"<font color='gray'>{Localizer["Detonation"]}: </font> <font class='fontSize-l' color='green'>{bombtime} s</font><br>" +
                            $"<font color='gray'>{Localizer["PlacedOn"]}</font> <font class='fontSize-m' color='green'>[{SitePlant}]</font>");
                        }
                        else if (bombtime >= 10)
                        {
                            client.PrintToCenterHtml(
                            $"<font color='gray'>{Localizer["Detonation"]}: </font> <font class='fontSize-l' color='green'>{bombtime} s</font><br>" +
                            $"<font color='gray'>{Localizer["PlacedOn"]}</font> <font class='fontSize-m' color='green'>[{SitePlant}]</font>");
                        }
                        else if (bombtime >= 5)
                        {
                            client.PrintToCenterHtml(
                            $"<font color='gray'>{Localizer["Detonation"]}: </font> <font class='fontSize-l' color='green'>{bombtime} s</font><br>" +
                            $"<font class='fontSize-l' color='red'> >>>> TIK TAK! {bombtime} s <<<< </font><br>" +
                            $"<font color='gray'>{Localizer["PlacedOn"]}</font> <font class='fontSize-m' color='green'>[{SitePlant}]</font>");
                        }
                        else if (bombtime >= 2)
                        {
                            client.PrintToCenterHtml(
                            $"<font class='fontSize-m' color='red'> >>>> !KABOOOM! <<<< </font><br>" +
                            $"<font class='fontSize-m' color='red'> >>>> !BOOOOOM! <<<< </font><br>" +
                            $"<font class='fontSize-m' color='red'> >>>> !KABOOOM! <<<< </font><br>");
                        }
                        else if (bombtime == 0)
                        {
                            Bomb = false;
                        }
                    }
                }
            }
        });
    }
    static public bool change_cvar(string cvar, string value)
    {
        var find_cvar = ConVar.Find($"{cvar}");
        if (find_cvar == null)
        {
            Server.PrintToConsole($"SpecialRound - [*ERROR*] Canno't set {cvar} to {value}.");
            return false;
        }
        Server.ExecuteCommand($"{cvar} {value}");
        return true;
    }
    public void CreateDatabase()
    {
        try
        {
            MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);



            MySql.ExecuteNonQueryAsync(@"CREATE TABLE IF NOT EXISTS `deadswim_settings` (`id` INT AUTO_INCREMENT PRIMARY KEY, `steamid` VARCHAR(32) UNIQUE NOT NULL, `enable_quake` INT(11) NOT NULL, `enable_nade` INT(11) NOT NULL, `trials` INT(11) NOT NULL, `guns` INT(11) NOT NULL,  `credits` VARCHAR(255) NOT NULL, UNIQUE (`steamid`));");
            MySql.ExecuteNonQueryAsync(@"CREATE TABLE IF NOT EXISTS `deadswim_vip` (`id` INT AUTO_INCREMENT PRIMARY KEY, `steamid` VARCHAR(32) UNIQUE NOT NULL, `healthshot` INT(11) NOT NULL, `nade` INT(11) NOT NULL, `store_credit` INT(11) NOT NULL, `trials` INT(11) NOT NULL, `shotlaser` INT(11) NOT NULL, `guns` INT(11) NOT NULL, `bhop` INT(11) NOT NULL, `models` INT(11) NOT NULL,  `health` INT(11) NOT NULL, `bomb` INT(11) NOT NULL, `mvip` INT(11) NOT NULL, `timestamp` INT(11) NOT NULL, UNIQUE (`steamid`));");
            MySql.ExecuteNonQueryAsync(@$"CREATE TABLE IF NOT EXISTS `deadswim_users_key_vip` (`id` INT AUTO_INCREMENT PRIMARY KEY, `token` VARCHAR(32) UNIQUE NOT NULL, `end` INT(11) NOT NULL, `group` INT(11) NOT NULL, UNIQUE (`token`));");

        }
        catch (Exception ex)
        {
            Server.PrintToConsole($"CustomPlugins - *[MYSQL ERROR WHILE LOADING: {ex.Message}]*");
        }
    }
    public static void TryBhop(CCSPlayerController controller)
    {
        if (!controller.PawnIsAlive)
            return;
        var buttons = controller.Buttons;
        var client = controller.Index;
        var PP = controller.PlayerPawn.Value;
        var pawn = PP;
        var flags = (PlayerFlags)PP!.Flags;
        if (Bhop[controller.Index] == 1)
        {
            if ((flags & PlayerFlags.FL_ONGROUND) != 0 && (buttons & PlayerButtons.Jump) != 0)
            {
                PP!.AbsVelocity.Z = 300;
                var vel = 400;

                var currentVelocity = new CounterStrikeSharp.API.Modules.Utils.Vector(pawn?.AbsVelocity.X, pawn?.AbsVelocity.Y, pawn?.AbsVelocity.Z);
                var currentSpeed3D = Math.Sqrt(currentVelocity.X * currentVelocity.X + currentVelocity.Y * currentVelocity.Y + currentVelocity.Z * currentVelocity.Z);

                pawn!.AbsVelocity.X = (float)(currentVelocity.X / currentSpeed3D) * vel;
                pawn!.AbsVelocity.Y = (float)(currentVelocity.Y / currentSpeed3D) * vel;
                pawn!.AbsVelocity.Z = (float)(currentVelocity.Z / currentSpeed3D) * vel;
            }
        }
    }

    public void GiveWeapon(CCSPlayerController player, string weapon)
    {
        AddTimer(0.1f, () =>
        {
            player.GiveNamedItem($"weapon_{weapon}");
        });
    }
    public void GiveWeapon_round(CCSPlayerController player)
    {
        var client = player.Index;
        if (Round >= 2)
        {
            if (player.PawnIsAlive)
            {
                if (Selected[client] == 0)
                {
                    Selected_round[client] = 0;
                }
                if (Selected[client] == 1)
                {
                    player.GiveNamedItem("weapon_m4a1");
                    Selected_round[client] = 1;
                }
                if (Selected[client] == 2)
                {
                    player.GiveNamedItem("weapon_m4a1_silence");
                    Selected_round[client] = 1;
                }
                if (Selected[client] == 3)
                {
                    player.GiveNamedItem("weapon_ak47"); 
                    Selected_round[client] = 1;
                }
                if (Selected[client] == 4)
                {
                    player.GiveNamedItem("weapon_awp");
                    Selected_round[client] = 1;
                }
                if (Selected[client] >= 1)
                {
                    player.PrintToChat($" {Config.Prefix} {Localizer["VIPUseGuns"]}");
                }
            }
            else
            {
                player.PrintToChat($" {Config.Prefix} {Localizer["MustBeAlive", "!guns"]}");
            }
        }
    }
    [ConsoleCommand("css_guns", "Set gun")]
    public void open_guns(CCSPlayerController? player, CommandInfo info)
    {
        if (Guns[player!.Index] == 1)
        {
            if (Round >= Config.MinRoundForGuns)
            {
                if(Selected_round[player!.Index] == 1)
                {
                    player.PrintToChat($" {Config.Prefix} {Localizer["VIPUsedGuns"]}");
                    return;
                }
                if (player.PawnIsAlive)
                {
                    guns(player);
                }
                else
                {
                    player.PrintToChat($" {Config.Prefix} {Localizer["MustBeAlive", "!guns"]}");
                }
            }
            else
            {
                player.PrintToChat($" {Config.Prefix} {Localizer["VIPRound", Config.MinRoundForGuns]}");
            }
            SaveSettings(player);
        }
    }
    [ConsoleCommand("css_guns_off", "Set gun")]
    public void guns_off(CCSPlayerController? player, CommandInfo info)
    {
        Selected[player!.Index] = 0;
        player.PrintToChat($" {Config.Prefix} {Localizer["VIPNoMoreGuns"]}");
        SaveSettings(player!);
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
    [ConsoleCommand("css_generatevip", "Generate token for VIP")]
    public void generatevip(CCSPlayerController? player, CommandInfo info)
    {
        var Time = info.GetArg(1);
        var Type = info.GetArg(2);

        if (!AdminManager.PlayerHasPermissions(player, "@css/root"))
        {
            info.ReplyToCommand($" {Config.Prefix} This command are only for admins!");
            return;
        }
        else if (Time == null || Time == "" || !IsInt(Time))
        {
            info.ReplyToCommand($" {Config.Prefix} /generatevip <Days / 0 = Infinite> <Type: 1 = VIP, 2 = MVIP, 3 = Models>");
            return;
        }
        else if (Type == null || Type == "" || !IsInt(Type))
        {
            info.ReplyToCommand($" {Config.Prefix} /generatevip <Days / 0 = Infinite> <Type: 1 = VIP, 2 = MVIP, 3 = Models>");
            return;
        }
        else
        {
            var token = CreatePassword(20);
            var group = Type;
            var time_vip = Time;

            var TimeToUTC = DateTime.UtcNow.AddDays(Convert.ToInt32(time_vip)).GetUnixEpoch();
            var timeofvip = 0;
            if (time_vip == "0")
            {
                timeofvip = 0;
            }
            else
            {
                timeofvip = DateTime.UtcNow.AddDays(Convert.ToInt32(time_vip)).GetUnixEpoch();
            }
            MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);
            MySqlQueryValue values = new MySqlQueryValue()
            .Add("token", token)
            .Add("end", $"{timeofvip}")
            .Add("`group`", group);
            MySql.Table($"deadswim_users_key_vip").Insert(values);

            info.ReplyToCommand($"==========================================");
            info.ReplyToCommand($"You generate new VIP Token");
            info.ReplyToCommand($"Token: {token}");
            info.ReplyToCommand($"Ending (days): {time_vip}");
            info.ReplyToCommand($"Group ID: {group}");
            info.ReplyToCommand($"-- /activator {token} --");
            info.ReplyToCommand($"==========================================");

        }
    }


    [ConsoleCommand("css_activator", "Activate VIP from Tokens")]
    public void CommandActivator(CCSPlayerController? player, CommandInfo info)
    {
        var token = info.ArgByIndex(1);
        if (token == null || token == "" || IsInt(token))
            return;
        if (Bhop[player!.Index] == 1)
        {
            player.PrintToChat($" {Config.Prefix} {Localizer["VIPStatus"]} {Localizer["VIPActive"]}");
            return;
        }

        MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);

        MySqlQueryResult result = MySql!.Table($"deadswim_users_key_vip").Where(MySqlQueryCondition.New("token", "=", $"{token}")).Select();

        if (result.Rows == 1)
        {
            var SteamIDC = player.SteamID;
            var timeofvip = result.Get<int>(0, "end");
            var group_int = result.Get<int>(0, "group");
            if (result.Get<int>(0, "end") == 0)
            {
                timeofvip = 0;
            }
            else
            {
                timeofvip = result.Get<int>(0, "end");
            }
            var client = player.Index;

            if(group_int == 1)
            {
                MySqlQueryValue values = new MySqlQueryValue()
                    .Add("healthshot", "1")
                    .Add("store_credit", "1")
                    .Add("guns", "1")
                    .Add("bhop", "1")
                    .Add("health", "1")
                    .Add("bomb", "1")
                    .Add("timestamp", $"{timeofvip}");
                int rowsAffected = MySql.Table("deadswim_vip").Where($"steamid = '{SteamIDC}'").Update(values);
            }
            if (group_int == 2)
            {
                MySqlQueryValue values = new MySqlQueryValue()
                    .Add("healthshot", "1")
                    .Add("store_credit", "1")
                    .Add("shotlaser", "1")
                    .Add("trials", "1")
                    .Add("guns", "1")
                    .Add("nade", "1")
                    .Add("bhop", "1")
                    .Add("health", "1")
                    .Add("bomb", "1")
                    .Add("mvip", "1")
                    .Add("timestamp", $"{timeofvip}");
                int rowsAffected = MySql.Table("deadswim_vip").Where($"steamid = '{SteamIDC}'").Update(values);
            }
            if (group_int == 3)
            {
                MySqlQueryValue values = new MySqlQueryValue()
                    .Add("models", "1");
                int rowsAffected = MySql.Table("deadswim_vip").Where($"steamid = '{SteamIDC}'").Update(values);
            }
            MySqlQueryValue _Tvalues = new MySqlQueryValue()
            .Add("steam_id", $"{player.SteamID}")
            .Add("end", $"{timeofvip}")
            .Add("`group`", $"{group_int}");
            MySql.Table($"deadswim_users").Insert(_Tvalues);


            player.PrintToChat($" {ChatColors.Lime}=========================================");
            player.PrintToChat($" {Config.Prefix} {Localizer["TokenUse"]}");
            player.PrintToChat($" {ChatColors.Lime}=========================================");
            LoadPlayerData(player);
            MySql.Table($"deadswim_users_key_vip").Where($"token = '{token}'").Delete();
        }
        else
        {
            player.PrintToChat($" {ChatColors.Lime}=========================================");
            player.PrintToChat($" {Config.Prefix} {Localizer["TokenNotExist"]}");
            player.PrintToChat($" {ChatColors.Lime}=========================================");
        }
    }

    [ConsoleCommand("css_addvip", "Add VIP to user")]
    public void addvip(CCSPlayerController? player, CommandInfo info)
    {
        var Type = info.GetArg(3);
        var SteamIDC = info.GetArg(2);
        var TimeSec = info.GetArg(1);

        if (!AdminManager.PlayerHasPermissions(player, "@css/root"))
        {
            info.ReplyToCommand($" {Config.Prefix} This command are only for admins!");
            return;
        }
        else if (SteamIDC == null || SteamIDC == "" || !IsInt(SteamIDC))
        {
            info.ReplyToCommand($" {Config.Prefix} /addvip <Days / 0 = Infinite> <SteamID> <Type: 1 = VIP, 2 = MVIP, 3 = Models>");
            return;
        }
        else if (TimeSec == null || TimeSec == "" || !IsInt(TimeSec))
        {
            info.ReplyToCommand($" {Config.Prefix} /addvip <Days / 0 = Infinite> <SteamID> <Type: 1 = VIP, 2 = MVIP, 3 = Models>");
            return;
        }
        else if (Type == null || Type == "" || !IsInt(Type))
        {
            info.ReplyToCommand($" {Config.Prefix} /addvip <Days / 0 = Infinite> <SteamID> <Type: 1 = VIP, 2 = MVIP, 3 = Models>");
            return;
        }
        else
        {
            MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);

            MySqlQueryResult result = MySql!.Table("deadswim_vip").Where(MySqlQueryCondition.New("steamid", "=", SteamIDC)).Select();
            if (result.Rows == 1)
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
                if (Type == "1")
                {
                    MySqlQueryValue values = new MySqlQueryValue()
                    .Add("healthshot", "1")
                    .Add("store_credit", "1")
                    .Add("guns", "1")
                    .Add("bhop", "1")
                    .Add("health", "1")
                    .Add("bomb", "1")
                    .Add("timestamp", $"{timeofvip}");
                    int rowsAffected = MySql.Table("deadswim_vip").Where($"steamid = '{SteamIDC}'").Update(values);
                    info.ReplyToCommand($" {Config.Prefix} VIP has been activated on SteamID {SteamIDC}");
                }
                else if (Type == "2")
                {
                    MySqlQueryValue values = new MySqlQueryValue()
                    .Add("healthshot", "1")
                    .Add("store_credit", "1")
                    .Add("shotlaser", "1")
                    .Add("trials", "1")
                    .Add("nade", "1")
                    .Add("guns", "1")
                    .Add("bhop", "1")
                    .Add("health", "1")
                    .Add("bomb", "1")
                    .Add("mvip", "1")
                    .Add("timestamp", $"{timeofvip}");
                    int rowsAffected = MySql.Table("deadswim_vip").Where($"steamid = '{SteamIDC}'").Update(values);
                    info.ReplyToCommand($" {Config.Prefix} VIP has been ativated on SteamID {SteamIDC}");
                }
                else if (Type == "3")
                {
                    MySqlQueryValue values = new MySqlQueryValue()
                    .Add("models", "1");
                    int rowsAffected = MySql.Table("deadswim_vip").Where($"steamid = '{SteamIDC}'").Update(values);
                    info.ReplyToCommand($" {Config.Prefix} VIP has been activated on SteamID {SteamIDC}");
                }
                else
                {
                    info.ReplyToCommand($" {Config.Prefix} This VIP not exist!");
                }

            }
        }

    }
    [ConsoleCommand("css_reloadvip", "Reload VIP permission")]
    public void reload_vip(CCSPlayerController? player, CommandInfo info)
    {
        player?.PrintToChat($" {Config.Prefix}{ChatColors.Lime}{Localizer["VIPLoading"]}");
        LoadPlayerData_VIP(player!);
    }
    [ConsoleCommand("css_vip", "Info about VIP")]
    public void VIP_Info(CCSPlayerController? player, CommandInfo info)
    {
        var client = player!.Index;
        int time = (int)Timestamp[client];
        var timeRemainingFormatted = "";
        var timeRemaining = DateTimeOffset.FromUnixTimeSeconds(time) - DateTimeOffset.UtcNow;
        var nowtimeis = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        timeRemainingFormatted = $"{timeRemaining.Days}d {timeRemaining.Hours}:{timeRemaining.Minutes:D2}";
        if(time == 0) { timeRemainingFormatted = "Nikdy"; }

        if (Timestamp[player.Index] == -1)
        {
            player.PrintToChat($" {Config.Prefix} {ChatColors.Red}------------------------");
            player.PrintToChat($" {Config.Prefix} {Localizer["VIPStatus"]} - {Config.Prefix}");
            player.PrintToChat($" {Config.Prefix} {Localizer["VIPRemaining"]} : {ChatColors.Red}{Localizer["VIPInActive"]}");
            player.PrintToChat($" {Config.Prefix} {Localizer["VIPBuy"]}");
            player.PrintToChat($" {Config.Prefix} {ChatColors.Red}------------------------");
        }
        else
        {
            var vip_active = $" {ChatColors.Orange}ERROR";
            var mvip_active = $" {ChatColors.Orange}ERROR";
            var models_active = $" {ChatColors.Orange}ERROR";

            if (Bomb_a[client] == 1) { vip_active = $"{ChatColors.Lime}{Localizer["Yes"]}"; } else { vip_active = $"{ChatColors.Red}{Localizer["No"]}"; }
            if (MVIP[client] == 1) { mvip_active = $"{ChatColors.Lime}{Localizer["Yes"]}"; } else { mvip_active = $"{ChatColors.Red}{Localizer["No"]}"; }
            if (Models[client] == 1) { models_active = $"{ChatColors.Lime}{Localizer["Yes"]}"; } else { models_active = $"{ChatColors.Red}{Localizer["No"]}"; }

            player.PrintToChat($" {Config.Prefix} {ChatColors.Red}------------------------");
            player.PrintToChat($" {Config.Prefix} {Localizer["VIPStatus"]} - {Config.Prefix}");
            player.PrintToChat($" {Config.Prefix} {Localizer["VIPRemaining"]} : {ChatColors.Lime}{timeRemainingFormatted}");
            player.PrintToChat($" {Config.Prefix} {ChatColors.Lime}⟫ {Localizer["VIPService"]} ⟪");
            player.PrintToChat($" {Config.Prefix} {Localizer["VIPActive"]} VIP: {vip_active}");
            player.PrintToChat($" {Config.Prefix} {Localizer["VIPActive"]} M-VIP: {mvip_active}");
            player.PrintToChat($" {Config.Prefix} {Localizer["VIPActive"]} Models: {models_active}");
            player.PrintToChat($" {Config.Prefix} {Localizer["VIPCommands"]} : /shots, /models, /trials, /guns, /reloadvip, /nade");
            player.PrintToChat($" {Config.Prefix} {ChatColors.Red}------------------------");
        }
    }
    public void OnTick(CCSPlayerController controller)
    {
        if (StoreApi == null) throw new Exception("StoreApi could not be located.");

        if (!controller.PawnIsAlive)
            return;
        var pawn = controller.Pawn.Value;
        var flags = (PlayerFlags)pawn!.Flags;
        var client = controller.Index;
        var buttons = controller.Buttons;
        var player = controller;

        if (player.IsBot)
            return;
        if (buttons != 0 || (flags & PlayerFlags.FL_ONGROUND) != 0)
        {
            if (Trials[client] == 1)
            {
                //CounterStrikeSharp.API.Modules.Utils.Vector player_vec = pawn?.AbsOrigin!;
                //CounterStrikeSharp.API.Modules.Utils.Vector end_pos = new CounterStrikeSharp.API.Modules.Utils.Vector(player_vec.X + 5.0f, player_vec.Y + 5.0f, player_vec.Z + 5.0f);
                //DrawTrials(player_vec, end_pos, controller); Only in Prémium
            }
        }
        LF[client] = flags;
        LB[client] = buttons;
    }
    public void ChangeTag(CCSPlayerController player)
    {
        if (Bhop[player.Index] == 1)
        {
            player.Clan = Config.VIPTag_Score;
            Utilities.SetStateChanged(player, "CCSPlayerController", "m_szClan");
            TagsApi?.SetPlayerTag(player, Tags.Tags_Tags.ChatTag, Config.VIPTag_Chat);
        }
        if (MVIP[player.Index] == 1)
        {
            player.Clan = Config.MVIPTag_Score;
            Utilities.SetStateChanged(player, "CCSPlayerController", "m_szClan");
            TagsApi?.SetPlayerTag(player, Tags.Tags_Tags.ChatTag, Config.MVIPTag_Chat);
        }
    }
    [GameEventHandler]
    public HookResult OnClientConnect(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        CCSPlayerController player = @event.Userid!;
        if (player == null || !player.IsValid || player.IsBot) return HookResult.Continue;

        LoadPlayerData(player);
        LoadPlayerData_VIP(player);

        return HookResult.Continue;

    }
    public void LoadVIP(CCSPlayerController player)
    {
        var client = player.Index;
        int time = (int)Timestamp[client];
        var timeRemainingFormatted = "";
        if (time == 0)
        {
            timeRemainingFormatted = "Nikdy";
        }
        else
        {
            var timeRemaining = DateTimeOffset.FromUnixTimeSeconds(time) - DateTimeOffset.UtcNow;
            var nowtimeis = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            timeRemainingFormatted = $"{timeRemaining.Days}d {timeRemaining.Hours}:{timeRemaining.Minutes:D2}";
        }
        if (time == -1)
        {
            timeRemainingFormatted = "Není aktivní";
            if (Models[client] == 1)
            {
                AdminManager.AddPlayerPermissions(player, "@vip/specials");
            }
            return;
        }
        player.PrintToChat($" {Config.Prefix} {ChatColors.Red}------------------------");
        player.PrintToChat($" {Config.Prefix} {Localizer["VIPStatus"]} - {Config.Prefix}");
        player.PrintToChat($" {Config.Prefix} {Localizer["VIPRemaining"]} : {ChatColors.Lime}{timeRemainingFormatted}");
        player.PrintToChat($" {Config.Prefix} {ChatColors.Red}------------------------");

        Server.PrintToConsole($"CustomPlugins - Hráč {player.PlayerName} Má = HS = {Healthshot[player.Index]} TR = {Trials[player.Index]}, Bhop = {Bhop[player.Index]}");
        if (Guns[client] == 1)
        {
            AdminManager.AddPlayerPermissions(player, "@vip/basic");
            AdminManager.AddPlayerPermissions(player, "@vip/text");
            ChangeTag(player);
        }
        if (Models[client] == 1)
        {
            AdminManager.AddPlayerPermissions(player, "@vip/specials");
        }
    }
    public void UnLoadVIP(CCSPlayerController player)
    {
        if (Models[player.Index] == 0)
        {
            AdminManager.RemovePlayerPermissions(player, "@vip/specials");
        }
        AdminManager.RemovePlayerPermissions(player, "@vip/basic");
        LoadPlayerData_VIP(player);
    }
    public void SaveSettings(CCSPlayerController player)
    {
        var client = player.Index;
        MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);

        MySqlQueryValue values = new MySqlQueryValue()
        .Add("trials", $"{UserTrialColor[client]}")
        .Add("guns", $"{Selected[client]}")
        .Add("enable_nade", $"{NadeEnable[client]}")
        .Add("shots", $"{ShotsEnable[client]}")
        .Add("credits", $"{CreditEnable[client]}");
        int rowsAffected = MySql.Table("deadswim_settings").Where($"steamid = '{player.SteamID.ToString()}'").Update(values);

    }
    public void LoadPlayerData_VIP(CCSPlayerController player)
    {
        MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);

        MySqlQueryResult result = MySql!.Table("deadswim_vip").Where(MySqlQueryCondition.New("steamid", "=", player.SteamID.ToString())).Select();
        if (result.Rows == 1)
        {
            var nowtimeis = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (result.Get<int>(0, "timestamp") == -1)
            {
                Server.PrintToConsole($"CustomPlugins - *[Hráč {player.PlayerName} nemá aktivní VIP/MVIP!]*");
            }

            Healthshot[player.Index] = result.Get<int>(0, "healthshot");
            CSStore[player.Index] = result.Get<int>(0, "store_credit");
            Trials[player.Index] = result.Get<int>(0, "trials");
            LaserShot[player.Index] = result.Get<int>(0, "shotlaser");
            NadeTrials[player.Index] = result.Get<int>(0, "nade");
            Guns[player.Index] = result.Get<int>(0, "guns");
            Bhop[player.Index] = result.Get<int>(0, "bhop");
            Health[player.Index] = result.Get<int>(0, "health");
            Models[player.Index] = result.Get<int>(0, "models");
            Bomb_a[player.Index] = result.Get<int>(0, "bomb");
            MVIP[player.Index] = result.Get<int>(0, "mvip");
            Timestamp[player.Index] = result.Get<int>(0, "timestamp");

            LoadVIP(player);


            if (result.Get<int>(0, "timestamp") != 0)
            {
                if (result.Get<int>(0, "timestamp") < nowtimeis && result.Get<int>(0, "timestamp") != -1)
                {
                    MySqlQueryValue values = new MySqlQueryValue()
                    .Add("healthshot", "0")
                    .Add("store_credit", "0")
                    .Add("trials", "0")
                    .Add("shotlaser", "0")
                    .Add("nade", "0")
                    .Add("guns", "0")
                    .Add("bhop", "0")
                    .Add("health", "0")
                    .Add("bomb", "0")
                    .Add("mvip", "0")
                    .Add("timestamp", "-1");
                    int rowsAffected = MySql.Table("deadswim_vip").Where($"steamid = '{player.SteamID.ToString()}'").Update(values);
                    UnLoadVIP(player);
                    Server.PrintToConsole($"CustomPlugins - *[VIP hráči {player.PlayerName} právě končí VIP!]*");
                }
            }
            else
            {
                Server.PrintToConsole($"CustomPlugins - *[VIP hráč {player.PlayerName} má VIP navždy!]*");
            }
        }
        else
        {
            MySqlQueryValue values = new MySqlQueryValue()
            .Add("steamid", player.SteamID.ToString())
            .Add("healthshot", "0")
            .Add("store_credit", "0")
            .Add("trials", "0")
            .Add("nade", "0")
            .Add("shotlaser", "0")
            .Add("guns", "0")
            .Add("bhop", "0")
            .Add("health", "0")
            .Add("models", "0")
            .Add("bomb", "0")
            .Add("mvip", "0")
            .Add("timestamp", "-1");
            MySql.Table("deadswim_vip").Insert(values);
        }
    }
    public void LoadPlayerData(CCSPlayerController player)
    {
        UserTrialColor[player.Index] = 0;
        Selected[player.Index] = 0;
        Selected_round[player.Index] = 0;


        MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);

        MySqlQueryResult result = MySql!.Table("deadswim_settings").Where(MySqlQueryCondition.New("steamid", "=", player.SteamID.ToString())).Select();
        if (result.Rows == 1)
        {
            UserTrialColor[player.Index] = result.Get<int>(0, "trials");
            CreditEnable[player.Index] = result.Get<int>(0, "credits");
            ShotsEnable[player.Index] = result.Get<int>(0, "shots");
            NadeEnable[player.Index] = result.Get<int>(0, "enable_nade");
            Selected[player.Index] = result.Get<int>(0, "guns");
        }
        else
        {
            UserTrialColor[player.Index] = 0;
            Selected[player.Index] = 0;
            MySqlQueryValue values = new MySqlQueryValue()
            .Add("steamid", player.SteamID.ToString())
            .Add("shots", "0")
            .Add("trials", "0")
            .Add("enable_nade", "0")
            .Add("free_vip", "0")
            .Add("credits", "1")
            .Add("guns", "0");
            MySql.Table("deadswim_settings").Insert(values);
        }
    }
}