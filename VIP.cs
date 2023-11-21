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
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Menu;
using static CounterStrikeSharp.API.Core.Listeners;


namespace VIP;

public class ConfigVIP : BasePluginConfig
{
    [JsonPropertyName("Prefix")] public string Prefix { get; set; } = $" {ChatColors.Default}[{ChatColors.Green}MadGames.eu{ChatColors.Default}]";
    [JsonPropertyName("EnableVIPPrefix")] public bool EnableVIPPrefix { get; set; } = true;
    [JsonPropertyName("EnableVIPAcceries")] public bool EnableVIPAcceries { get; set; } = true;
    [JsonPropertyName("KnifeDMGEnable")] public bool KnifeDMGEnable { get; set; } = false;
    [JsonPropertyName("WelcomeMessageEnable")] public bool WelcomeMessageEnable { get; set; } = true;
    [JsonPropertyName("WelcomeMessage")] public string WelcomeMessage { get; set; } = $"Welcom on server you are BEST VIP!";

}
public class VIP : BasePlugin, IPluginConfig<ConfigVIP>
{
    public override string ModuleName => "VIP";
    public override string ModuleAuthor => "DeadSwim";
    public override string ModuleDescription => "Simple VIP system based on permissions.";
    public override string ModuleVersion => "V. 1.0.0";

    private static readonly int?[] Used = new int?[65];
    private static readonly int?[] LastUsed = new int?[65];
    public ConfigVIP Config { get; set; }

    public int Round;


