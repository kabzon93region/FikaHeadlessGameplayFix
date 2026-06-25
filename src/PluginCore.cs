using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace FikaHeadlessGameplayFix
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    [BepInDependency("com.fika.core", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.fika.headless", BepInDependency.DependencyFlags.SoftDependency)]
    public sealed class PluginCore : BaseUnityPlugin
    {
        internal static ManualLogSource FixLogger;

        private void Awake()
        {
            FixLogger = Logger;
            var harmony = new Harmony(PluginInfo.GUID);

            HeadlessSetStatusFix.Initialize(Logger);

            if (HeadlessRaidLocationFixPatch.TryApply(harmony, Logger))
            {
                Logger.LogInfo("[HEADLESS_FIX] Headless raid LocationId fix applied");
            }

            if (HeadlessWaitTimeoutPatch.TryApply(harmony, Logger))
            {
                Logger.LogInfo("[HEADLESS_FIX] Headless wait-for-players timeout patch applied");
            }

            Logger.LogInfo($"{PluginInfo.NAME} v{PluginInfo.VERSION} loaded");
        }

        private void OnDestroy()
        {
            HeadlessSetStatusFix.Shutdown();
        }
    }
}
