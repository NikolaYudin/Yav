/* Host-side logic test: compiles the persistence/quest code paths of
 * yav_game.c semantics (copy) to validate save/load roundtrip. */
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

typedef struct { int x, y; } PPos;
static char g_nick[64];
static int g_selected, g_hpCur, g_hpMax, g_px, g_py, g_questStep, g_acorns, g_catTalked;

static void SaveProfileTo(const char* apath)
{
    FILE* f = fopen(apath, "w");
    fprintf(f,
        "{\n  \"version\": 2,\n  \"nickname\": \"%s\",\n  \"classId\": %d,\n"
        "  \"hp\": %d,\n  \"hpMax\": %d,\n  \"pos\": { \"x\": %d, \"y\": %d },\n"
        "  \"quest\": { \"step\": %d, \"acorns\": %d, \"catTalked\": %s }\n}\n",
        g_nick, g_selected, g_hpCur, g_hpMax, g_px, g_py,
        g_questStep, g_acorns, g_catTalked ? "true" : "false");
    fclose(f);
}
static int JsonGetInt(const char* js, const char* key, int def)
{
    char pat[64]; const char* p;
    snprintf(pat, sizeof(pat), "\"%s\"", key);
    p = strstr(js, pat); if (!p) return def;
    p += strlen(pat); p = strchr(p, ':'); if (!p) return def;
    return atoi(p + 1);
}
static void JsonGetStr(const char* js, const char* key, char* out, const char* def)
{
    char pat[64]; const char *p, *s, *e; int n;
    snprintf(pat, sizeof(pat), "\"%s\"", key);
    strcpy(out, def);
    p = strstr(js, pat); if (!p) return;
    p = strchr(p + strlen(pat), '"'); if (!p) return;
    s = p + 1; e = strchr(s, '"'); if (!e) return;
    n = (int)(e - s); if (n > 63) n = 63;
    memcpy(out, s, n); out[n] = 0;
}
static int LoadProfileFrom(const char* apath)
{
    FILE* f = fopen(apath, "rb"); long sz; char* buf; int ok = 0;
    if (!f) return 0;
    fseek(f,0,SEEK_END); sz=ftell(f); fseek(f,0,SEEK_SET);
    buf = malloc(sz+1); fread(buf,1,sz,f); buf[sz]=0; fclose(f);
    if (strstr(buf, "\"classId\"")) {
        g_selected = JsonGetInt(buf,"classId",-1);
        g_hpMax = JsonGetInt(buf,"hpMax",100);
        g_hpCur = JsonGetInt(buf,"hp",g_hpMax);
        g_px = JsonGetInt(buf,"x",320); g_py = JsonGetInt(buf,"y",380);
        g_questStep = JsonGetInt(buf,"step",0);
        g_acorns = JsonGetInt(buf,"acorns",0);
        g_catTalked = strstr(buf,"\"catTalked\": true")?1:0;
        JsonGetStr(buf,"nickname",g_nick,"");
        if (g_selected>=0 && g_selected<3 && g_nick[0]) ok=1;
    }
    free(buf); return ok;
}
int main(void)
{
    /* simulate quest flow with real-time saves */
    strcpy(g_nick, "\xd0\x9c\xd0\xb0\xd0\xba\xd0\xb0\xd1\x80"); /* Макарыч in UTF-8 */
    g_selected = 0; g_hpMax = 150; g_hpCur = 150; g_px = 320; g_py = 380;
    g_questStep = 0; g_acorns = 0; g_catTalked = 0;
    SaveProfileTo("/tmp/yav_test_profile.json");

    g_catTalked = 1; g_questStep = 2; g_acorns = 2; g_px = 300; g_py = 250;
    SaveProfileTo("/tmp/yav_test_profile.json");

    memset(g_nick,0,sizeof g_nick); g_selected=-1; g_questStep=0; g_acorns=0; g_catTalked=0;
    int ok = LoadProfileFrom("/tmp/yav_test_profile.json");
    printf("loaded=%d nick=%s class=%d hp=%d/%d pos=%d,%d step=%d acorns=%d talked=%d\n",
           ok, g_nick, g_selected, g_hpCur, g_hpMax, g_px, g_py, g_questStep, g_acorns, g_catTalked);
    if (!ok || g_selected != 0 || g_hpCur != 150 || g_px != 300 || g_py != 250 ||
        g_questStep != 2 || g_acorns != 2 || !g_catTalked) { printf("FAIL\n"); return 1; }
    printf("PASS: real-time save/load roundtrip OK\n");
    return 0;
}