    public void OnConfigParsed(ConfigVIP config)
    {
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        Console.WriteLine($"{Config.Prefix} VIP Plugins started, by deadswim");
        RegisterListener<Listeners.OnMapStart>(name =>
        {
            Round = 0;
        });
        // Load VIP Prefix
        AddCommandListener("say", OnPlayerChat);
        AddCommandListener("say_team", OnPlayerChatTeam);

    }
    private HookResult OnPlayerChat(CCSPlayerController? player, CommandInfo info)
    {
        if (Config.EnableVIPPrefix)
        {
            var message = info.GetArg(1);
            string message_first = info.GetArg(1);

            if (player == null || !player.IsValid || player.IsBot || message == null)
                return HookResult.Continue;
            if (message_first.Substring(0, 1) == "/" || message_first.Substring(0, 1) == "!" || message_first.Substring(0, 1) == "rtv")
                return HookResult.Continue;
            if (message_first.Substring(0, 1) != "/" || message_first.Substring(0, 1) != "!")
            {
                var GetTag = "";
                if (AdminManager.PlayerHasPermissions(player, "@css/vip"))
                {
                    GetTag = $" {ChatColors.Lime}VIP {ChatColors.Default}»";
                }

                var isAlive = player.PawnIsAlive ? "" : "-DEAD-";

                Server.PrintToChatAll(ReplaceTags($"{isAlive} {GetTag} {ChatColors.Red}{player.PlayerName} {ChatColors.Default}: {ChatColors.Lime}{message}"));
                return HookResult.Handled;
            }
            return HookResult.Continue;
        }
        else
        {
            return HookResult.Continue;
        }
    }
    private HookResult OnPlayerChatTeam(CCSPlayerController? player, CommandInfo info)
    {
        if (Config.EnableVIPPrefix)
        {
            var message = info.GetArg(1);
            string message_first = info.GetArg(1);

            if (player == null || !player.IsValid || player.IsBot || message == null)
                return HookResult.Continue;
            if (message_first.Substring(0, 1) == "/" || message_first.Substring(0, 1) == "!" || message_first.Substring(0, 1) == "rtv")
                return HookResult.Continue;
            if (message_first.Substring(0, 1) != "/" || message_first.Substring(0, 1) != "!")
            {
                var GetTag = "";
                if (AdminManager.PlayerHasPermissions(player, "@css/vip"))
                {
                    GetTag = $" {ChatColors.Lime}VIP {ChatColors.Default}»";
                }

                var isAlive = player.PawnIsAlive ? "" : "-DEAD-";
                for (int i = 1; i <= Server.MaxPlayers; i++)
                {
                    CCSPlayerController? pc = Utilities.GetPlayerFromIndex(i);
                    if (pc == null || !pc.IsValid || pc.IsBot || pc.TeamNum != player.TeamNum) continue;
                    pc.PrintToChat(ReplaceTags($"{isAlive}(TEAM) {GetTag} {ChatColors.Red}{player.PlayerName} {ChatColors.Default}: {ChatColors.Lime}{message}"));
                }
                return HookResult.Handled;
            }
            return HookResult.Continue;
        }
        else
        {
            return HookResult.Continue;
        }
    }
    public static void RemoveWeapons(CCSPlayerController? player)
    {
        foreach (var weapon in player.PlayerPawn.Value.WeaponServices!.MyWeapons)
        {
            if (weapon is { IsValid: true, Value.IsValid: true })
            {
                if (!weapon.Value.DesignerName.Contains("knife") || !weapon.Value.DesignerName.Contains("c4"))
                {
                    weapon.Value.Remove();
                    Server.PrintToConsole($"{player.PlayerName} remove weapon {weapon.Value.DesignerName}");
                }
            }
        }
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
    [RequiresPermissions("@css/vip")]
    [ConsoleCommand("ak", "Give a VIP Ak47")]
    public void CommandVIP_ak(CCSPlayerController? player, CommandInfo info)
    {
        var client = player.EntityIndex!.Value.Value;
        if (!player.IsValid || !player.PlayerPawn.IsValid)
        {
            return;
        }

        if (Round < 5)
        {
            player.PrintToChat($" {Config.Prefix} Must be third round to use this command.");
        }else{
            if (Used[client] == 1)
            {
                player.PrintToChat($" {Config.Prefix} You can't use this command 2x.");
            }
            else
            {

                player.GiveNamedItem("weapon_ak47");
                player.PrintToChat($" {Config.Prefix} You got AK-47.");
                Used[client] = 1;
                LastUsed[client] = 1;
            }
        }
    }
    [RequiresPermissions("@css/vip")]
    [ConsoleCommand("pack1", "Give you Pack 1")]
    public void CommandVIP_pack2(CCSPlayerController? player, CommandInfo info)
    {
        var client = player.EntityIndex!.Value.Value;
        if (!player.IsValid || !player.PlayerPawn.IsValid)
        {
            return;
        }
        if (Round < 5)
        {
            player.PrintToChat($" {Config.Prefix} Must be third round to use this command.");
        }else{
            if (Used[client] == 1)
            {
                player.PrintToChat($" {Config.Prefix} You can't use this command 2x.");
            }
            else
            {
                RemoveWeapons(player);

                player.GiveNamedItem("weapon_ak47");
                player.GiveNamedItem("weapon_deagle");
                player.GiveNamedItem("weapon_healthshot");

                player.GiveNamedItem("weapon_molotov");
                player.GiveNamedItem("weapon_smokegrenade");
                player.GiveNamedItem("weapon_hegrenade");
                player.PrintToChat($" {Config.Prefix} You got pack number 1.");
                Used[client] = 1;
                LastUsed[client] = 2;
            }
        }
    }
    [RequiresPermissions("@css/vip")]
    [ConsoleCommand("pack2", "Give you Pack 2")]
    public void CommandVIP_pack1(CCSPlayerController? player, CommandInfo info)
    {
        var client = player.EntityIndex!.Value.Value;
        if (!player.IsValid || !player.PlayerPawn.IsValid)
        {
            return;
        }
        if (Round < 5)
        {
            player.PrintToChat($" {Config.Prefix} Must be third round to use this command.");
        }else{
            if (Used[client] == 1)
            {
                player.PrintToChat($" {Config.Prefix} You can't use this command 2x.");
            }
            else
            {
                RemoveWeapons(player);

                player.GiveNamedItem("weapon_m4a1");
                player.GiveNamedItem("weapon_deagle");
                player.GiveNamedItem("weapon_healthshot");

                player.GiveNamedItem("weapon_molotov");
                player.GiveNamedItem("weapon_smokegrenade");
                player.GiveNamedItem("weapon_hegrenade");
                player.PrintToChat($"{Config.Prefix} You got pack number 2.");
                Used[client] = 1;
                LastUsed[client] = 3; 
            }
        }
    }
    [RequiresPermissions("@css/vip")]
    [ConsoleCommand("guns_off", "Turn off giving automaticlly weapons")]
    public void CommandGUNS_off(CCSPlayerController? player, CommandInfo info)
    {
        var client = player.EntityIndex!.Value.Value;
        if (!player.IsValid || !player.PlayerPawn.IsValid)
        {
            return;
        }
        if (LastUsed[client] >= 1)
        {
            LastUsed[client] = 0;
            player.PrintToCenter($"You turn off automatically weapon..");
        }
    }
    [RequiresPermissions("@css/vip")]
    [ConsoleCommand("m4", "Give a VIP M4A1")]
    public void CommandVIP_m4(CCSPlayerController? player, CommandInfo info)
    {
        var client = player.EntityIndex!.Value.Value;
        if (!player.IsValid || !player.PlayerPawn.IsValid)
        {
            return;
        }

        if (Round < 5)
        {
            player.PrintToChat($" {Config.Prefix} Must be third round to use this command.");
        }else{
            if (Used[client] == 1)
            {
                player.PrintToChat($" {Config.Prefix} You can't use this command 2x.");
            }
            else
            {
                player.GiveNamedItem("weapon_m4a1");
                player.PrintToChat($" {Config.Prefix} You got M4A1.");
                Used[client] = 1;
                LastUsed[client] = 4;


            }
        }
    }
    [GameEventHandler]
    public HookResult OnClientConnect(EventPlayerConnectFull @event, GameEventInfo info)
    {
        CCSPlayerController player = @event.Userid;

        if (player == null || !player.IsValid || player.IsBot)
            return HookResult.Continue;


        var client = player.EntityIndex!.Value.Value;
        Used[client] = 0;
        LastUsed[client] = 0;
        if (AdminManager.PlayerHasPermissions(player, "@css/vip"))
        {
            if (Config.WelcomeMessageEnable)
            {
                player.PrintToCenter($"{Config.WelcomeMessage}");
            }
        }

        return HookResult.Continue;
    }
    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        Round++;
        return HookResult.Continue;
    }
    private void Give_Values(CCSPlayerController controller)
    {
        var PawnValue = controller.PlayerPawn.Value;
        var moneyServices = controller.InGameMoneyServices;
        if (AdminManager.PlayerHasPermissions(controller, "@css/vip"))
        {
            if (Config.EnableVIPAcceries)
            {
                PawnValue.Health = 115;
                PawnValue.ArmorValue = 100;

                if (moneyServices.Account <= 800)
                {
                    moneyServices.Account = 1200;
                }
            }
        }

    }
    [GameEventHandler]
    public HookResult OnClientSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        CCSPlayerController player = @event.Userid;

