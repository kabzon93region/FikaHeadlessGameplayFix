using System;
using System.Threading.Tasks;
using BepInEx.Logging;
using Fika.Core.Main.Utils;
using Fika.Core.Modding;
using Fika.Core.Modding.Events;
using Fika.Core.Networking.Http;
using Fika.Core.Networking.Models;
using Fika.Core.UI.Models;

namespace FikaHeadlessGameplayFix
{
    /// <summary>
    /// Fika prefix на method_41 — async void; await UpdateSetStatus может зависнуть,
    /// пока основной поток грузит карту. sethost (sync PutJson) проходит, setstatus — нет.
    /// </summary>
    internal sealed class HeadlessSetStatusFix
    {
        private static bool _subscribed;
        private static bool _sentForSession;
        private static ManualLogSource _logger;

        public static void Initialize(ManualLogSource logger)
        {
            if (_subscribed)
            {
                return;
            }

            _logger = logger;
            FikaEventDispatcher.SubscribeEvent<FikaNetworkManagerCreatedEvent>(OnNetworkManagerCreated);
            FikaEventDispatcher.SubscribeEvent<FikaNetworkManagerDestroyedEvent>(OnNetworkManagerDestroyed);
            _subscribed = true;
        }

        public static void Shutdown()
        {
            if (!_subscribed)
            {
                return;
            }

            FikaEventDispatcher.UnsubscribeEvent<FikaNetworkManagerCreatedEvent>(OnNetworkManagerCreated);
            FikaEventDispatcher.UnsubscribeEvent<FikaNetworkManagerDestroyedEvent>(OnNetworkManagerDestroyed);
            _subscribed = false;
            _sentForSession = false;
        }

        private static void OnNetworkManagerDestroyed(FikaNetworkManagerDestroyedEvent evt)
        {
            _sentForSession = false;
        }

        private static void OnNetworkManagerCreated(FikaNetworkManagerCreatedEvent evt)
        {
            if (!FikaBackendUtils.IsServer || !FikaBackendUtils.IsHeadless)
            {
                return;
            }

            if (_sentForSession)
            {
                return;
            }

            _sentForSession = true;
            var groupId = FikaBackendUtils.GroupId;

            _ = Task.Run(async () =>
            {
                try
                {
                    var status = new SetStatusModel(groupId, LobbyEntry.ELobbyStatus.COMPLETE);
                    await FikaRequestHandler.UpdateSetStatus(status).ConfigureAwait(false);
                    _logger?.LogInfo("[HEADLESS_FIX] /fika/update/setstatus COMPLETE sent (off main thread)");
                }
                catch (Exception ex)
                {
                    _logger?.LogError($"[HEADLESS_FIX] setstatus failed: {ex.Message}");
                }
            });
        }
    }
}
