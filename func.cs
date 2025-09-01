using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using CS2MenuManager.API.Enum;
using CS2MenuManager.API.Menu;
using Nexd.MySQL;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text;


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
        private string ReplaceColorTags(string input)
        {
            string[] colorPatterns =
            {
                "{DEFAULT}", "{RED}", "{LIGHTPURPLE}", "{GREEN}", "{LIME}", "{LIGHTGREEN}", "{LIGHTRED}", "{GRAY}",
                "{LIGHTOLIVE}", "{OLIVE}", "{LIGHTBLUE}", "{BLUE}", "{PURPLE}", "{GRAYBLUE}"
            };
            string[] colorReplacements =
            {
                "\x01", "\x02", "\x03", "\x04", "\x05", "\x06", "\x07", "\x08", "\x09", "\x10", "\x0B", "\x0C", "\x0E",
                "\x0A"
            };

            for (var i = 0; i < colorPatterns.Length; i++)
                input = input.Replace(colorPatterns[i], colorReplacements[i]);

            return input;
        }
        public void LoadOnTick()
        {
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                var ent = NativeAPI.GetEntityFromIndex(i);
                if (ent == 0)
                    continue;
                var client = new CCSPlayerController(ent);
                TryBhop(client);
                OnTick(client);
                if (Bomb_a[client.Index] == 1)
                {
                    if (Bomb)
                    {
                        if (bombtime >= 25)
                        {
                            client.PrintToCenterHtml(
                            $"<font color='gray'>{Localizer["Detonation"]}: </font> <font class='fontSize-l' color='green'>{bombtime} s</font><br>" +
                            $"<font color='gray'>{Localizer["PlacedOn"]}</font> <font class='fontSize-m' color='green'>[{SitePlant}]</font>");
                        }
                        else if (bombtime >= 10)
                        {
                            client.PrintToCenterHtml(
                            $"<font color='gray'>{Localizer["Detonation"]}: </font> <font class='fontSize-l' color='green'>{bombtime} s</font><br>" +
                            $"<font color='gray'>{Localizer["PlacedOn"]}</font> <font class='fontSize-m' color='green'>[{SitePlant}]</font>");
                        }
                        else if (bombtime >= 5)
                        {
                            client.PrintToCenterHtml(
                            $"<font color='gray'>{Localizer["Detonation"]}: </font> <font class='fontSize-l' color='green'>{bombtime} s</font><br>" +
                            $"<font class='fontSize-l' color='red'> >>>> TIK TAK! {bombtime} s <<<< </font><br>" +
                            $"<font color='gray'>{Localizer["PlacedOn"]}</font> <font class='fontSize-m' color='green'>[{SitePlant}]</font>");
                        }
                        else if (bombtime >= 2)
                        {
                            client.PrintToCenterHtml(
                            $"<font class='fontSize-m' color='red'> >>>> !KABOOOM! <<<< </font><br>" +
                            $"<font class='fontSize-m' color='red'> >>>> !BOOOOOM! <<<< </font><br>" +
                            $"<font class='fontSize-m' color='red'> >>>> !KABOOOM! <<<< </font><br>");
                        }
                        else if (bombtime == 0)
                        {
                            Bomb = false;
                        }
                    }
                }
            }
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
        public bool TryedVIP(CCSPlayerController player)
        {
            if (player == null)
                return false;

            var client = player.Index;

            MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);

            MySqlQueryResult result = MySql!.Table($"deadswim_settings").Where(MySqlQueryCondition.New("steamid", "=", $"{player.SteamID.ToString()}")).Select();

            if (result.Rows == 1)
            {
                for (int i = 0; i < result.Rows; i++)
                {
                    var row = result[i];
                    if (row == null) return false;
                    string freevip = row["free_vip"]!.ToString();
                    if (freevip == "1") { return true; }
                    if (freevip == "0") { return false; }
                }
            }

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
        public void OpenVIPMenu(CCSPlayerController player)
        {
            if (player == null)
                return;

            open_Settings(player);
        }
        public void OpenModelsMenu(CCSPlayerController player)
        {
            if (player == null) return;
            open_Models(player);
        }
        public void OpenTagMenu(CCSPlayerController player)
        {
            if (player == null) return;
            open_Tags(player);
        }
        public void quake(CCSPlayerController player)
        {
            if (player == null) return;
            var client = player.Index;
            if (QuakeEnable[client] == 1) { QuakeEnable[client] = 0; }
            if (QuakeEnable[client] == 0) { QuakeEnable[client] = 1; }
        }
        public void CreateNadeTrail(Vector start, CBaseGrenade grenade)
        {
            // Only in VIP Prémium
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

            CS2MenuManager.API.Menu.ChatMenu menu = new($"-> {Localizer["Guns"]} <-", this);

            bool vip = false;
            bool mvip = false;
            if (Guns[client] == 1) { vip = true; }
            if (MVIP[client] == 1) { mvip = true; }

            foreach (var gun in Config.Guns)
            {
                if (gun.permission == "vip")
                {
                    if (vip)
                    {
                        menu.AddItem(gun.name, (p, option) =>
                        {
                            player.GiveNamedItem(gun.weapon);
                            Selected[client] = gun.id;
                            Selected_round[client] = 1;
                            player.PrintToChat($" {Config.Prefix} {Localizer["VIPYouChoose", gun.name]} ");
                            SaveSettings(player);
                        });
                    }
                }
                if (gun.permission == "mvip")
                {
                    if (mvip)
                    {
                        menu.AddItem(gun.name, (p, option) =>
                        {
                            player.GiveNamedItem(gun.weapon);
                            Selected[client] = gun.id;
                            Selected_round[client] = 1;
                            player.PrintToChat($" {Config.Prefix} {Localizer["VIPYouChoose", gun.name]} ");
                            SaveSettings(player);
                        });
                    }
                }
            }


            menu.Display(player, 0);
        }
    }

}