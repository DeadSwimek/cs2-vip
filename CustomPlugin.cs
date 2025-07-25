using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using CS2MenuManager.API.Enum;
using CS2MenuManager.API.Menu;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Newtonsoft.Json;
using Nexd.MySQL;
using StoreApi;
using System;
using System.Drawing;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TagsApi;
using VIPAPI;
using static CounterStrikeSharp.API.Core.Listeners;
using static StoreApi.Store;
using static TagsApi.Tags;

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
    public override string ModuleVersion => "V. 2.0.0";

    public bool Bomb;
    public bool active_bool;
    public float bombtime;
    public int Round;
    public string ?SitePlant;
    public CounterStrikeSharp.API.Modules.Timers.Timer? timer_ex;
    public CounterStrikeSharp.API.Modules.Timers.Timer? timer_ac;
    private static readonly int?[] UserTrialColor = new int?[64];
    private static readonly int?[] CreditEnable = new int?[64];
    private static readonly int?[] JumpEnable = new int?[64];
    private static readonly int?[] ShotsEnable = new int?[64];
    private static readonly int?[] BhopEnable = new int?[64];
    private static readonly int?[] QuakeEnable = new int?[64];
    private static readonly int?[] HealthEnable = new int?[64];
    private static readonly int?[] BombEnable = new int?[64];
    private static readonly int?[] KillCount = new int?[64];
    private static readonly int?[] Selected = new int?[64];
    private static readonly int?[] Selected_round = new int?[64];
    private static readonly int?[] Selectedr = new int?[64];
    private static readonly int?[] SelectedTag = new int?[64];
    private static readonly int?[] SelectedTag2 = new int?[64];

    private static readonly int?[] IReload = new int?[64];
    private static readonly int?[] Tag = new int?[64];
    private static readonly int?[] DJump = new int?[64];
    private static readonly int?[] Trails = new int?[64];
    private static readonly int?[] NadeEnable = new int?[64];
    private static readonly int?[] NadeTrails = new int?[64];
    private static readonly int?[] CSStore = new int?[64];
    private static readonly int?[] SelectedModel = new int?[64];
    private static readonly int?[] SelectedWings = new int?[64];
    private static readonly int?[] LaserShot = new int?[64];
    private static readonly int?[] Healthshot = new int?[64];
    private static readonly int?[] Reserved = new int?[64];
    private static readonly int?[] Guns = new int?[64];
    private static readonly int?[] falldmg = new int?[64];
    private static readonly int?[] knife = new int?[64];
    private static readonly int?[] Bhop = new int?[64];
    private static readonly int?[] Health = new int?[64];
    private static readonly int?[] Models = new int?[64];
    private static readonly int?[] Wings = new int?[64];
    private static readonly int?[] Bomb_a = new int?[64];
    private static readonly int?[] MVIP = new int?[64];
    private static readonly int?[] AntiFlash = new int?[64];
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
    public CoreAPI CoreAPI { get; set; } = null!;
    private static PluginCapability<IAPI> APICapability { get; } = new("vip:api");
    public IStoreApi? StoreApi { get; set; }
    public ITagApi tagApi = null!;

    public required ConfigBan Config { get; set; }


    public void OnConfigParsed(ConfigBan config)
    {
        Config = config;
    }

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        if (Config.More_Credit || Config.More_Credit2 || Config.More_Credit3)
        {
            StoreApi = IStoreApi.Capability.Get() ?? throw new Exception("StoreApi could not be located.");
        }
        if (Config.EnabledTags)
        {
            tagApi = ITagApi.Capability.Get() ?? throw new Exception("Tags Api not found!");
        }
    }
    public override void Load(bool hotReload)
    {
        CoreAPI = new CoreAPI(this);
        if (CoreAPI != null)
        {
            Capabilities.RegisterPluginCapability(APICapability, () => CoreAPI);
        }
        CreateDatabase();
            #pragma warning disable CS8622
        AddCommand(Config.SettingsCommand, "Open settings menu for VIP", SettingsMenu);
            #pragma warning restore CS8622
        Console.WriteLine("VIP System, created by DeadSwim");
        RegisterEventHandler<EventBombDefused>(EventBombDefused);
        RegisterEventHandler<EventBombExploded>(EventBombExploded);
        RegisterEventHandler<EventBombPlanted>(EventBombPlanted);
        RegisterEventHandler<EventRoundStart>(EventRoundStart);
        RegisterEventHandler<EventPlayerHurt>(EventPlayerHurt);
        RegisterEventHandler<EventPlayerSpawn>(EventPlayerSpawn);
        RegisterEventHandler<EventPlayerDeath>(EventPlayerDeath);
        RegisterEventHandler<EventWarmupEnd>(EventWarmupEnd);

        RegisterEventHandler<EventPlayerBlind>(EventPlayerBlind);

        RegisterListener<Listeners.OnMapEnd>(() => {
            timer_ac?.Kill();
            timer_ex?.Kill();
        });

        RegisterListener<Listeners.OnMapStart>(name =>
        {
            //change_cvar("bot_quota", "2");
            //change_cvar("bot_join_after_player", "0");
            active_bool = false;
            Round = 0;
        });
        //RegisterListener<OnEntityCreated>(OnEntityCreated); Only in Premium
        RegisterListener<Listeners.OnTick>(LoadOnTick);
    }
    public override void Unload(bool hotReload)
    {
        DeregisterEventHandler<EventBombDefused>(EventBombDefused);
        DeregisterEventHandler<EventBombExploded>(EventBombExploded);
        DeregisterEventHandler<EventBombPlanted>(EventBombPlanted);
        DeregisterEventHandler<EventRoundStart>(EventRoundStart);
        DeregisterEventHandler<EventPlayerHurt>(EventPlayerHurt);
        DeregisterEventHandler<EventPlayerSpawn>(EventPlayerSpawn);
        DeregisterEventHandler<EventPlayerDeath>(EventPlayerDeath);

        DeregisterEventHandler<EventPlayerBlind>(EventPlayerBlind);


        RemoveListener<Listeners.OnTick>(LoadOnTick);

        timer_ac?.Kill();
        timer_ex?.Kill();
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

            MySql.ExecuteNonQueryAsync(@"CREATE TABLE IF NOT EXISTS `deadswim_settings` (`id` INT AUTO_INCREMENT PRIMARY KEY, `steamid` VARCHAR(32) UNIQUE NOT NULL, `enable_quake` INT(11) NOT NULL, `wings` INT(11) NOT NULL,`tag` INT(11) NOT NULL, `tag2` INT(11) NOT NULL, `enable_djump` INT(11) NOT NULL, `model` INT(11) NOT NULL, `bomb` INT(11) NOT NULL, `health` INT(11) NOT NULL, `bhop` INT(11) NOT NULL, `free_vip` INT(11) NOT NULL, `shots` INT(11) NOT NULL, `enable_nade` INT(11) NOT NULL, `trials` INT(11) NOT NULL, `guns` INT(11) NOT NULL,  `credits` VARCHAR(255) NOT NULL, UNIQUE (`steamid`));");
            MySql.ExecuteNonQueryAsync(@"CREATE TABLE IF NOT EXISTS `deadswim_vip` (`id` INT AUTO_INCREMENT PRIMARY KEY, `steamid` VARCHAR(32) UNIQUE NOT NULL, `wings` INT(11) NOT NULL, `reserved` INT(11) NOT NULL,`tag` INT(11) NOT NULL, `healthshot` INT(11) NOT NULL, `reloading` INT(11) NOT NULL, `antiflash` INT(11) NOT NULL, `reload` INT(11) NOT NULL, `jump` INT(11) NOT NULL, `falldmg` INT(11) NOT NULL, `knife` INT(11) NOT NULL, `nade` INT(11) NOT NULL, `store_credit` INT(11) NOT NULL, `trials` INT(11) NOT NULL, `shotlaser` INT(11) NOT NULL, `guns` INT(11) NOT NULL, `bhop` INT(11) NOT NULL, `models` INT(11) NOT NULL,  `health` INT(11) NOT NULL, `bomb` INT(11) NOT NULL, `mvip` INT(11) NOT NULL, `timestamp` INT(11) NOT NULL, UNIQUE (`steamid`));");
            MySql.ExecuteNonQueryAsync(@"CREATE TABLE IF NOT EXISTS `deadswim_users_key_vip` (`id` INT AUTO_INCREMENT PRIMARY KEY, `token` VARCHAR(32) UNIQUE NOT NULL, `end` INT(11) NOT NULL, `group` INT(11) NOT NULL, UNIQUE (`token`));");
            MySql.ExecuteNonQueryAsync(@$"CREATE TABLE IF NOT EXISTS `deadswim_models` (`id` INT AUTO_INCREMENT PRIMARY KEY, `name` VARCHAR(32) UNIQUE NOT NULL, `permission` VARCHAR(32) NOT NULL, `side` VARCHAR(32) NOT NULL, `path` VARCHAR(128) UNIQUE NOT NULL, UNIQUE (`id`));");
            MySql.ExecuteNonQueryAsync(@$"CREATE TABLE IF NOT EXISTS `deadswim_wings` (`id` INT AUTO_INCREMENT PRIMARY KEY, `name` VARCHAR(32) UNIQUE NOT NULL, `permission` VARCHAR(32) NOT NULL, `side` VARCHAR(32) NOT NULL, `path` VARCHAR(128) UNIQUE NOT NULL, UNIQUE (`id`));");
            MySql.ExecuteNonQueryAsync(@$"CREATE TABLE IF NOT EXISTS `deadswim_tags` (`id` INT AUTO_INCREMENT PRIMARY KEY, `tag` VARCHAR(32) NOT NULL, `permission` VARCHAR(32) NOT NULL, `type` VARCHAR(32) NOT NULL, UNIQUE (`id`));");
            
            using (var connection = new MySqlConnection($"Server={Config.DBHost};Database={Config.DBDatabase};Uid={Config.DBUser};Pwd={Config.DBPassword};"))
            {
                connection.Open();
                EnsureColumns(connection, Config.DBDatabase);
                EnsureColumns_Settings(connection, Config.DBDatabase);
            }
        }
        catch (Exception ex)
        {
            Server.PrintToConsole($"CustomPlugins - *[MYSQL ERROR WHILE LOADING: {ex.Message}]*");
        }
    }
    public static void EnsureColumns(MySqlConnection connection, string db)
    {
        string databaseName = $"{db}";
        string tableName = "deadswim_vip";

        var columnsToEnsure = new Dictionary<string, string>
        {
            { "tag", "INT(11) NOT NULL DEFAULT 0" },
            { "healthshot", "INT(11) NOT NULL DEFAULT 0" },
            { "reloading", "INT(11) NOT NULL DEFAULT 0" },
            { "antiflash", "INT(11) NOT NULL DEFAULT 0" },
            { "reload", "INT(11) NOT NULL DEFAULT 0" },
            { "jump", "INT(11) NOT NULL DEFAULT 0" },
            { "falldmg", "INT(11) NOT NULL DEFAULT 0" },
            { "knife", "INT(11) NOT NULL DEFAULT 0" },
            { "nade", "INT(11) NOT NULL DEFAULT 0" },
            { "reserved", "INT(11) NOT NULL DEFAULT 0" },
            { "wings", "INT(11) NOT NULL DEFAULT 0" }
        };

        var existingColumns = new HashSet<string>();

        string columnQuery = $@"
            SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = '{databaseName}' AND TABLE_NAME = '{tableName}';
        ";

        using (var cmd = new MySqlCommand(columnQuery, connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                existingColumns.Add(reader.GetString(0));
            }
        }

        foreach (var column in columnsToEnsure)
        {
            if (!existingColumns.Contains(column.Key))
            {
                string alterSql = $"ALTER TABLE `{tableName}` ADD COLUMN `{column.Key}` {column.Value};";
                using (var alterCmd = new MySqlCommand(alterSql, connection))
                {
                    alterCmd.ExecuteNonQuery();
                    Console.WriteLine($"✅ Adding in database: {column.Key}");
                }
            }
        }
    }
    public static void EnsureColumns_Settings(MySqlConnection connection, string db)
    {
        string databaseName = $"{db}";
        string tableName = "deadswim_settings";

        var columnsToEnsure = new Dictionary<string, string>
        {
            { "wings", "INT(11) NOT NULL DEFAULT 0" }
        };

        var existingColumns = new HashSet<string>();

        string columnQuery = $@"
            SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = '{databaseName}' AND TABLE_NAME = '{tableName}';
        ";

        using (var cmd = new MySqlCommand(columnQuery, connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                existingColumns.Add(reader.GetString(0));
            }
        }

        foreach (var column in columnsToEnsure)
        {
            if (!existingColumns.Contains(column.Key))
            {
                string alterSql = $"ALTER TABLE `{tableName}` ADD COLUMN `{column.Key}` {column.Value};";
                using (var alterCmd = new MySqlCommand(alterSql, connection))
                {
                    alterCmd.ExecuteNonQuery();
                    Console.WriteLine($"✅ Adding in database: {column.Key}");
                }
            }
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
                if (Selected_round[player!.Index] == 1)
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
    [ConsoleCommand("css_testvip", "Test VIP")]
    public void testvip(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null) return;
        if (Config.AllowTestVIP == false) return;
        if (TryedVIP(player))
        {
            player.PrintToChat($" {Config.Prefix} {Localizer["TryedVIP"]}");
            return;
        }
        if (IsPlayerVip(player))
        {
            player.PrintToChat($" {Config.Prefix} {Localizer["HaveVIP"]}");
            return;
        }
        else
        {
            MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);

            var SteamIDC = player.SteamID.ToString();
            var TimeSec = Config.DaysTestVIP;

            MySqlQueryResult result = MySql!.Table("deadswim_vip").Where(MySqlQueryCondition.New("steamid", "=", SteamIDC)).Select();
            if (result.Rows == 1)
            {
                int timeofvip;
                var TimeToUTC = DateTime.UtcNow.AddDays(Convert.ToInt32(TimeSec)).GetUnixEpoch();
                timeofvip = DateTime.UtcNow.AddDays(Convert.ToInt32(TimeSec)).GetUnixEpoch();
                var values = new Func<MySqlQueryValue>(() =>
                {
                    var v = new MySqlQueryValue();
                    foreach (var per in Config.VIPs)
                        v.Add(per.permission, per.value.ToString());
                    v.Add("timestamp", $"{timeofvip}");
                    return v;
                })();

                int rowsAffected = MySql.Table("deadswim_vip")
                        .Where($"steamid = '{SteamIDC}'")
                        .Update(values);

                MySqlQueryValue values2 = new MySqlQueryValue()
                .Add("free_vip", "1");
                int rowsAffected2 = MySql.Table("deadswim_settings")
                        .Where($"steamid = '{SteamIDC}'")
                        .Update(values2);
                player.PrintToChat($" {Config.Prefix} {Localizer["UsedTestVIP", Config.DaysTestVIP]}");
            }
        }
    }
    [ConsoleCommand("css_models", "Set Trails color")]
    public void Models_command(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null) return;
        open_Models(player);
    }
    public void open_Models(CCSPlayerController player)
    {
        if (player == null) return;
        if (!Config.ModelsEnabled) return;

        MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);

        MySqlQueryResult result = MySql!
            .Table("deadswim_models")
            .Select();
        ScreenMenu menu = new("Models", this);
        menu.AddItem("OFF", (p, option) =>
        {
            SelectedModel[player.Index] = 0;
            player.PrintToChat($" {Config.Prefix} {Localizer["SkinNextRound", "OFF"]} ");
            SaveSettings(player);
        });

        for (int i = 0; i < result.Rows; i++)
        {
            var row = result[i];
            if (row == null) return;


            string id_model = Convert.ToString(row["id"]) ?? string.Empty;
            string name_model = Convert.ToString(row["name"]) ?? string.Empty;
            string permission = Convert.ToString(row["permission"]) ?? string.Empty;
            string path = Convert.ToString(row["path"]) ?? string.Empty;
            string side = Convert.ToString(row["side"]) ?? string.Empty;

            var get_permission = AdminManager.PlayerHasPermissions(player, permission);
            var get_playerteam = player.Team;
            var team = "none";

            if (get_playerteam == CsTeam.Terrorist) { team = "t"; }
            if (get_playerteam == CsTeam.CounterTerrorist) { team = "ct"; }

            if (get_permission && side == team)
            {
                menu.AddItem($"{name_model} - {side}", (p, option) =>
                {
                    SelectedModel[player.Index] = Convert.ToInt32(id_model);
                    player.PrintToChat($" {Config.Prefix} {Localizer["SkinNextRound", name_model]} ");
                    SaveSettings(player);
                });
            }
            else if (get_permission && side == "all")
            {
                menu.AddItem($"{name_model} - {side}", (p, option) =>
                {
                    SelectedModel[player.Index] = Convert.ToInt32(id_model);
                    player.PrintToChat($" {Config.Prefix} {Localizer["SkinNextRound", name_model]} ");
                    SaveSettings(player);
                });
            }

        }
        menu.Display(player, 0);

    }
    public void SettingsMenu(CCSPlayerController player, CommandInfo info)
    {
        if (player == null) return;
        open_Settings(player);
    }
    public void open_Settings(CCSPlayerController player)
    {
        if (player == null) return;

        var client = player.Index;

        var guns_status = "-";
        var Trails_status = "-";
        var shots_status = "-";
        var nade_status = "-";
        var bhop_status = "-";
        var jump_status = "-";
        var health_status = "-";
        var bomb_status = "-";

        if (Selected[client] >= 1) { guns_status = $"{Localizer["TurnOn"]} !guns_off"; } else { guns_status = $"{Localizer["TurnOff"]} !guns"; }
        if (UserTrialColor[client] == 0) { Trails_status = Localizer["TurnOff"]; } else { Trails_status = Localizer["TurnOn"]; }
        if (ShotsEnable[client] == 0) { shots_status = Localizer["TurnOff"]; } else { shots_status = Localizer["TurnOn"]; }
        if (BhopEnable[client] == 0) { bhop_status = Localizer["TurnOff"]; } else { bhop_status = Localizer["TurnOn"]; }
        if (NadeEnable[client] == 0) { nade_status = Localizer["TurnOff"]; } else { nade_status = Localizer["TurnOn"]; }
        if (JumpEnable[client] == 0) { jump_status = Localizer["TurnOff"]; } else { jump_status = Localizer["TurnOn"]; }
        if (BombEnable[client] == 0) { bomb_status = Localizer["TurnOff"]; } else { bomb_status = Localizer["TurnOn"]; }
        if (HealthEnable[client] == 0) { health_status = Localizer["TurnOff"]; } else { health_status = Localizer["TurnOn"]; }

        ScreenMenu menu = new("Settings\r\n", this);
        if (Config.EnabledGuns && Guns[player.Index] == 1)
        {
            menu.AddItem($"Weapon Menu", (p, option) => { });
        }
        if (Config.ModelsEnabled && Models[player.Index] == 1)
        {
            menu.AddItem($"Models Menu", (p, option) => { open_Models(player); });
        }
        if (Config.EnabledTags && Tag[player.Index] == 1)
        {
            menu.AddItem($"Tag Menu\r\n___________", (p, option) => { open_Tags(player); });
        }

        if (Config.EnabledBhop && Bhop[player.Index] == 1)
        {
            menu.AddItem($"Bhop - {bhop_status}", (p, option) =>
            {
                if (BhopEnable[client] == 0)
                {
                    BhopEnable[client] = 1;
                }
                else if (BhopEnable[client] == 1)
                {
                    BhopEnable[client] = 0;
                }
                SaveSettings(player);
            });
        }
        if (Config.EnabledDoubbleJump && DJump[player.Index] == 1)
        {
            menu.AddItem($"Doubble Jump - {jump_status}", (p, option) =>
            {
                if (JumpEnable[client] == 0)
                {
                    JumpEnable[client] = 1;
                }
                else if (JumpEnable[client] == 1)
                {
                    JumpEnable[client] = 0;
                }
                SaveSettings(player);
            });
        }

        if (Bomb_a[player.Index] == 1)
        {
            menu.AddItem($"Bomb Info - {bomb_status}", (p, option) =>
            {
                if (BombEnable[client] == 0)
                {
                    BombEnable[client] = 1;
                }
                else if (BombEnable[client] == 1)
                {
                    BombEnable[client] = 0;
                }
                SaveSettings(player);
            });
        }
        if (Healthshot[player.Index] == 1)
        {
            menu.AddItem($"HealthShot - {health_status}", (p, option) =>
            {
                if (HealthEnable[client] == 0)
                {
                    HealthEnable[client] = 1;
                }
                else if (HealthEnable[client] == 1)
                {
                    HealthEnable[client] = 0;
                }
                SaveSettings(player);
            });
        }

        menu.Display(player, 0);
    }
    [ConsoleCommand("css_generatevip", "Generate token for VIP")]
    public void generatevip(CCSPlayerController? player, CommandInfo info)
    {
        var Time = info.GetArg(1);
        var Type = info.GetArg(2);

        if (!AdminManager.PlayerHasPermissions(player, Config.AdminPermissions))
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
    [ConsoleCommand("css_addmodel", "Add Model IN DB")]
    public void addmodel(CCSPlayerController? player, CommandInfo info)
    {
        var Name = info.GetArg(4);
        var Permission = info.GetArg(3);
        var Side = info.GetArg(2);
        var Path = info.GetArg(1);

        if (!AdminManager.PlayerHasPermissions(player, Config.AdminPermissions))
        {
            info.ReplyToCommand($" {Config.Prefix} This command are only for admins!");
            return;
        }
        else if (Path == null || Path == "" || Side == null || Side == "" || Permission == null || Permission == "" || Name == null || Name == "")
        {
            info.ReplyToCommand($" {Config.Prefix} /addmodel <Path> <Side(t,ct,all)> <@vip/basic> <Name>");
            return;
        }
        else
        {
            MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);
            MySqlQueryValue values = new MySqlQueryValue()
            .Add("name", $"{Name}")
            .Add("permission", $"{Permission}")
            .Add("path", $"{Path}")
            .Add("side", $"{Side}");
            MySql.Table("deadswim_models").Insert(values);
            info.ReplyToCommand($" {Config.Prefix} Model added in DTB");
        }

    }
    [ConsoleCommand("css_addtag", "Add Tag IN DB")]
    public void addtag(CCSPlayerController? player, CommandInfo info)
    {
        var Name = info.GetArg(3);
        var Permission = info.GetArg(2);
        var Type = info.GetArg(1);

        if (!AdminManager.PlayerHasPermissions(player, Config.AdminPermissions))
        {
            info.ReplyToCommand($" {Config.Prefix} This command are only for admins!");
            return;
        }
        else if (Name == null || Permission == null || Type == null || Name == "" || Permission == "" || Type == "")
        {
            info.ReplyToCommand($" {Config.Prefix} /addtag <Type (1=ScoreBoard, 2=Chat)> <Permission/SteamID> <Name/Tag>");
            return;
        }
        else
        {
            MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);
            MySqlQueryValue values = new MySqlQueryValue()
            .Add("tag", $"{Name}")
            .Add("permission", $"{Permission}")
            .Add("type", $"{Type}");
            MySql.Table("deadswim_tags").Insert(values);
            info.ReplyToCommand($" {Config.Prefix} Tag added in DTB");
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
                var values = new Func<MySqlQueryValue>(() =>
                {
                    var v = new MySqlQueryValue();
                    foreach (var per in Config.VIPs)
                        v.Add(per.permission, per.value.ToString());
                    v.Add("timestamp", $"{timeofvip}");
                    return v;
                })();

                int rowsAffected = MySql.Table("deadswim_vip")
                    .Where($"steamid = '{SteamIDC}'")
                    .Update(values);
            }
            if (group_int == 2)
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
                        .Add("falldmg", "1")
                        .Add("knife", "1")
                        .Add("jump", "1")
                        .Add("reloading", "1")
                        .Add("tag", "1")
                        .Add("antiflash", "1")
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

        if (!AdminManager.PlayerHasPermissions(player, Config.AdminPermissions))
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
                    var values = new Func<MySqlQueryValue>(() =>
                    {
                        var v = new MySqlQueryValue();
                        foreach (var per in Config.VIPs)
                            v.Add(per.permission, per.value.ToString());
                        v.Add("timestamp", $"{timeofvip}");
                        return v;
                    })();

                    int rowsAffected = MySql.Table("deadswim_vip")
                        .Where($"steamid = '{SteamIDC}'")
                        .Update(values);
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
                        .Add("falldmg", "1")
                        .Add("knife", "1")
                        .Add("jump", "1")
                        .Add("reloading", "1")
                        .Add("tag", "1")
                        .Add("antiflash", "1")
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
        int time = (int)Timestamp[client]!;
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
        if (Config.More_Credit || Config.More_Credit2 || Config.More_Credit3)
        {
            if (StoreApi == null) throw new Exception("StoreApi could not be located.");
        }

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
            if (Trails[client] == 1)
            {
                //CounterStrikeSharp.API.Modules.Utils.Vector player_vec = pawn?.AbsOrigin!;
                //CounterStrikeSharp.API.Modules.Utils.Vector end_pos = new CounterStrikeSharp.API.Modules.Utils.Vector(player_vec.X + 5.0f, player_vec.Y + 5.0f, player_vec.Z + 5.0f);
                //DrawTrials(player_vec, end_pos, controller); Only in Prémium
            }
        }
        if (JumpEnable[client] == 1 && Config.EnabledDoubbleJump && DJump[client] == 1)
        {
            if ((LF[client] & PlayerFlags.FL_ONGROUND) != 0 && (flags & PlayerFlags.FL_ONGROUND) == 0 &&
            (buttons & PlayerButtons.Jump) == 0 && (buttons & PlayerButtons.Jump) != 0)
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
    public void ChangeTag(CCSPlayerController player)
    {
        if (!Config.EnabledTags) return;
        MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);

        MySqlQueryResult result = MySql!
            .Table("deadswim_tags")
            .Select();
        for (int i = 0; i < result.Rows; i++)
        {
            var row = result[i];
            if (row == null) return;


            string id_tag = Convert.ToString(row["id"]) ?? string.Empty;
            string name_tag = Convert.ToString(row["tag"]) ?? string.Empty;
            string permission = Convert.ToString(row["permission"]) ?? string.Empty;
            string type = Convert.ToString(row["type"]) ?? string.Empty;

            if (IsInt(permission))
            {
                var Get_Player_ID = player.SteamID.ToString();
                if (permission == Get_Player_ID && SelectedTag[player.Index] == Convert.ToInt32(id_tag))
                {
                    if (Convert.ToInt32(type) == 1)
                    {
                        // Scoreboard
                        player.Clan = name_tag;
                        Utilities.SetStateChanged(player, "CCSPlayerController", "m_szClan");
                    }
                }
                if (permission == Get_Player_ID && SelectedTag2[player.Index] == Convert.ToInt32(id_tag))
                {
                    if (Convert.ToInt32(type) == 2)
                    {
                        // Chat - pomocí tagApi
                        tagApi.SetAttribute(player, TagType.ChatTag, $"{name_tag} ");
                    }
                }
            }
            else
            {
                var get_permission = AdminManager.PlayerHasPermissions(player, permission);
                if (get_permission && SelectedTag[player.Index] == Convert.ToInt32(id_tag))
                {
                    if (Convert.ToInt32(type) == 1)
                    {
                        // Scoreboard
                        player.Clan = name_tag;
                        Utilities.SetStateChanged(player, "CCSPlayerController", "m_szClan");
                    }
                }
                if (get_permission && SelectedTag2[player.Index] == Convert.ToInt32(id_tag))
                {
                    if (Convert.ToInt32(type) == 2)
                    {
                        // Chat - pomocí tagApi
                        tagApi.SetAttribute(player, TagType.ChatTag, $"{name_tag} ");
                    }
                }
            }
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

        int time = (int)Timestamp[client]!;


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

        if (Guns[client] == 1)
        {
            AdminManager.AddPlayerPermissions(player, "@vip/basic");
            AdminManager.AddPlayerPermissions(player, "@vip/text");
            ChangeTag(player);
        }
        if (MVIP[client] == 1)
        {
            AdminManager.AddPlayerPermissions(player, "@vip/mvip");
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
        if (player == null) return;

        var client = player.Index;
        MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);

        MySqlQueryValue values = new MySqlQueryValue()
        .Add("trials", $"{UserTrialColor[client]}")
        .Add("guns", $"{Selected[client]}")
        .Add("bhop", $"{BhopEnable[client]}")
        .Add("enable_nade", $"{NadeEnable[client]}")
        .Add("enable_quake", $"{QuakeEnable[client]}")
        .Add("shots", $"{ShotsEnable[client]}")
        .Add("enable_djump", $"{JumpEnable[client]}")
        .Add("model", $"{SelectedModel[client]}")
        .Add("wings", $"{SelectedWings[client]}")
        .Add("credits", $"{CreditEnable[client]}")
        .Add("bomb", $"{BombEnable[client]}")
        .Add("health", $"{HealthEnable[client]}")
        .Add("tag", $"{SelectedTag[client]}")
        .Add("tag2", $"{SelectedTag2[client]}");
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
            Tag[player.Index] = result.Get<int>(0, "tag");
            CSStore[player.Index] = result.Get<int>(0, "store_credit");
            Trails[player.Index] = result.Get<int>(0, "trials");
            LaserShot[player.Index] = result.Get<int>(0, "shotlaser");
            NadeTrails[player.Index] = result.Get<int>(0, "nade");
            Guns[player.Index] = result.Get<int>(0, "guns");
            Bhop[player.Index] = result.Get<int>(0, "bhop");
            Health[player.Index] = result.Get<int>(0, "health");
            Models[player.Index] = result.Get<int>(0, "models");
            Wings[player.Index] = result.Get<int>(0, "wings");
            Bomb_a[player.Index] = result.Get<int>(0, "bomb");
            MVIP[player.Index] = result.Get<int>(0, "mvip");
            knife[player.Index] = result.Get<int>(0, "knife");
            falldmg[player.Index] = result.Get<int>(0, "falldmg");
            DJump[player.Index] = result.Get<int>(0, "jump");
            IReload[player.Index] = result.Get<int>(0, "reloading");
            Reserved[player.Index] = result.Get<int>(0, "reserved");
            AntiFlash[player.Index] = result.Get<int>(0, "antiflash");
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
                    .Add("knife", "0")
                    .Add("wings", "0")
                    .Add("falldmg", "0")
                    .Add("mvip", "0")
                    .Add("reserved", "0")
                    .Add("jump", "0")
                    .Add("reloading", "0")
                    .Add("tag", "0")
                    .Add("antiflash", "0")
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
            .Add("knife", "0")
            .Add("wings", "0")
            .Add("falldmg", "0")
            .Add("mvip", "0")
            .Add("jump", "0")
            .Add("reloading", "0")
            .Add("reserved", "0")
            .Add("reload", "0")
            .Add("tag", "0")
            .Add("antiflash", "0")
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
            BhopEnable[player.Index] = result.Get<int>(0, "bhop");
            ShotsEnable[player.Index] = result.Get<int>(0, "shots");
            NadeEnable[player.Index] = result.Get<int>(0, "enable_nade");
            JumpEnable[player.Index] = result.Get<int>(0, "enable_djump");
            QuakeEnable[player.Index] = result.Get<int>(0, "enable_quake");
            Selected[player.Index] = result.Get<int>(0, "guns");
            SelectedModel[player.Index] = result.Get<int>(0, "model");
            SelectedWings[player.Index] = result.Get<int>(0, "wings");
            BombEnable[player.Index] = result.Get<int>(0, "bomb");
            HealthEnable[player.Index] = result.Get<int>(0, "health");
            SelectedTag[player.Index] = result.Get<int>(0, "tag");
            SelectedTag2[player.Index] = result.Get<int>(0, "tag2");
        }
        else
        {
            UserTrialColor[player.Index] = 0;
            Selected[player.Index] = 0;
            MySqlQueryValue values = new MySqlQueryValue()
            .Add("steamid", player.SteamID.ToString())
            .Add("shots", "1")
            .Add("trials", "0")
            .Add("bhop", "1")
            .Add("enable_djump", "1")
            .Add("enable_nade", "1")
            .Add("enable_quake", "1")
            .Add("free_vip", "0")
            .Add("model", "0")
            .Add("credits", "1")
            .Add("bomb", "1")
            .Add("wings", "0")
            .Add("health", "1")
            .Add("tag", "0")
            .Add("tag2", "0")
            .Add("guns", "0");
            MySql.Table("deadswim_settings").Insert(values);
        }
    }
}