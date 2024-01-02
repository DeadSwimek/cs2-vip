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
using System.Security.Cryptography;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;


namespace VIP
{
    
    public partial class VIP
    {
        private readonly nint Handle;
        public CHandle<CBaseEntity> EndEntity => Schema.GetDeclaredClass<CHandle<CBaseEntity>>(this.Handle, "CBeam", "m_hEndEntity");
        private HookResult OnPlayerChat(CCSPlayerController? player, CommandInfo info)
        {
            if (!Config.EnableVIPPrefix)
            {
                return HookResult.Continue;
            }
            var client = player.Index;
                var message = info.GetArg(1);
                string message_first = info.GetArg(1);

                if (player == null || !player.IsValid || player.IsBot || message == null || message == "")
                    return HookResult.Continue;
                if (message_first.Substring(0, 1) == "/" || message_first.Substring(0, 1) == "!" || message_first.Substring(0, 1) == "rtv")
                    return HookResult.Continue;
                var GetTag = "";
                if (IsVIP[client] == 1)
                {
                    GetTag = $" {ChatColors.Lime}VIP {ChatColors.Default}»";
                    var isAlive = player.PawnIsAlive ? "" : "-DEAD-";

                    Server.PrintToChatAll(ReplaceTags($"{isAlive} {GetTag} {ChatColors.Red}{player.PlayerName} {ChatColors.Default}: {ChatColors.Lime}{message}"));
                }
                else
                {
                return HookResult.Continue;
                }
                return HookResult.Handled;
        }
        private HookResult OnPlayerChatTeam(CCSPlayerController? player, CommandInfo info)
        {
            if (!Config.EnableVIPPrefix)
            {
                return HookResult.Continue;
            }
            var client = player.Index;
                var message = info.GetArg(1);
                string message_first = info.GetArg(1);

                if (player == null || !player.IsValid || player.IsBot || message == null || message == "")
                    return HookResult.Continue;
                if (message_first.Substring(0, 1) == "/" || message_first.Substring(0, 1) == "!" || message_first.Substring(0, 1) == "rtv")
                    return HookResult.Continue;
                var GetTag = "";
                if (IsVIP[client] == 1)
                {
                    GetTag = $" {ChatColors.Lime}VIP {ChatColors.Default}»";
                    var isAlive = player.PawnIsAlive ? "" : "-DEAD-";
                    for (int i = 1; i <= Server.MaxPlayers; i++)
                    {
                        CCSPlayerController? pc = Utilities.GetPlayerFromIndex(i);
                        if (pc == null || !pc.IsValid || pc.IsBot || pc.TeamNum != player.TeamNum) continue;
                        pc.PrintToChat(ReplaceTags($"{isAlive}(TEAM) {GetTag} {ChatColors.Red}{player.PlayerName} {ChatColors.Default}: {ChatColors.Lime}{message}"));
                    }
                }
                else
                {
                    return HookResult.Continue;
                }
            return HookResult.Handled;
        }
        [GameEventHandler]
        public HookResult OnClientConnect(EventPlayerConnectFull @event, GameEventInfo info)
        {
            CCSPlayerController player = @event.Userid;
            if (player == null || !player.IsValid || player.IsBot)
                return HookResult.Continue;
            if (ConnectedPlayers == 0)
            {
                ConnectedPlayers = 1;
            }
            else
            {
                ConnectedPlayers++;
            }

            var client = player.Index;
            Used[client] = 0;
            LastUsed[client] = 0;
            IsVIP[client] = 0;
            HaveGroup[client] = 0;
            LoadPlayerData(player);
            if (Config.WelcomeMessageEnable)
            {
                player.PrintToChat($" {Localizer["welcome"]}");
            }
            return HookResult.Continue;
        }
        [GameEventHandler]
        public HookResult OnClientDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
        {
            CCSPlayerController player = @event.Userid;

            if (player == null || !player.IsValid || player.IsBot)
                return HookResult.Continue;
            int connected = 0;
            foreach (var player_l in Utilities.GetPlayers().Where(player => player is { IsBot: false, IsValid: true }))
            {
                connected++;
            }
            ConnectedPlayers = connected;
            return HookResult.Continue;
        }
        [GameEventHandler]
        public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            Server.PrintToConsole("This plugins is created by DeadSwim / https://madgames.eu");
            timer_ex?.Kill();
            foreach (var l_player in Utilities.GetPlayers())
            {
                CCSPlayerController player = l_player;
                var client = player.Index;
                if (IsVIP[client] == 1)
                {
                    RespawnUsed[client] = 0;
                }
            }

