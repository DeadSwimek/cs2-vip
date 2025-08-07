using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using CS2MenuManager.API.Menu;
using CS2MenuManager;
using Nexd.MySQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomPlugin
{
    public partial class CustomPlugin
    {
        public void open_Tags(CCSPlayerController player)
        {
            if (player == null) return;
            if (!Config.EnabledTags || !Tags) return;
            ChatMenu menu = new("Tags Menu\r\n", this);
            menu.AddItem("OFF - Chat", (p, option) =>
            {
                SelectedTag2[player.Index] = 0;
                player.PrintToChat($" {Config.Prefix} {Localizer["SelectedTag", "OFF", "Chat"]} ");
                SaveSettings(player);
            });
            menu.AddItem("OFF - ScoreBoard\r\n___________", (p, option) =>
            {
                SelectedTag[player.Index] = 0;
                player.PrintToChat($" {Config.Prefix} {Localizer["SelectedTag", "OFF", "ScoreBoard"]} ");
                SaveSettings(player);
            });
            menu.AddItem("Chat Tags", (p, option) =>
            {
                open_CTags(player);
            });
            menu.AddItem("ScoreBoard Tags", (p, option) =>
            {
                open_STags(player);
            });
            menu.Display(player, 0);
        }
        public void open_STags(CCSPlayerController player)
        {
            if (player == null) return;
            if (!Config.EnabledTags || !Tags) return;

            MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);

            MySqlQueryResult result = MySql!
                .Table("deadswim_tags")
                .Select();
            ChatMenu menu = new("ScoreBoard Tags", this);

            for (int i = 0; i < result.Rows; i++)
            {
                var row = result[i];
                if (row == null) return;


                string id_tag = Convert.ToString(row["id"]) ?? string.Empty;
                string name_tag = Convert.ToString(row["tag"]) ?? string.Empty;
                string permission = Convert.ToString(row["permission"]) ?? string.Empty;
                string type = Convert.ToString(row["type"]) ?? string.Empty;

                string type_ = "";
                string Active = "";

                if (Convert.ToInt32(id_tag) == SelectedTag[player.Index]) { Active = "✔️"; }

                if (Convert.ToInt32(type) == 1) { type_ = "ScoreBoard"; }

                if (IsInt(permission))
                {
                    var Get_Player_ID = player.SteamID.ToString();
                    if (permission == Get_Player_ID)
                    {
                        if (Convert.ToInt32(type) == 1)
                        {
                            menu.AddItem($"{name_tag} ({type_}) {Active}", (p, option) =>
                            {
                                SelectedTag[player.Index] = Convert.ToInt32(id_tag);
                                player.PrintToChat($" {Config.Prefix} {Localizer["SelectedTag", name_tag, type_]} ");
                                SaveSettings(player);
                                ChangeTag(player);
                                open_Tags(player);
                            });
                        }
                    }
                }
                else
                {
                    var get_permission = AdminManager.PlayerHasPermissions(player, permission);
                    if (get_permission)
                    {
                        if (Convert.ToInt32(type) == 1)
                        {
                            menu.AddItem($"{name_tag} ({type_}) {Active}", (p, option) =>
                            {
                                SelectedTag[player.Index] = Convert.ToInt32(id_tag);
                                player.PrintToChat($" {Config.Prefix} {Localizer["SelectedTag", name_tag, type_]} ");
                                SaveSettings(player);
                                ChangeTag(player);
                                open_Tags(player);
                            });
                        }
                    }
                }

            }
            menu.Display(player, 0);

        }
        public void open_CTags(CCSPlayerController player)
        {
            if (player == null) return;
            if (!Config.EnabledTags || !Tags) return;

            MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);

            MySqlQueryResult result = MySql!
                .Table("deadswim_tags")
                .Select();
            ChatMenu menu = new("Chat Tags", this);

            for (int i = 0; i < result.Rows; i++)
            {
                var row = result[i];
                if (row == null) return;


                string id_tag = Convert.ToString(row["id"]) ?? string.Empty;
                string name_tag = Convert.ToString(row["tag"]) ?? string.Empty;
                string permission = Convert.ToString(row["permission"]) ?? string.Empty;
                string type = Convert.ToString(row["type"]) ?? string.Empty;

                string type_ = "";
                string Active2 = "";

                if (Convert.ToInt32(id_tag) == SelectedTag2[player.Index]) { Active2 = "✔️"; }

                if (Convert.ToInt32(type) == 2) { type_ = "Chat"; }

                if (IsInt(permission))
                {
                    var Get_Player_ID = player.SteamID.ToString();
                    if (permission == Get_Player_ID)
                    {
                        if (Convert.ToInt32(type) == 2)
                        {
                            menu.AddItem($"{name_tag} ({type_}) {Active2}", (p, option) =>
                            {
                                SelectedTag2[player.Index] = Convert.ToInt32(id_tag);
                                player.PrintToChat($" {Config.Prefix} {Localizer["SelectedTag", name_tag, type_]} ");
                                SaveSettings(player);
                                ChangeTag(player);
                                open_Tags(player);
                            });
                        }
                    }
                }
                else
                {
                    var get_permission = AdminManager.PlayerHasPermissions(player, permission);
                    if (get_permission)
                    {
                        if (Convert.ToInt32(type) == 2)
                        {
                            menu.AddItem($"{name_tag} ({type_}) {Active2}", (p, option) =>
                            {
                                SelectedTag2[player.Index] = Convert.ToInt32(id_tag);
                                player.PrintToChat($" {Config.Prefix} {Localizer["SelectedTag", name_tag, type_]} ");
                                SaveSettings(player);
                                ChangeTag(player);
                                open_Tags(player);
                            });
                        }
                    }
                }

            }
            menu.Display(player, 0);

        }
    }
}
