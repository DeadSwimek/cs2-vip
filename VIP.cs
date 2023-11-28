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

namespace VIP;
[MinimumApiVersion(55)]
public class ConfigVIP : BasePluginConfig
{
    [JsonPropertyName("Prefix")] public string Prefix { get; set; } = $" {ChatColors.Default}[{ChatColors.Green}MadGames.eu{ChatColors.Default}]";
    [JsonPropertyName("GiveHPAfterKill")] public bool GiveHPAfterKill { get; set; } = true;
    [JsonPropertyName("GiveMoneyAfterKill")] public bool GiveMoneyAfterKill { get; set; } = true;
    [JsonPropertyName("AllowKillMessages")] public bool AllowKillMessages { get; set; } = true;
    [JsonPropertyName("EnableVIPPrefix")] public bool EnableVIPPrefix { get; set; } = true;
    [JsonPropertyName("EnableVIPAcceries")] public bool EnableVIPAcceries { get; set; } = true;
    [JsonPropertyName("EnableVIPColoredSmokes")] public bool EnableVIPColoredSmokes { get; set; } = true;
    [JsonPropertyName("EnableFalldamage")] public bool EnableFalldamage { get; set; } = false;
    [JsonPropertyName("RespawnAllowed")] public bool RespawnAllowed { get; set; } = true;
    [JsonPropertyName("DetonateRewards")] public bool DetonateRewards { get; set; } = true;
    [JsonPropertyName("EnableDoubbleJump")] public bool EnableDoubbleJump { get; set; } = true;
    [JsonPropertyName("KnifeDMGEnable")] public bool KnifeDMGEnable { get; set; } = false;
    [JsonPropertyName("WelcomeMessageEnable")] public bool WelcomeMessageEnable { get; set; } = true;
    [JsonPropertyName("ReservedSlotsForVIP")] public int ReservedSlotsForVIP { get; set; } = 1;
    [JsonPropertyName("ReservedMethod")] public int ReservedMethod { get; set; } = 1;
    [JsonPropertyName("Bombinfo")] public bool Bombinfo { get; set; } = true;
    [JsonPropertyName("DisablePackWeaponAfter20Sec")] public bool DisablePackWeaponAfter20Sec { get; set; } = false;


    [JsonPropertyName("WelcomeMessage")] public string WelcomeMessage { get; set; } = $"Welcom on server you are BEST VIP!";
    [JsonPropertyName("DBDatabase")] public string DBDatabase { get; set; } = "database";
    [JsonPropertyName("DBUser")] public string DBUser { get; set; } = "user";
    [JsonPropertyName("DBPassword")] public string DBPassword { get; set; } = "password";
    [JsonPropertyName("DBHost")] public string DBHost { get; set; } = "localhost";
    [JsonPropertyName("DBPort")] public int DBPort { get; set; } = 3306;

    [JsonPropertyName("translation")] public TranslationClass TranslationClass { get; set; } = new TranslationClass();
    [JsonPropertyName("money")] public RewardsClass RewardsClass { get; set; } = new RewardsClass();

    [JsonPropertyName("pack1")] public Pack1Settings Pack1Settings { get; set; } = new Pack1Settings();
    [JsonPropertyName("pack2")] public Pack2Settings Pack2Settings { get; set; } = new Pack2Settings();

}
public class RewardsClass
{
    [JsonPropertyName("FirstSpawnMoney")] public int FirstSpawnMoney { get; set; } = 1200;
    [JsonPropertyName("SpawnArmor")] public int SpawnArmor { get; set; } = 100;
    [JsonPropertyName("SpawnHP")] public int SpawnHP { get; set; } = 110;
    [JsonPropertyName("KillHP")] public int KillHP { get; set; } = 10;
    [JsonPropertyName("KillMoney")] public int KillMoney { get; set; } = 300;
    [JsonPropertyName("DetonateMoney")] public int DetonateMoney { get; set; } = 300;


}
public class Pack1Settings
{
    [JsonPropertyName("Gun")] public string Gun { get; set; } = "ak47";
    [JsonPropertyName("Pistol")] public string Pistol { get; set; } = "deagle";
    [JsonPropertyName("Acceroies")] public string Acceroies { get; set; } = "healthshot";
    [JsonPropertyName("Acceroies_2")] public string Acceroies_2 { get; set; } = "molotov";
    [JsonPropertyName("Acceroies_3")] public string Acceroies_3 { get; set; } = "smokegrenade";
    [JsonPropertyName("Acceroies_4")] public string Acceroies_4 { get; set; } = "hegrenade";

}
public class Pack2Settings
{
    [JsonPropertyName("Gun")] public string Gun { get; set; } = "m4a1";
    [JsonPropertyName("Pistol")] public string Pistol { get; set; } = "deagle";
    [JsonPropertyName("Acceroies")] public string Acceroies { get; set; } = "healthshot";
    [JsonPropertyName("Acceroies_2")] public string Acceroies_2 { get; set; } = "molotov";
    [JsonPropertyName("Acceroies_3")] public string Acceroies_3 { get; set; } = "smokegrenade";
    [JsonPropertyName("Acceroies_4")] public string Acceroies_4 { get; set; } = "hegrenade";


}
public class TranslationClass
{
    [JsonPropertyName("OnceUse")] public string OnceUse { get; set; } = $" This command you can use {ChatColors.Red}only once{ChatColors.Default} on round!";
    [JsonPropertyName("MustBeVIP")] public string MustBeVIP { get; set; } = $" This command are allowed only for {ChatColors.Lime}VIP{ChatColors.Default}!";
    [JsonPropertyName("MustBeThird")] public string MustBeThird { get; set; } = $" Must be a {ChatColors.Red}Third{ChatColors.Default} round, to use this command!";

