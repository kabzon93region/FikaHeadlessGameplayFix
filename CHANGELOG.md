# Changelog — FikaHeadlessGameplayFix

## [1.0.3] — 2026-06-24

### Fixed
- **Убран** `HostBotInitLocationFixPatch`: подмена `GameWorld.LocationId` на MongoDB id (`653e6760…`) ломала SPT CustomAI (`key not in dictionary`) на Sandbox и других картах.
- Остаётся только fix `RaidSettings.LocationId` в `CreateFikaGame` (для `maxCap` ботов).

## [1.0.2] — 2026-06-22

### Fixed
- **Headless raid init:** `setstatus COMPLETE` не доходил до сервера (deadlock `async void` prefix + загрузка карты на main thread). Отправка через `Task.Run` при `FikaNetworkManagerCreatedEvent`.
- **Таймаут ожидания игроков:** 45 → 180 сек (`WaitForPlayersToConnect`).

### Changed
- Мод снова в keep-листе headless strip (раньше ошибочно убирали).

## [1.0.1] — 2026-06-21

### Fixed
- LocationId выставляется **prefix на `CreateFikaGame`** (до `LocalRaidStarted`), а не postfix на async `BeginFikaStartRaid`

## [1.0.0] — 2026-06-21

### Fixed
- **Спавн ботов на Fika Headless:** `BeginFikaStartRaid` не выставлял `RaidSettings.LocationId` → SPT запрашивал `maxCap/default`, волны ботов не шли
- Postfix на `FikaHeadlessPlugin.BeginFikaStartRaid`: `LocationId = SelectedLocation.Id`
- Prefix на `HostGameController.InitializeBotsSystem`: страховка `GameWorld.LocationId` перед загрузкой пресетов

### Added
- Логи `[HEADLESS_FIX]` в BepInEx/LogOutput.log
