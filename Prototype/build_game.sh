#!/usr/bin/env bash
# ==========================================================================
# Сборка Windows-прототипа «ЯВЬ: Забытые Былины» v0.2
# (окно запуска + выбор героя + деревня с дубом и Котом Учёным)
# Требуется: mingw-w64 (x86_64-w64-mingw32-gcc)
#   Debian/Ubuntu: apt-get install gcc-mingw-w64-x86-64
# Результат: Prototype/YavLaunch.exe — 64-битный exe для Windows 10/11
# ==========================================================================
set -e
cd "$(dirname "$0")"

echo "[1/2] Компиляция x86_64-w64-mingw32-gcc..."
x86_64-w64-mingw32-gcc -O2 -Wall -mwindows \
    yav_game.c \
    -o YavLaunch.exe \
    -lmsimg32 -lgdi32 -luser32 -lkernel32

echo "[2/2] Утяжка (strip)..."
x86_64-w64-mingw32-strip -s YavLaunch.exe || true

ls -la YavLaunch.exe
echo "Готово: Prototype/YavLaunch.exe"
