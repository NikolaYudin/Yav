#!/bin/bash
cd "$(dirname "$0")"
x86_64-w64-mingw32-gcc -O2 -std=gnu++17 -mwindows -w -Ishims yav_game.c -o YavLaunch.exe -lmsimg32 -lgdi32 -luser32 -lkernel32 -lws2_32 -lgdiplus && echo BUILD_OK
