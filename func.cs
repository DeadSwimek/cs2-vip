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