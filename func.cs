using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CS2MenuManager.API.Enum;
using CS2MenuManager.API.Menu;
using CounterStrikeSharp.API.Modules.Admin;


namespace CustomPlugin
{
    public partial class CustomPlugin
    {
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
        public bool IsPlayerVip(CCSPlayerController player)
        {
            if (player == null)
                return false;

            var client = player.Index;

            if (MVIP[client] == 1) return true;
            else if (Guns[client] == 1) return true;

            return false;
        }
        public string GetGroup(CCSPlayerController player)
        {
            if (player == null)
                return "Player=Null";

            var client = player.Index;

            if (MVIP[client] == 1) return "MVIP";
            else if (Guns[client] == 1) return "VIP";

            return "None";
        }
        public void CreateNadeTrail(Vector start, CBaseGrenade grenade)
        {
            // Only in VIP Prémium
        }
        [ConsoleCommand("css_settings", "VIP settings")]
        public void SettingsMenu(CCSPlayerController player, CommandInfo info)
        {
            if (player == null) return;
            if (Guns[player.Index] == 0) { return; }

            var client = player.Index;

            var guns_status = "-";
            var Trails_status = "-";
            var shots_status = "-";
            var nade_status = "-";
            var bhop_status = "-";
            var jump_status = "-";

            if (Selected[client] >= 1) { guns_status = $"{Localizer["TurnOn"]} !guns_off"; } else { guns_status = $"{Localizer["TurnOff"]} !guns"; }
            if (UserTrialColor[client] == 0) { Trails_status = Localizer["TurnOff"]; } else { Trails_status = Localizer["TurnOn"]; }
            if (ShotsEnable[client] == 0) { shots_status = Localizer["TurnOff"]; } else { shots_status = Localizer["TurnOn"]; }
            if (BhopEnable[client] == 0) { bhop_status = Localizer["TurnOff"]; } else { bhop_status = Localizer["TurnOn"]; }
            if (NadeEnable[client] == 0) { nade_status = Localizer["TurnOff"]; } else { nade_status = Localizer["TurnOn"]; }
            if (JumpEnable[client] == 0) { jump_status = Localizer["TurnOff"]; } else { jump_status = Localizer["TurnOn"]; }

            PlayerMenu menu = new("Settings", this);
            if (Config.EnabledGuns)
            {
                menu.AddItem($"{Localizer["Guns"]} - {guns_status}", (p, option) => { }, DisableOption.DisableHideNumber);
            }
            //if (Config.EnabledTrails)
            //{
            //    menu.AddItem($"Trails Menu", (p, option) => { open_Trails(player); });
            //}

            //if (Config.EnabledShotTrails)
            //{
            //    menu.AddItem($"Shots - {shots_status}", (p, option) =>
            //    {
            //        if (ShotsEnable[client] == 0)
            //        {
            //            ShotsEnable[client] = 1;
            //        }
            //        else if (ShotsEnable[client] == 1)
            //        {
            //            ShotsEnable[client] = 0;
            //        }
            //        SaveSettings(player);
            //    });
            //} PREMIUM VERSION

            if (Config.EnabledBhop)
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

            if (Config.NadeEnable)
            {
                menu.AddItem($"Nade's Trails - {nade_status}", (p, option) =>
                {
                    if (NadeEnable[client] == 0)
                    {
                        NadeEnable[client] = 1;
                    }
                    else if (NadeEnable[client] == 1)
                    {
                        NadeEnable[client] = 0;
                    }
                    SaveSettings(player);
                });
            }
            if (Config.EnabledDoubbleJump)
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

            menu.Display(player, 0);
        }
        [ConsoleCommand("css_quake", "Enable/Disable")]
        public void switch_quake(CCSPlayerController? player, CommandInfo info)
        {
            var client = player!.Index;
            if (QuakeEnable[client] == 1)
            {
                QuakeEnable[client] = 0;
                player.PrintToChat($" {Config.Prefix} Quake {Localizer["JustNow"]} {ChatColors.Red}{Localizer["TurnOff"]}{ChatColors.Default}.");
            }
            else
            {
                QuakeEnable[client] = 1;
                player.PrintToChat($" {Config.Prefix} Quake {Localizer["JustNow"]} {ChatColors.Green}{Localizer["TurnOn"]}{ChatColors.Default}.");
            }
            SaveSettings(player);
        }
        public void guns(CCSPlayerController? player)
        {
            var client = player!.Index;

            if (player == null) return;

            if (Guns[player.Index] == 0) { return; }

            ScreenMenu menu = new($"-> {Localizer["Guns"]} <-", this);
            menu.AddItem("M4A1", (p, option) =>
            {
                player.GiveNamedItem("weapon_m4a1");
                Selected[client] = 1;
                Selected_round[client] = 1;
                player.PrintToChat($" {Config.Prefix} {Localizer["VIPYouChoose", "M4A1"]} ");
            });
            menu.AddItem("M4A1-S", (p, option) =>
            {
                player.GiveNamedItem("weapon_m4a1_silence");
                Selected[client] = 2;
                Selected_round[client] = 1;
                player.PrintToChat($" {Config.Prefix} {Localizer["VIPYouChoose", "M4A1-S"]}");
            });
            menu.AddItem("AK-47", (p, option) =>
            {
                player.GiveNamedItem("weapon_ak47");
                Selected[client] = 3;
                Selected_round[client] = 1;
                player.PrintToChat($" {Config.Prefix} {Localizer["VIPYouChoose", "AK-47"]}");
            });
            menu.AddItem("AWP [ M-VIP ]", (p, option) =>
            {
                if (AdminManager.PlayerHasPermissions(player, "@vip/mvip"))
                {
                    player.GiveNamedItem("weapon_awp");
                    Selected[client] = 4;
                    Selected_round[client] = 1;
                    player.PrintToChat($" {Config.Prefix} {Localizer["VIPYouChoose", "AWP"]}");
                }
                else
                {
                    player.PrintToChat($" {Config.Prefix} {Localizer["AllowedFor", "MVIP"]}");
                }
            });

            menu.Display(player, 0);
        }
    }

}