using CounterStrikeSharp.API.Core;
using CustomPlugin;
using VIPAPI;
namespace CustomPlugin
{
    public class CoreAPI : IAPI
    {
        private CustomPlugin _api;
        public CoreAPI(CustomPlugin cp)
        {
            _api = cp;  // Initialize it through the constructor
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
    }
}