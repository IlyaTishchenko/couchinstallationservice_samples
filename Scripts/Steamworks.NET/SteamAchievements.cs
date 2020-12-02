using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !DISABLESTEAMWORKS
using Steamworks;
#endif

// Steam achievement helper class
public class SteamAchievements : MonoBehaviour
{
#if !DISABLESTEAMWORKS

    public static void UnlockSteamAchievement(string id)
    {
        if (!SteamManager.Initialized)
            return;

        bool unlocked;
        SteamUserStats.GetAchievement(id, out unlocked);

        if (!unlocked)
        {
            SteamUserStats.SetAchievement(id);
            SteamUserStats.StoreStats();
        }
    }

    public static void DebugLockSteamAchievement(string id)
    {
        if (!SteamManager.Initialized)
            return;

        bool unlocked;
        SteamUserStats.GetAchievement(id, out unlocked);

        if (unlocked)
        {
            SteamUserStats.ClearAchievement(id);
        }
    }

#endif
}
