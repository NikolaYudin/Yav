# Явь: Забытые Былины (Yav: Forgotten Byliny)

Проект игры-гриндилки с мультиплеером (client-server, server-authoritative).

## Быстрый старт (тест на Windows 10/11)
Скачайте и распакуйте **`Yav_Win_Test.zip`** → запустите `YavLaunch.exe`.
Полная инструкция, управление и структура проекта — в **[README_RU.md](README_RU.md)**.

## Состав
- `Prototype/` — C-прототип (WinAPI), собирается `bash Prototype/build_game.sh` (mingw-w64)
- `Assets/`, `ProjectSettings/` — Unity-проект (C#): запуск, выбор героя, файловая БД
- `build_archives/` — архивы предыдущих версий билдов (пометка `_old`)
