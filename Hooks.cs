
using System.Drawing;

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
using System.Runtime.ExceptionServices;
using CounterStrikeSharp.API.Core.Attributes;
using Microsoft.Extensions.Logging;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Memory;
using System.Security.Cryptography;
using System.Net;
using CounterStrikeSharp.API.Modules.Events;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;
using System;
using Nexd.MySQL;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Numerics;
using static StoreApi.Store;
using WASDSharedAPI;
using CounterStrikeSharp.API.Core.Capabilities;
using TagsApi;


namespace CustomPlugin
{
    public partial class CustomPlugin
    {
        public static CustomPlugin ?It;
        public readonly string[] projectiles = ["hegrenade_projectile", "flashbang_projectile", "smokegrenade_projectile", "decoy_projectile", "molotov_projectile"];

        public static IWasdMenuManager? MenuManager;
        public IWasdMenuManager? GetMenuManager()
        {
            if (MenuManager == null)
                MenuManager = new PluginCapability<IWasdMenuManager>("wasdmenu:manager").Get();

            return MenuManager;
        }
        internal void replace_weapon(CCSPlayerController player, string weapon_s)
        {
            AddTimer(0.1f, () =>
            {
                GiveWeapon(player, weapon_s); 
            });
        }
        [GameEventHandler]
        public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
        {
            timer_ex?.Kill();
            Bomb = false;
            bombtime = 0.0f;
            Server.PrintToConsole("MadGames.eu BOMB INFO - Ukončuji, je konec kola");
            return HookResult.Continue;
        }
        public bool PlayerHasWeapon(CCSPlayerController player, string designerName)
        {
            if (player == null || !player.PlayerPawn.IsValid)
            {
                return false;
            }

            var weaponServices = player!.PlayerPawn!.Value!.WeaponServices;
            if (weaponServices == null)
            {
                return false;
            }

            foreach (var weaponHandle in weaponServices.MyWeapons)
            {
                var weapon = weaponHandle.Value;
                if (weapon != null && weapon.IsValid)
                {

                    if (weapon.DesignerName == designerName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        [GameEventHandler]
        public HookResult KillPlayer(EventPlayerDeath @event, GameEventInfo info)
        {
            var attacker = @event.Attacker;
            var victin = @event.Userid;

            if (attacker == null) return HookResult.Continue;


            if (Config.More_Credit)
            {
                if (CSStore[attacker.Index] == 1)
                {
                    if (StoreApi == null) throw new Exception("StoreApi could not be located.");
                    int credits = Config.Credits_For_Kill;

                    StoreApi.GivePlayerCredits(attacker, credits);

                    attacker.PrintToChat($" {Config.Prefix} {Localizer["GiveMoneyForKill", credits]}");
                }
            }

            return HookResult.Continue;
        }
        [GameEventHandler]
        public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            timer_ex?.Kill();
            Bomb = false;
            bombtime = 0.0f;
            Round++;
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                var ent = NativeAPI.GetEntityFromIndex(i);
                var client = new CCSPlayerController(ent);
                Server.NextFrame(() =>
                {
                    if (client == null || !client.IsValid || client.IsBot || !client.PawnIsAlive) return;
                    Selected_round[client.Index] = 0;
                    if (Timestamp[client.Index] == null) { Timestamp[client.Index] = -1; }
                    if (Health[client.Index] == 1)
                    {
                        client.PlayerPawn!.Value!.Health = Config.StartHealth;
                        Utilities.SetStateChanged(client.PlayerPawn.Value, "CBaseEntity", "m_iHealth");
                        client.PlayerPawn!.Value!.ArmorValue = 100;
                        Utilities.SetStateChanged(client.PlayerPawn.Value, "CCSPlayerPawn", "m_ArmorValue");
                        client.PlayerPawn!.Value!.MaxHealth = Config.StartHealth;
                    }
                    if (Healthshot[client.Index] == 1)
                    {
                        if(!PlayerHasWeapon(client, "weapon_healthshot"))
                        {
                            client.GiveNamedItem("weapon_healthshot");
                        }
                    }
                    CreditEnable[client.Index] = 1;
                    AddTimer(4.0f, () => { CreditEnable[client.Index] = 0; });
                    It?.GiveWeapon_round(client);
                });
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

            if(player == null) return HookResult.Continue;

            if (Config.More_Credit2)
            {
                if (CSStore[player.Index] == 1)
                {
                    if (StoreApi == null) throw new Exception("StoreApi could not be located.");
                    int credits = Config.Credits_For_Plant;

                    StoreApi.GivePlayerCredits(player, credits);

                    player.PrintToChat($" {Config.Prefix} {Localizer["GiveMoneyForPlant", credits]}");
                }
            }

            Bomb = true;
            bombtime = 40.0f;
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
                }

                bombtime = bombtime - 1.0f;
            }, TimerFlags.REPEAT);

            return HookResult.Continue;
        }
        public void Explosive()
        {
            if (bombtime == 0)
            {
                timer_ex?.Kill();
            }
            bombtime = bombtime - 1.0f;
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
            var player = @event.Userid;
            if (player == null) return HookResult.Continue;

            if (Config.More_Credit3)
            {
                if (CSStore[player.Index] == 1)
                {
                    if (StoreApi == null) throw new Exception("StoreApi could not be located.");
                    int credits = Config.Credits_For_Defuse;

                    StoreApi.GivePlayerCredits(player, credits);

                    player.PrintToChat($" {Config.Prefix} {Localizer["GiveMoneyForDefuse", credits]}");
                }
            }

            Bomb = false;
            timer_ex?.Kill();
            return HookResult.Continue;
        }
    }
}