using CounterStrikeSharp.API.Core;
using System.Collections.Generic;
using System;

namespace VIPAPI;

public interface IAPI
{
    bool IsVIP(CCSPlayerController player);
    string GetVIPGroup(CCSPlayerController player);
    public void OpenTagMenu(CCSPlayerController player);
    public void OpenVIPMenu(CCSPlayerController player);
    public void OpenModelsMenu(CCSPlayerController player);
    public void SwitchQuake(CCSPlayerController player);
    public string GenerateVIP(CCSPlayerController target, string? type, string? Time);
}
