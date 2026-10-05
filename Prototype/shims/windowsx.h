#pragma once
#define GET_X_LPARAM(lp) ((int)(short)((lp) & 0xFFFF))
#define GET_Y_LPARAM(lp) ((int)(short)(((lp) >> 16) & 0xFFFF))
