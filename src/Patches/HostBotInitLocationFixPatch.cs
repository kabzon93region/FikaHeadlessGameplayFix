using BepInEx.Logging;
using EFT;
using Fika.Core.Main.GameMode;
using HarmonyLib;

namespace FikaHeadlessGameplayFix
{
    /// <summary>
    /// Страховка перед загрузкой пресетов ботов: GameWorld.LocationId = карта рейда.
    /// </summary>
    internal static class HostBotInitLocationFixPatch
    {
        public static bool TryApply(Harmony harmony, ManualLogSource logger)
        {
            var method = AccessTools.Method(typeof(HostGameController), "InitializeBotsSystem");
            if (method == null)
            {
                logger.LogWarning("[HEADLESS_FIX] HostGameController.InitializeBotsSystem not found");
                return false;
            }

            harmony.Patch(
                method,
                prefix: new HarmonyMethod(typeof(HostBotInitLocationFixPatch), nameof(InitializeBotsSystemPrefix)));

            return true;
        }

        private static void InitializeBotsSystemPrefix(object location, object controllerSettings, GameWorld gameWorld)
        {
            if (location == null || gameWorld == null)
            {
                return;
            }

            var mapId = Traverse.Create(location).Property<string>("Id").Value;
            if (string.IsNullOrWhiteSpace(mapId))
            {
                mapId = Traverse.Create(location).Field<string>("_Id").Value;
            }

            if (string.IsNullOrWhiteSpace(mapId))
            {
                return;
            }

            if (gameWorld.LocationId == mapId)
            {
                return;
            }

            PluginCore.FixLogger?.LogInfo(
                $"[HEADLESS_FIX] GameWorld.LocationId '{gameWorld.LocationId}' -> '{mapId}' before bot init");

            gameWorld.LocationId = mapId;
        }
    }
}
