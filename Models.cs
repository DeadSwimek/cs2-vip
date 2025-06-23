using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

using Newtonsoft.Json.Linq;
using Nexd.MySQL;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using CS2MenuManager;

namespace CustomPlugin
{
    public partial class CustomPlugin
    {
        public void SetModel(CCSPlayerController? player, string path, string name)
        {
            var pawn = player.Pawn.Value!;
            var originalRender = pawn?.Render;
            if (SelectedModel[player.Index] == 0)
            {
                return;
            }
            AddTimer(1.0f, () =>
            {
                if (path != null || SelectedModel[player.Index] >= 1)
                {
                    pawn.SetModel(path);
                    //pawn.Render = Color.FromArgb(255, originalRender.Value.R, originalRender.Value.G, originalRender.Value.B);
                    //Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");
                    //Utilities.SetStateChanged(pawn, "CBaseEntity", "m_CBodyComponent");
                    player.PrintToChat($" {Config.Prefix} {Localizer["LoadedModel", name]}");
                }
            });
        }
        public void PrecacheResource(ResourceManifest mainfest)
        {
            if (!Config.ModelsEnabled) return;
            if (!Config.Precache) return;
            List<string> Resource = new List<string>();
            MySqlDb MySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);
            MySqlQueryResult result = MySql!
                .Table("deadswim_models")
                .Select();
            for (int i = 0; i < result.Rows; i++)
            {
                var row = result[i];
                if (row == null) return;
                string path = row["path"].ToString();

                Resource.Add(path);
                mainfest.AddResource(path);
                Server.PrecacheModel(path);

            }
            Resource.Add(Config.NadeModel);
            foreach (string data in Resource)
            {
                mainfest.AddResource(data);
                Server.PrintToConsole($"--->>>Loading Resource<<<--- PATH: {data}");
            }
        }

    }
}
