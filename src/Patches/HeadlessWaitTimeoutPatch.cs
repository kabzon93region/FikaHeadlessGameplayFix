using System;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using HarmonyLib;

namespace FikaHeadlessGameplayFix
{
    /// <summary>
    /// Fika.Headless ждёт 45 сек (лог «2 minutes»), затем убивает процесс если никто не подключился.
  /// Увеличиваем до 180 сек — запас на медленную загрузку Woods.
    /// </summary>
    internal static class HeadlessWaitTimeoutPatch
    {
        private const int WaitSeconds = 180;

        public static bool TryApply(Harmony harmony, ManualLogSource logger)
        {
            var pluginType = AccessTools.TypeByName("Fika.Headless.FikaHeadlessPlugin");
            if (pluginType == null)
            {
                logger.LogWarning("[HEADLESS_FIX] FikaHeadlessPlugin not found — wait timeout patch skipped");
                return false;
            }

            var method = AccessTools.Method(pluginType, "WaitForPlayersToConnect");
            if (method == null)
            {
                logger.LogWarning("[HEADLESS_FIX] WaitForPlayersToConnect not found");
                return false;
            }

            harmony.Patch(
                method,
                transpiler: new HarmonyMethod(typeof(HeadlessWaitTimeoutPatch), nameof(Transpiler)));

            logger.LogInfo($"[HEADLESS_FIX] WaitForPlayersToConnect timeout -> {WaitSeconds}s");
            return true;
        }

        private static System.Collections.Generic.IEnumerable<CodeInstruction> Transpiler(
            System.Collections.Generic.IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_R8 && instruction.operand is double value && Math.Abs(value - 45d) < 0.001d)
                {
                    yield return new CodeInstruction(OpCodes.Ldc_R8, (double)WaitSeconds);
                    continue;
                }

                yield return instruction;
            }
        }
    }
}
