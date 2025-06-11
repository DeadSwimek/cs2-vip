using CounterStrikeSharp.API.Core;
using System.Collections.Generic;
using System;

namespace VIPAPI;

public interface IAPI
{
    bool IsVIP(CCSPlayerController player);
    string GetVIPGroup(CCSPlayerController player);
}