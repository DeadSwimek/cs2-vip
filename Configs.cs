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
    [JsonPropertyName("EnableShowDamage")] public bool EnableShowDamage { get; set; } = true;
    


    [JsonPropertyName("WelcomeMessage")] public string WelcomeMessage { get; set; } = $"Welcom on server you are BEST VIP!";
    [JsonPropertyName("DBDatabase")] public string DBDatabase { get; set; } = "database";
    [JsonPropertyName("DBUser")] public string DBUser { get; set; } = "user";
    [JsonPropertyName("DBPassword")] public string DBPassword { get; set; } = "password";
    [JsonPropertyName("DBHost")] public string DBHost { get; set; } = "localhost";
    [JsonPropertyName("DBPort")] public int DBPort { get; set; } = 3306;
    [JsonPropertyName("DBPrefix")] public string DBPrefix { get; set; } = "deadswim";

    [JsonPropertyName("money")] public RewardsClass RewardsClass { get; set; } = new RewardsClass();

    [JsonPropertyName("pack1")] public Pack1Settings Pack1Settings { get; set; } = new Pack1Settings();
    [JsonPropertyName("pack2")] public Pack2Settings Pack2Settings { get; set; } = new Pack2Settings();
    [JsonPropertyName("pack3")] public Pack3Settings Pack3Settings { get; set; } = new Pack3Settings();

    [JsonPropertyName("GroupsNames")] public GroupsNames GroupsNames { get; set; } = new GroupsNames();

    [JsonPropertyName("CommandOnGroup")] public CommandOnGroup CommandOnGroup { get; set; } = new CommandOnGroup();
    [JsonPropertyName("Messages")] public Messages Messages { get; set; } = new Messages();
    [JsonPropertyName("TestVIP")] public TestVIP TestVIP { get; set; } = new TestVIP();
}
public class Messages
{
    [JsonPropertyName("AllowCenterMessages")] public bool AllowCenterMessages { get; set; } = true;

}
public class TestVIP
{
    [JsonPropertyName("EnableTestVIP")] public bool EnableTestVIP { get; set; } = true;
    [JsonPropertyName("TimeOfVIP")] public int TimeOfVIP { get; set; } = 3600;

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
    [JsonPropertyName("Flash")] public int Flash { get; set; } = 0;
}


public class Pack1Settings
{
    [JsonPropertyName("Weapons")] public List<string?> Weapons { get; set; } = new List<string?> { "ak47", "deagle", "healthshot", "molotov", "smokegrenade", "hegrenade" };
}
public class Pack2Settings
{
    [JsonPropertyName("Weapons")] public List<string?> Weapons { get; set; } = new List<string?> { "m4a1", "deagle", "healthshot", "molotov", "smokegrenade", "hegrenade" };
}

public class Pack3Settings
{
    [JsonPropertyName("Allowed")] public bool Allowed { get; set; } = false;
    [JsonPropertyName("Weapons")] public List<string?> Weapons { get; set; } = new List<string?> { "m4a1", "deagle", "healthshot", "molotov", "smokegrenade", "hegrenade" };

}