        if (player == null || !player.IsValid || player.IsBot)
            return HookResult.Continue;

        if (player.UserId != null)
        {
            var client = player.EntityIndex!.Value.Value;
            Used[client] = 0;
            Give_Values(player);
            if (LastUsed[client] == 1) {
                player.GiveNamedItem("weapon_ak47");
                player.PrintToChat($" {Config.Prefix} You got automatically AK-47, if you wanna turn off type /guns_off.");
                Used[client] = 1;
            }
            if (LastUsed[client] == 2)
            {
                RemoveWeapons(player);

                player.GiveNamedItem("weapon_ak47");
                player.GiveNamedItem("weapon_deagle");
                player.GiveNamedItem("weapon_healthshot");

                player.GiveNamedItem("weapon_molotov");
                player.GiveNamedItem("weapon_smokegrenade");
                player.GiveNamedItem("weapon_hegrenade");
                player.PrintToChat($" {Config.Prefix} You got automatically Pack 2, if you wanna turn off type /guns_off.");
                Used[client] = 1;
            }
            if (LastUsed[client] == 3)
            {
                RemoveWeapons(player);

                player.GiveNamedItem("weapon_m4a1");
                player.GiveNamedItem("weapon_deagle");
                player.GiveNamedItem("weapon_healthshot");

                player.GiveNamedItem("weapon_molotov");
                player.GiveNamedItem("weapon_smokegrenade");
                player.GiveNamedItem("weapon_hegrenade");
                player.PrintToChat($" {Config.Prefix} You got automatically Pack 1, if you wanna turn off type /guns_off.");
                Used[client] = 1;
            }
            if (LastUsed[client] == 4)
            {
                player.GiveNamedItem("weapon_m4a1");
                player.PrintToChat($" {Config.Prefix} You got automatically M4A1, if you wanna turn off type /guns_off.");
                Used[client] = 1;
            }
            //player.PrintToChat($"{Config.Prefix} You can use /ak for give AK47 or /m4 for give M4A1");
        }

        return HookResult.Continue;
    }
    [GameEventHandler(HookMode.Post)]
    public HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        CCSPlayerController player = @event.Userid;
        CCSPlayerController attacker = @event.Attacker;

        if (player.Connected != PlayerConnectedState.PlayerConnected || !player.PlayerPawn.IsValid || !@event.Userid.IsValid)
            return HookResult.Continue;
        if (Config.KnifeDMGEnable)
        {
            if (@event.Weapon == "knife")
            {
                if (@event.Userid.PlayerPawn.Value.Health + @event.DmgHealth <= 100)
                {
                    @event.Userid.PlayerPawn.Value.Health = @event.Userid.PlayerPawn.Value.Health + @event.DmgHealth;
                    if(@event.Attacker.IsValid)
                    {
                        attacker.PrintToChat($" {Config.Prefix} You canno't hit {ChatColors.Lime}VIP {ChatColors.Default}player with Knife!");
                    }
                }
                else
                {
                    @event.Userid.PlayerPawn.Value.Health = 100;
                }
            }
        }
        @event.Userid.PlayerPawn.Value.VelocityModifier = 1;
        return HookResult.Continue;
    }
}
