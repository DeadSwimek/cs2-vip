using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using VIPAPI;
using Microsoft.Extensions.Logging;
using CounterStrikeSharp.API.Core.Capabilities;

namespace VIP_Test;

public class VIPTestPlugin : BasePlugin
{
    public override string ModuleName => "TestVIP";
    public override string ModuleVersion => "1.0.0";

    public IAPI? _api;
    public static PluginCapability<IAPI> PluginCapability { get; } = new("vip:api");

    public override void Load(bool hotReload)
    {
        Logger.LogInformation("VIPTest plugin byl úspěšně načten.");
    }

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        _api = PluginCapability.Get();
        if (_api == null) return;
    }
    [ConsoleCommand("css_testvip", "Set trials color")]
    public void TestVipCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null || !player.IsValid) return;

        player.PrintToChat($"T:{_api?.GetVIPGroup(player)}");
        player.PrintToChat($"T:{_api?.IsVIP(player)}");
    }
}
