using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json.Serialization;

namespace CustomPlugin;


public class ConfigBan : BasePluginConfig
{

    [JsonPropertyName("Token")] public string Token { get; set; } = "YOUR_TOKEN";

    [JsonPropertyName("Prefix")] public string Prefix { get; set; } = $" {ChatColors.Default}[{ChatColors.Green}MadGames.eu{ChatColors.Default}]";

    [JsonPropertyName("DBDatabase")] public string DBDatabase { get; set; } = "database";
    [JsonPropertyName("DBUser")] public string DBUser { get; set; } = "user";
    [JsonPropertyName("DBPassword")] public string DBPassword { get; set; } = "password";
    [JsonPropertyName("DBHost")] public string DBHost { get; set; } = "localhost";
    [JsonPropertyName("DBPort")] public int DBPort { get; set; } = 3306;

    [JsonPropertyName("VIPTag_Chat")] public string VIPTag_Chat { get; set; } = "{Green} VIP{Grey}» ";
    [JsonPropertyName("VIPTag_Score")] public string VIPTag_Score { get; set; } = "★";
    [JsonPropertyName("MVIPTag_Chat")] public string MVIPTag_Chat { get; set; } = "{Green} MVIP{Grey}» ";
    [JsonPropertyName("MVIPTag_Score")] public string MVIPTag_Score { get; set; } = "⁂";

    [JsonPropertyName("NadeEnable")] public bool NadeEnable { get; set; } = true;
    [JsonPropertyName("NadeModel")] public string NadeModel { get; set; } = "particles/ui/hud/ui_map_def_utility_trail.vpcf";
    [JsonPropertyName("NadeColor")] public string NadeColor { get; set; } = "red";

    [JsonPropertyName("Enable_Credits")] public bool More_Credit { get; set; } = true;
    [JsonPropertyName("Credits_Kill")] public int Credits_For_Kill { get; set; } = 50;
    [JsonPropertyName("Enable_Credits_Plant")] public bool More_Credit2 { get; set; } = true;
    [JsonPropertyName("Credits_Plant")] public int Credits_For_Plant { get; set; } = 50;
    [JsonPropertyName("Enable_Credits_Defuse")] public bool More_Credit3 { get; set; } = true;
    [JsonPropertyName("Credits_Defuse")] public int Credits_For_Defuse { get; set; } = 50;

    [JsonPropertyName("StartHealth")] public int StartHealth { get; set; } = 110;

    [JsonPropertyName("MinRoundForGuns")] public int MinRoundForGuns { get; set; } = 2;

}