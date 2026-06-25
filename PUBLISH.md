# Publish to GitHub — Fika Headless Gameplay Fix

**Статус:** `ready`  
**GitHub:** Release + zip  
**Версия:** `1.0.3`  
**Deployment:** `(headless_host)`

## 1. Подготовка (уже сделано этим скриптом)

Папка: `github-repos/FikaHeadlessGameplayFix/`

## 2. Создать репозиторий и запушить

```powershell
cd github-repos/FikaHeadlessGameplayFix
git init
git add .
git commit -m "Source backup Fika Headless Gameplay Fix v1.0.3"
git branch -M main
git remote add origin https://github.com/${GITHUB_OWNER:-YOUR_GITHUB_USER}/FikaHeadlessGameplayFix.git
git push -u origin main
```

Или автоматически:

```powershell
python CURSORAIMODING/tools/publish/publish_github_release.py FikaHeadlessGameplayFix --create-repo
```

## 3. GitHub Release

Прикрепить zip (только игровые файлы, без INSTALL.md):

`\\Servant\data\Games\EscapeFromTarkov4\CURSORAIMODING\releases\FikaHeadlessGameplayFix_(headless_host)_v1.0.3_2026-06-26.zip`

```powershell
gh release create v1.0.3 "\\Servant\data\Games\EscapeFromTarkov4\CURSORAIMODING\releases\FikaHeadlessGameplayFix_(headless_host)_v1.0.3_2026-06-26.zip" ^
  --title "Fika Headless Gameplay Fix v1.0.3" ^
  --notes-file CHANGELOG.md
```

## Описание репозитория (suggested)

LocationId, таймауты и gameplay-фиксы на Fika Headless.

SPT 4.0 + Fika 2.3 headless stack. Deployment: `(headless_host)`.