    [JsonPropertyName("Pack1")] public string Pack1 { get; set; } = $" You got a Packages {ChatColors.Lime}number one{ChatColors.Default}.";
    [JsonPropertyName("Pack2")] public string Pack2 { get; set; } = $" You got a Packages {ChatColors.Lime}number two{ChatColors.Default}.";

    [JsonPropertyName("WeaponAK")] public string WeaponAK { get; set; } = $" You got a weapon {ChatColors.Lime}AK-47{ChatColors.Default}.";
    [JsonPropertyName("WeaponM4A1")] public string WeaponM4A1 { get; set; } = $" You got a weapon {ChatColors.Lime}M4A1{ChatColors.Default}.";
    [JsonPropertyName("WeaponM4A1S")] public string WeaponM4A1S { get; set; } = $" You got a weapon {ChatColors.Lime}M4A1-S{ChatColors.Default}.";
    [JsonPropertyName("WeaponAWP")] public string WeaponAWP { get; set; } = $" You got a weapon {ChatColors.Lime}AWP{ChatColors.Default}.";
    [JsonPropertyName("Autoguns")] public string Autoguns { get; set; } = $" <font color:'green'>If you wanna turn off automaticall weapon type</font><font color:'red'> /guns_off</font>";
    [JsonPropertyName("MustFirst20Sec")] public string MustFirst20Sec { get; set; } = $" You can use this command only in {ChatColors.Red}first 20 Seconds{ChatColors.Default}.";
    [JsonPropertyName("MustBeAlive")] public string MustBeAlive { get; set; } = $" You can use this command only when {ChatColors.Red}you are alive{ChatColors.Default}!";

}
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
    public override string ModuleDescription => "Simple VIP system based on database.";
    public override string ModuleVersion => "V. 1.1.4";
    private string DatabaseConnectionString = string.Empty;
    private static readonly int?[] IsVIP = new int?[65];
    private static readonly int?[] Used = new int?[65];
    private static readonly int?[] LastUsed = new int?[65];
    private static readonly int?[] RespawnUsed = new int?[64];
    private static readonly int?[] HaveDoubble = new int?[64];

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


            Server.PrintToConsole($"MySQL {Config.DBHost} Connected");

            MySql.ExecuteNonQueryAsync(@"CREATE TABLE IF NOT EXISTS `users` (`id` INT AUTO_INCREMENT PRIMARY KEY, `steam_id` VARCHAR(32) UNIQUE NOT NULL, `end` INT(11) NOT NULL, UNIQUE (`steam_id`));");
            MySql.ExecuteNonQueryAsync(@"CREATE TABLE IF NOT EXISTS `users_test_vip` (`id` INT AUTO_INCREMENT PRIMARY KEY, `steam_id` VARCHAR(32) UNIQUE NOT NULL, `used` INT(11) NOT NULL, UNIQUE (`steam_id`));");
            MySql.ExecuteNonQueryAsync(@"CREATE TABLE IF NOT EXISTS `users_key_vip` (`id` INT AUTO_INCREMENT PRIMARY KEY, `token` VARCHAR(32) UNIQUE NOT NULL, `end` INT(11) NOT NULL, UNIQUE (`token`));");

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
                OnTick(client);
                if (IsVIP[client.EntityIndex!.Value.Value] == 0)
                    return;
                if (!Config.Bombinfo)
                    return;
                if (Bomb)
                {
                    if (bombtime >= 25)
                    {
                        client.PrintToCenterHtml(
                        $"<font color='gray'>Bomb detonating</font> <font class='fontSize-l' color='green'>{bombtime}</font><br>" +
                        $"<font color='white'>Tik</font> <font color='orange'>tak</font>");
                    }
                    else if (bombtime >= 10)
                    {
                        client.PrintToCenterHtml(
                        $"<font color='green'>Bomb detonating</font> <font class='fontSize-l' color='orange'>{bombtime}</font><br>" +
                        $"<font color='orange'>Timer is</font> <font color='white'>smaller</font>");
                    }
                    else if (bombtime >= 5)
                    {
                        client.PrintToCenterHtml(
                        $"<font color='gold'>Bomb detonating</font> <font class='fontSize-l' color='red'>{bombtime}</font><br>" +
                        $"<font color='white'>Last change</font> <font color='orange'>TO DEFUSE!</font>");
                    }
                    else if (bombtime >= 0)
                    {
                        client.PrintToCenterHtml(
                        $"<font color='gold'>Bomb detonating</font> <font class='fontSize-l' color='red'>{bombtime}</font><br>" +
                        $"<font color='white'>All on site is</font> <font color='orange'>DEAD!</font>");
                    }
                }
            }
        });


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
    public static void OnTick(CCSPlayerController controller)
    {
        if (!controller.PawnIsAlive)
            return;
        var pawn = controller.Pawn.Value;
        var flags = (PlayerFlags)pawn.Flags;
        var client = controller.EntityIndex.Value.Value;
        var buttons = controller.Buttons;

        if (IsVIP[client] == 0)
            return;

        if (HaveDoubble[client] == 0)
            return;


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

        MySqlQueryResult result = MySql!.Table("users").Where(MySqlQueryCondition.New("steam_id", "=", player.SteamID.ToString())).Select();
        if (result.Rows == 1)
        {
            var client = player.EntityIndex!.Value.Value;
            IsVIP[client] = 1;
            player.PrintToCenter("Congratulation! You have VIP");
            var timeRemaining = DateTimeOffset.FromUnixTimeSeconds(result.Get<int>(0, "end")) - DateTimeOffset.UtcNow;
            var nowtimeis = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var timeRemainingFormatted =
            $"{timeRemaining.Days}d {timeRemaining.Hours}:{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}";
            Server.PrintToConsole($"VIP Plugin - Player {player.PlayerName} ({player.SteamID}) have VIP. Remaining time of VIP {timeRemainingFormatted}");
            // Checking if is still time to VIP
            if (result.Get<int>(0, "end") != 0)
            {
                if (result.Get<int>(0, "end") < nowtimeis)
                {
                    Server.PrintToConsole($"VIP Plugin - Player {player.PlayerName} ({player.SteamID}) exp. VIP today..");
                    MySql.Table("users").Where(MySqlQueryCondition.New("steam_id", "=", player.SteamID.ToString())).Delete();
                    IsVIP[client] = 0;
                }
            }
            else
            {
                Server.PrintToConsole($"VIP Plugin - Player {player.PlayerName} ({player.SteamID}) have VIP forever");
            }
        }
        else
        {
            Server.PrintToConsole($"VIP Plugin - Player {player.PlayerName} ({player.SteamID}) is not VIP");
        }
    }
    public static void RemoveWeapons(CCSPlayerController? player)
    {
        foreach (var weapon in player.PlayerPawn.Value.WeaponServices!.MyWeapons)
        {
            if (weapon is { IsValid: true, Value.IsValid: true })
            {
                if(!weapon.Value.DesignerName.Contains("bayonet") || !weapon.Value.DesignerName.Contains("knife"))
                { continue; } 
                    weapon.Value.Remove();
                    Server.PrintToConsole($"{player.PlayerName} remove weapon {weapon.Value.DesignerName}");
            }
        }
    }
    private bool CheckIsHaveWeapon(string weapon_name, CCSPlayerController? pc)
    {
        foreach (var weapon in pc.PlayerPawn.Value.WeaponServices!.MyWeapons)
        {
            if (weapon is { IsValid: true, Value.IsValid: true })
            {
                if (weapon.Value.DesignerName.Contains($"{weapon_name}"))
                {
                    Server.PrintToConsole($"VIP Plugin - Requested weapon is weapon_{weapon_name}");
                    Server.PrintToConsole($"VIP Plugin - {pc.PlayerName} have weapon with name {weapon.Value.DesignerName}");
                    return true;
                }
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
            var entityIndex = smokeGrenadeEntity.Thrower.Value.Controller.Value.EntityIndex!.Value.Value;

            if (entityIndex == null) return;
            if (IsVIP[entityIndex] == 0) return;

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

        var client = controller.EntityIndex!.Value.Value;
        var PawnValue = controller.PlayerPawn.Value;
        var moneyServices = controller.InGameMoneyServices;
        if (IsVIP[client] == 1)
        {
            if (Config.EnableVIPAcceries)
            {
                PawnValue.Health += Config.RewardsClass.SpawnHP;
                PawnValue.ArmorValue = Config.RewardsClass.SpawnArmor;

                if (moneyServices.Account <= 800)
                {
                    moneyServices.Account = Config.RewardsClass.FirstSpawnMoney;
                }
                if (LastUsed[client] != 2 || LastUsed[client] != 3)
                {
                    controller.GiveNamedItem("weapon_healthshot");
                }
            }
        }

    }
    // Database settings
}
