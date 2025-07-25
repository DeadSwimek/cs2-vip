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

    [JsonPropertyName("SettingsCommand")] public string SettingsCommand { get; set; } = "css_settings";
    [JsonPropertyName("AdminPermissions")] public string AdminPermissions { get; set; } = "@css/root";

    [JsonPropertyName("VIPTag_Chat")] public string VIPTag_Chat { get; set; } = "{Green} VIP{Grey}» ";
    [JsonPropertyName("VIPTag_Score")] public string VIPTag_Score { get; set; } = "★";
    [JsonPropertyName("MVIPTag_Chat")] public string MVIPTag_Chat { get; set; } = "{Green} MVIP{Grey}» ";
    [JsonPropertyName("MVIPTag_Score")] public string MVIPTag_Score { get; set; } = "⁂";

    [JsonPropertyName("NadeEnable")] public bool NadeEnable { get; set; } = true;
    [JsonPropertyName("NadeModel")] public string NadeModel { get; set; } = "particles/ui/hud/ui_map_def_utility_trail.vpcf";
    [JsonPropertyName("TrailParticle")] public string ParticleModel { get; set; } = "particles/ui/hud/ui_map_def_utility_trail.vpcf";
    [JsonPropertyName("NadeColor")] public string NadeColor { get; set; } = "red";

    [JsonPropertyName("Enable_Credits")] public bool More_Credit { get; set; } = true;
    [JsonPropertyName("Credits_Kill")] public int Credits_For_Kill { get; set; } = 50;
    [JsonPropertyName("Enable_Credits_Plant")] public bool More_Credit2 { get; set; } = true;
    [JsonPropertyName("Credits_Plant")] public int Credits_For_Plant { get; set; } = 50;
    [JsonPropertyName("Enable_Credits_Defuse")] public bool More_Credit3 { get; set; } = true;
    [JsonPropertyName("Credits_Defuse")] public int Credits_For_Defuse { get; set; } = 50;

    [JsonPropertyName("AllowTestVIP")] public bool AllowTestVIP { get; set; } = true;
    [JsonPropertyName("ReservedSlots")] public int ReservedSlots { get; set; } = 1;
    [JsonPropertyName("ReservedType")] public int ReservedType { get; set; } = 1;
    [JsonPropertyName("DaysTestVIP")] public int DaysTestVIP { get; set; } = 7;

    [JsonPropertyName("EnabledTags")] public bool EnabledTags { get; set; } = true;
    [JsonPropertyName("EnabledBhop")] public bool EnabledBhop { get; set; } = true;
    [JsonPropertyName("EnabledTrails")] public bool EnabledTrails { get; set; } = true;
    [JsonPropertyName("EnabledShotTrails")] public bool EnabledShotTrails { get; set; } = true;
    [JsonPropertyName("EnabledGuns")] public bool EnabledGuns { get; set; } = true;
    [JsonPropertyName("EnabledQuake")] public bool EnabledQuake { get; set; } = false;
    [JsonPropertyName("EnabledFallDamage")] public bool FallDamage { get; set; } = true;
    [JsonPropertyName("EnabledNoKnifeDamage")] public bool NoKnifeDamage { get; set; } = true;
    [JsonPropertyName("EnabledModels")] public bool ModelsEnabled { get; set; } = true;
    [JsonPropertyName("EnabledDoubbleJump")] public bool EnabledDoubbleJump { get; set; } = true;
    [JsonPropertyName("EnabledInstantReload")] public bool EnabledInstantReload { get; set; } = true;
    [JsonPropertyName("AllowPreCacheResources")] public bool Precache { get; set; } = true;
    [JsonPropertyName("QuakeTeamKiller")] public string QuakeTeamKiller { get; set; } = "sounds/madgamessounds/quake/teamkiller.vsnd_c";
    [JsonPropertyName("QuakeHeadShot")] public string QuakeHeadShot { get; set; } = "sounds/madgamessounds/quake/headshot.vsnd_c";

    public List<Sound> Sounds { get; set; } = new List<Sound>
    {
        new Sound { Kill = 1, Path = "sounds/madgamessounds/quake/firstblood.vsnd_c" },
        new Sound { Kill = 2, Path = "sounds/madgamessounds/quake/doublekill.vsnd_c" },
        new Sound { Kill = 3, Path = "sounds/madgamessounds/quake/triplekill.vsnd_c" },
    };
    public List<VIP> VIPs { get; set; } = new List<VIP>
    {
        new VIP { value = 1, permission = "healthshot" },
        new VIP { value = 1, permission = "store_credit" },
        new VIP { value = 0, permission = "shotlaser" },
        new VIP { value = 1, permission = "trials" },
        new VIP { value = 0, permission = "nade" },
        new VIP { value = 1, permission = "guns" },
        new VIP { value = 1, permission = "bhop" },
        new VIP { value = 1, permission = "bomb" },
        new VIP { value = 1, permission = "health" },
        new VIP { value = 1, permission = "falldmg" },
        new VIP { value = 1, permission = "knife" },
        new VIP { value = 1, permission = "jump" },
        new VIP { value = 0, permission = "mvip" },
        new VIP { value = 1, permission = "tag" },
        new VIP { value = 0, permission = "reserved"},
        new VIP { value = 0, permission = "reloading" },
        new VIP { value = 0, permission = "antiflash" },
        new VIP { value = 0, permission = "wings"},
    };

    [JsonPropertyName("StartHealth")] public int StartHealth { get; set; } = 110;
    [JsonPropertyName("MinRoundForGuns")] public int MinRoundForGuns { get; set; } = 2;

}
public class Sound
{
    [JsonPropertyName("quake_kill")]
    public required int Kill { get; init; }

    [JsonPropertyName("path")]
    public required string Path { get; init; }
}
public class VIP
{
    [JsonPropertyName("permission")]
    public required string permission { get; init; }

    [JsonPropertyName("value")]
    public required int value { get; init; }
}