            if(Round == 0 || Round == -1)
            {
                foreach (var l_player in Utilities.GetPlayers())
                {
                    int?[] HaveC4 = new int?[65];

                    CCSPlayerController player = l_player;
                    if(player == null && !player.IsValid)
                    {
                        return HookResult.Continue;
                    }
                    var client = player.Index;
                    if (IsVIP[client] == 1)
                    {
                        if (CheckIsHaveWeapon("c4", player) == true)
                        {
                            HaveC4[client] = 1;
                        }
                        foreach (var weapon in player.PlayerPawn.Value.WeaponServices!.MyWeapons)
                        {
                            if (weapon is { IsValid: true, Value.IsValid: true })
                            {
                                if (weapon.Value.DesignerName.Contains("bayonet") || weapon.Value.DesignerName.Contains("knife"))
                                {
                                    continue;
                                }
                                weapon.Value.Remove();
                            }
                        }
                        if (player.TeamNum == ((byte)CsTeam.Terrorist))
                        {
                            player.GiveNamedItem("weapon_glock");
                            if (HaveC4[client] == 1)
                            {
                                player.GiveNamedItem("weapon_c4");
                                HaveC4[client] = 0;
                            }
                        }
                        if (player.TeamNum == ((byte)CsTeam.CounterTerrorist))
                        {
                            player.GiveNamedItem("weapon_hkp2000");
                        }
                    }
                }
            }

            Round++;

            Bombplanted = false;
            Bomb = false;
            timer_ex?.Kill();
            timer_twenty?.Kill();
            Disabled20Sec = false;
            if (Round < 2)
            {
                DisableGiving = false;
            }
            WriteColor($"VIP Plugin - Added new round count, now is [{Round}] {Bomb}.", ConsoleColor.Magenta);


