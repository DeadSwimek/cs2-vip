using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CustomPlugin;
using Nexd.MySQL;
using VIPAPI;
namespace CustomPlugin
{
    public class CoreAPI : IAPI
    {
        private CustomPlugin _api;
        public CoreAPI(CustomPlugin cp)
        {
            _api = cp;
        }

        public string GetVIPGroup(CCSPlayerController player)
        {
            var group = _api.GetGroup(player);
            return group;
        }

        public bool IsVIP(CCSPlayerController player)
        {
            var vip = _api.IsPlayerVip(player);
            return vip;
        }
        public void OpenTagMenu(CCSPlayerController player)
        {
            _api.OpenTagMenu(player);
        }
        public void OpenVIPMenu(CCSPlayerController player)
        {
            _api.OpenVIPMenu(player);
        }
        public void OpenModelsMenu(CCSPlayerController player)
        {
            _api.OpenModelsMenu(player);
        }
        public void SwitchQuake(CCSPlayerController player)
        {
            _api.quake(player);
        }
        public string GenerateVIP(CCSPlayerController target, string? type, string? Time)
        {
            if (target != null)
            {
                var token = _api.CreatePassword(20);
                var group = type;
                var time_vip = Time;
                if (group == null || group == "") { return "**VIP API ERROR** Group must be included! (1=VIP, 2=M-VIP, 3=MODELS)"; }
                if (time_vip == null || time_vip == "") { return "**VIP API ERROR** Time must be incuded! (0=FOREVER, >1 = HOURS)"; }

                var TimeToUTC = DateTime.UtcNow.AddDays(Convert.ToInt32(time_vip)).GetUnixEpoch();
                var timeofvip = 0;
                if (time_vip == "0")
                {
                    timeofvip = 0;
                }
                else
                {
                    timeofvip = DateTime.UtcNow.AddDays(Convert.ToInt32(time_vip)).GetUnixEpoch();
                }
                MySqlDb MySql = new MySqlDb(_api.Config.DBHost, _api.Config.DBUser, _api.Config.DBPassword, _api.Config.DBDatabase);
                MySqlQueryValue values = new MySqlQueryValue()
                .Add("token", token)
                .Add("end", $"{timeofvip}")
                .Add("`group`", group);
                MySql.Table($"deadswim_users_key_vip").Insert(values);

                return token;
            }
            else
            {
                return "error";
            }
        }
    }
}