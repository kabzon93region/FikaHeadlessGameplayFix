using BepInEx.Logging;
using EFT;
using HarmonyLib;

namespace FikaHeadlessGameplayFix
{
    internal static class RaidLocationIdUtil
    {
        internal static bool Ensure(RaidSettings raidSettings, ManualLogSource logger, string source)
        {
            if (raidSettings?.SelectedLocation == null)
            {
                return false;
            }

            var mapId = raidSettings.SelectedLocation.Id;
            if (string.IsNullOrWhiteSpace(mapId))
            {
                mapId = Traverse.Create(raidSettings.SelectedLocation).Field<string>("_Id").Value;
            }

            if (string.IsNullOrWhiteSpace(mapId))
            {
                logger?.LogWarning($"[HEADLESS_FIX] {source}: SelectedLocation has no Id");
                return false;
            }

            if (raidSettings.LocationId == mapId)
            {
                return false;
            }

            logger?.LogInfo(
                $"[HEADLESS_FIX] {source}: RaidSettings.LocationId '{raidSettings.LocationId}' -> '{mapId}'");

            raidSettings.LocationId = mapId;
            return true;
        }
    }
}
