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


namespace VIP;


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
    [JsonPropertyName("MinimumRoundToUseCommands")] public int MinimumRoundToUseCommands { get; set; } = 3;
    [JsonPropertyName("DefusedReward")] public bool DefusedReward { get; set; } = true;


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
    [JsonPropertyName("pack3")] public Pack3Settings Pack3Settings { get; set; } = new Pack3Settings();

    [JsonPropertyName("GroupsNames")] public GroupsNames GroupsNames { get; set; } = new GroupsNames();

    [JsonPropertyName("CommandOnGroup")] public CommandOnGroup CommandOnGroup { get; set; } = new CommandOnGroup();
    [JsonPropertyName("Messages")] public Messages Messages { get; set; } = new Messages();

}
public class Messages
{
    [JsonPropertyName("AllowCenterMessages")] public bool AllowCenterMessages { get; set; } = true;

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
public class GroupsNames
{
    [JsonPropertyName("Group1")] public string Group1 { get; set; } = "VIP";
    [JsonPropertyName("Group2")] public string Group2 { get; set; } = "VIP II";
    [JsonPropertyName("Group3")] public string Group3 { get; set; } = "VIP III";

}
public class CommandOnGroup
{
    [JsonPropertyName("Respawn")] public int Respawn { get; set; } = 0;
    [JsonPropertyName("Pack")] public int Pack { get; set; } = 0;
    [JsonPropertyName("Weapons")] public int Weapons { get; set; } = 0;
    [JsonPropertyName("Acceries")] public int Acceries { get; set; } = 0;
    [JsonPropertyName("Smoke")] public int Smoke { get; set; } = 0;
    [JsonPropertyName("DoubbleJump")] public int DoubbleJump { get; set; } = 0;
    [JsonPropertyName("BombInfo")] public int BombInfo { get; set; } = 0;
    [JsonPropertyName("Glow")] public int Glow { get; set; } = 0;
    [JsonPropertyName("ReservedSlots")] public int ReservedSlots { get; set; } = 0;

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

public class Pack3Settings
{
    [JsonPropertyName("Allowed")] public bool Allowed { get; set; } = false;
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
    [JsonPropertyName("Pack3")] public string Pack3 { get; set; } = $" You got a Packages {ChatColors.Lime}number three{ChatColors.Default}.";


    [JsonPropertyName("WeaponAK")] public string WeaponAK { get; set; } = $" You got a weapon {ChatColors.Lime}AK-47{ChatColors.Default}.";
    [JsonPropertyName("WeaponM4A1")] public string WeaponM4A1 { get; set; } = $" You got a weapon {ChatColors.Lime}M4A1{ChatColors.Default}.";
    [JsonPropertyName("WeaponM4A1S")] public string WeaponM4A1S { get; set; } = $" You got a weapon {ChatColors.Lime}M4A1-S{ChatColors.Default}.";
    [JsonPropertyName("WeaponAWP")] public string WeaponAWP { get; set; } = $" You got a weapon {ChatColors.Lime}AWP{ChatColors.Default}.";
    [JsonPropertyName("Autoguns")] public string Autoguns { get; set; } = $" <font color:'green'>If you wanna turn off automaticall weapon type</font><font color:'red'> /guns_off</font>";
    [JsonPropertyName("MustFirst20Sec")] public string MustFirst20Sec { get; set; } = $" You can use this command only in {ChatColors.Red}first 20 Seconds{ChatColors.Default}.";
    [JsonPropertyName("MustBeAlive")] public string MustBeAlive { get; set; } = $" You can use this command only when {ChatColors.Red}you are alive{ChatColors.Default}!";

}
