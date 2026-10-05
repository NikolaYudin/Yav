#!/usr/bin/env bash
# ==========================================================================
# Сборка Windows-прототипа «ЯВЬ: Забытые Былины» (окно запуска + выбор героя)
# Требуется: python3, mingw-w64 (x86_64-w64-mingw32-gcc)
#   Debian/Ubuntu: apt-get install gcc-mingw-w64-x86-64
# Результат: Prototype/YavLaunch.exe — 64-битный exe для Windows 10/11
# ==========================================================================
set -e
cd "$(dirname "$0")/.."

echo "[1/3] Генерация исходника из данных классов..."
python3 Prototype/gen.py

echo "[2/3] Компиляция x86_64-w64-mingw32-gcc..."
x86_64-w64-mingw32-gcc -O2 -Wall -mwindows \
    Prototype/yav_prototype.c \
    -o Prototype/YavLaunch.exe \
    -lmsimg32 -lgdi32 -luser32 -lkernel32

echo "[3/3] Утяжка (strip)..."
x86_64-w64-mingw32-strip -s Prototype/YavLaunch.exe || true

ls -la Prototype/YavLaunch.exe
echo "Готово: Prototype/YavLaunch.exe"