            return HookResult.Continue;
        }
        [GameEventHandler]
        public HookResult OnClientSpawn(EventPlayerSpawn @event, GameEventInfo info)
        {
            CCSPlayerController player = @event.Userid;
            if (player == null || !player.IsValid || player.TeamNum == 1)
            {
                return HookResult.Continue;
            }
            else
            {
                timer_ex?.Kill();
                var client = player.Index;
                Used[client] = 0;
                Server.NextFrame(() =>
                {
                    AddTimer(0.1f, () => { Give_Values(player); });
                });

                if (Config.DisablePackWeaponAfter20Sec)
                {
                    timer_twenty?.Kill();
                    Disabled20Sec = false;
                }
                if (Config.DisablePackWeaponAfter20Sec)
                {
                    timer_twenty = AddTimer(20.0f, () =>
                            Disabled20Sec = true
                    );
                }

                if (Config.EnableDoubbleJump)
                {
                    if (Config.CommandOnGroup.DoubbleJump > HaveGroup[client])
                    {
                        return HookResult.Continue;
                    }
                    HaveDoubble[client] = 1;
                }
                else
                {
                    HaveDoubble[client] = 0;
                }

                if (Round < Config.MinimumRoundToUseCommands)
                {
                    //player.PrintToChat($" {Config.Prefix} {Config.TranslationClass.MustBeThird}");
                    return HookResult.Stop;
                }

                if (LastUsed[client] >= 1)
                {
                    if (Config.Messages.AllowCenterMessages)
                    {
                        AddTimer(2.0f, () =>
                        player.PrintToCenterHtml($" {Localizer["AutoGun"]}")
                        );
                    }
                    if (!Config.Messages.AllowCenterMessages)
                    {
                        player.PrintToChat($" {Config.Prefix} {Localizer["AutoGun"]}");
                    }
                }
                if (LastUsed[client] == 1)
                {
                    if (CheckIsHaveWeapon("ak47", player) == false)
                    {
                        player.GiveNamedItem("weapon_ak47");
                    }
                    player.PrintToChat($" {Config.Prefix} {Localizer["WeaponAK"]}");
                    Used[client] = 1;
                }
                else if (LastUsed[client] == 2)
                {
                    foreach (var weapon in Config.Pack1Settings.Weapons)
                    {
                        if (CheckIsHaveWeapon($"{weapon}", player) == false)
                        {
                            player.GiveNamedItem($"weapon_{weapon}");
                        }
                    }
                    player.PrintToChat($" {Config.Prefix} {Localizer["Packages_one"]}");
                    Used[client] = 1;
                }
                else if (LastUsed[client] == 3)
                {
                    foreach (var weapon in Config.Pack2Settings.Weapons)
                    {
                        if (CheckIsHaveWeapon($"{weapon}", player) == false)
                        {
                            player.GiveNamedItem($"weapon_{weapon}");
                        }
                    }
                    player.PrintToChat($" {Config.Prefix} {Localizer["Package_two"]}");
                    Used[client] = 1;
                }
                else if (LastUsed[client] == 10)
                {
                    foreach (var weapon in Config.Pack3Settings.Weapons)
                    {
                        if (CheckIsHaveWeapon($"{weapon}", player) == false)
                        {
                            player.GiveNamedItem($"weapon_{weapon}");
                        }
                    }
                    player.PrintToChat($" {Config.Prefix} {Localizer["Package_three"]}");
                    Used[client] = 1;
                }
                else if (LastUsed[client] == 4)
                {
                    if (CheckIsHaveWeapon("m4a1", player) == false)
                    {
                        player.GiveNamedItem("weapon_m4a1");
                    }
                    player.PrintToChat($" {Config.Prefix} {Localizer["WeaponM4A1"]}");
                    Used[client] = 1;
                }
                else if (LastUsed[client] == 5)
                {
                    if (CheckIsHaveWeapon("m4a1_silencer", player) == false)
                    {
                        player.GiveNamedItem("weapon_m4a1_silencer");
                    }
                    player.PrintToChat($" {Config.Prefix} {Localizer["WeaponM4A1S"]}");
                    Used[client] = 1;
                }
                else if (LastUsed[client] == 6)
                {
                    if (CheckIsHaveWeapon("awp", player) == false)
                    {
                        player.GiveNamedItem("weapon_awp");
                    }
                    player.PrintToChat($" {Config.Prefix} {Localizer["WeaponAWP"]}");
                    Used[client] = 1;
                }
                //player.PrintToChat($"{Config.Prefix} You can use /ak for give AK47 or /m4 for give M4A1");
            }
            return HookResult.Continue;
        }
        [GameEventHandler]
        public HookResult OnBombDetonate(EventBombDefused @event, GameEventInfo info)
        {
            Bomb = false;
            timer_ex?.Kill();
            return HookResult.Continue;
        }
        [GameEventHandler]
        public HookResult OnBombDefused(EventBombDefused @event, GameEventInfo info)
        {
            Bomb = false;
            timer_ex?.Kill();
            var player = @event.Userid;

            if (Config.DefusedReward)
            {
                var moneyServices = player.InGameMoneyServices;
                moneyServices.Account = moneyServices.Account + Config.RewardsClass.DetonateMoney;
            }
            return HookResult.Continue;
        }
        [GameEventHandler]
        public HookResult OnBombPlanted(EventBombPlanted @event, GameEventInfo info)
        {
            var c4list = Utilities.FindAllEntitiesByDesignerName<CC4>("weapon_c4");
            var c4 = c4list.FirstOrDefault();
            var planted = new CBombTarget(NativeAPI.GetEntityFromIndex(@event.Site));


            var player = @event.Userid;
            Bomb = true;
            bombtime = 40.0f;
            if (Config.Bombinfo)
            {
                if (Config.CommandOnGroup.BombInfo > get_vip_group(player))
                {
                    return HookResult.Continue;
                }
                allow_bombinfo[player.Index] = 1;
                if (planted.IsBombSiteB)
                {
                    SitePlant = "B";
                }
                else
                {
                    SitePlant = "A";
                }
                timer_ex = AddTimer(1.0f, () =>
                {
                    if (bombtime == 0)
                    {
                        timer_ex?.Kill();
                        return;
                    }

                    bombtime = bombtime - 1.0f;
                }, TimerFlags.REPEAT);
            }
            AddTimer(35.0f, () =>
            {
                Bombplanted = true;
                WriteColor("VIP Plugin - *[WARNING]* Now players canno't get rewards form kills..", ConsoleColor.Yellow);
                if (Config.DetonateRewards)
                {
                    var moneyServices = player.InGameMoneyServices;
                    moneyServices.Account = moneyServices.Account + Config.RewardsClass.DetonateMoney;
                }
            });

            return HookResult.Continue;
        }
        public void Explosive()
        {
            if (bombtime == 0)
            {
                timer_ex?.Kill();
                return;
            }
            bombtime = bombtime - 1.0f;
        }
        [GameEventHandler]
        public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            if(Bombplanted == true)
            {
                return HookResult.Continue;
            }
            CCSPlayerController player = @event.Userid;
            CCSPlayerController attacker = @event.Attacker;
            var MoneyValueAttacker = attacker.InGameMoneyServices;
            var attacker_entity = attacker.Index;
            if (player == null || !player.IsValid)
                return HookResult.Continue;
            if (attacker == null || !attacker.IsValid)
                return HookResult.Continue;
            if (IsVIP[attacker_entity] == 0)
                return HookResult.Continue;
            if (player != attacker)
            {
                
                if (Config.GiveHPAfterKill || Config.GiveMoneyAfterKill)
                    {
                        if (Config.AllowKillMessages)
                        {
                        Server.PrintToChatAll($" {Config.Prefix} {Localizer["KillInfo", player.PlayerName, attacker.PlayerName]}");
                        }
                    }
                    if (Config.GiveMoneyAfterKill)
                    {
                        var AttackerMoneys = MoneyValueAttacker.Account;
                        MoneyValueAttacker.Account = AttackerMoneys + Config.RewardsClass.KillMoney;
                        //attacker.PrintToChat($" {Config.Prefix} You got {ChatColors.Lime}+{Config.RewardsClass.KillMoney} ${ChatColors.Default} for kill player {ChatColors.LightRed}{player.PlayerName}{ChatColors.Default}, enjoy.");
                        attacker.PrintToChat($" {Config.Prefix} {Localizer["MoneyRewards", Config.RewardsClass.KillMoney, player.PlayerName]}");

                    }
                if (Config.GiveHPAfterKill)
                    {
                        var health_attacker = attacker.PlayerPawn.Value.Health;
                        Server.NextFrame(() =>
                        {
                            AddTimer(0.1f, () => { set_hp(attacker, health_attacker + Config.RewardsClass.KillHP); });
                        });
                        Server.PrintToConsole($"VIP Plugins - Here is bug from valve https://discord.com/channels/1160907911501991946/1160907912445710482/1175583981387927602");
                        attacker.PrintToChat($" {Config.Prefix} {Localizer["KillRewards", Config.RewardsClass.KillHP, player.PlayerName]}");
                    }

            }
            return HookResult.Continue;
        }
        [GameEventHandler(HookMode.Pre)]
        public HookResult OnPlayerBlind(EventPlayerBlind @event, GameEventInfo info)
        {

            var player = @event.Userid;
            if (!player.IsValid || !player.PawnIsAlive || player == null)
                return HookResult.Continue;
            var attacker = @event.Attacker;
            if (!attacker.IsValid || !attacker.PawnIsAlive || attacker == null)
                return HookResult.Continue;
            var client = player.Index;
            var player_team = player.TeamNum;
            var attacker_team = attacker.TeamNum;
            if (Config.CommandOnGroup.Flash >= get_vip_group(player))
                return HookResult.Continue;
            if(player_team == attacker_team)
            {
                @event.BlindDuration = 0.1f;
                player.PlayerPawn.Value.FlashMaxAlpha = 0.5f;
                return HookResult.Changed;
            }
            return HookResult.Continue;

        }
        [GameEventHandler(HookMode.Post)]

        public HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
        {
            CCSPlayerController player = @event.Userid;
            CCSPlayerController attacker = @event.Attacker;

            var client = player.Index;


            if (player.Connected != PlayerConnectedState.PlayerConnected || !player.PlayerPawn.IsValid || !@event.Userid.IsValid)
                return HookResult.Continue;
            if (IsVIP[client] == 0)
            {
                return HookResult.Continue;
            }
            if (Config.EnableShowDamage)
            {
                if (@event.Hitgroup != 0)
                {
                    ShowDamage(attacker, @event.DmgHealth, @event.DmgArmor, player.PlayerName);
                }
            }
            if (!Config.EnableFalldamage)
            {
                if (@event.Hitgroup == 0 && @event.Weapon != "inferno" && @event.Weapon != "hegrenade" && @event.Weapon != "knife" && @event.Weapon != "decoy")
                {
                    if (@event.DmgHealth >= 100)
                    {
                        WriteColor($"VIP Plugin - *[WARNING]* Player {player.PlayerName} died on map drop or bomb detonate (Weapon: {@event.Weapon} HiTG: {@event.Hitgroup}).", ConsoleColor.Yellow);

                    }
                    else
                    {
                        player.PlayerPawn.Value.Health = player.PlayerPawn.Value.Health += @event.DmgHealth;
                        WriteColor($"VIP Plugin - *[WARNING]* Player {player.PlayerName} registred a FallDamage (Weapon: {@event.Weapon} HiTG: {@event.Hitgroup})!", ConsoleColor.Yellow);
                    }
                }
            }
            if (Config.KnifeDMGEnable)
            {
                if (@event.Weapon == "knife")
                {
                    if (@event.Userid.PlayerPawn.Value.Health + @event.DmgHealth <= 100)
                    {
                        @event.Userid.PlayerPawn.Value.Health = @event.Userid.PlayerPawn.Value.Health += @event.DmgHealth;
                        if (@event.Attacker.IsValid)
                        {
                            attacker.PrintToChat($" {Config.Prefix} {Localizer["PlayerISVIP"]}");
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
}
