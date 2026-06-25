using BepInEx.Logging;
using EFT;
using HarmonyLib;

namespace FikaHeadlessGameplayFix
{
    /// <summary>
    /// Fika Headless CreateFikaGame читает raidSettings.LocationId для LocalRaidStarted → без Id SPT видит maxCap/default.
    /// Postfix на BeginFikaStartRaid не срабатывает вовремя (async state machine).
    /// </summary>
    internal static class HeadlessRaidLocationFixPatch
    {
        private const string PatchTypeName = "Fika.Headless.Patches.GameMode.Headless_LocalGameCreator_Patch";

        public static bool TryApply(Harmony harmony, ManualLogSource logger)
        {
            var patchType = AccessTools.TypeByName(PatchTypeName);
            if (patchType == null)
            {
                logger.LogWarning("[HEADLESS_FIX] Headless_LocalGameCreator_Patch not found — LocationId fix skipped");
                return false;
            }

            var method = AccessTools.Method(patchType, "CreateFikaGame");
            if (method == null)
            {
                logger.LogWarning("[HEADLESS_FIX] CreateFikaGame not found");
                return false;
            }

            harmony.Patch(
                method,
                prefix: new HarmonyMethod(typeof(HeadlessRaidLocationFixPatch), nameof(CreateFikaGamePrefix)));

            return true;
        }

        private static void CreateFikaGamePrefix(RaidSettings raidSettings)
        {
            RaidLocationIdUtil.Ensure(raidSettings, PluginCore.FixLogger, "CreateFikaGame");
        }
    }
}
