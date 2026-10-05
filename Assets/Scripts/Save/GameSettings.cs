using System;
using System.IO;
using UnityEngine;
using Yav.Core;

namespace Yav.Save
{
    /// <summary>Лёгкие настройки клиента (не игровая логика!). settings.json.</summary>
    [Serializable]
    public class GameSettings
    {
        public CharacterClassId LastUsedClass = CharacterClassId.Bogatyr;
        public string LastNickname = "";
        public bool MusicOn = true;
        public bool SfxOn = true;
        public int TargetFrameRate = 60; // для слабых Android-устройств можно снизить до 30

        private string _path;

        public static GameSettings Load(string path)
        {
            GameSettings s = null;
            try
            {
                if (File.Exists(path))
                    s = JsonUtility.FromJson<GameSettings>(File.ReadAllText(path));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[YAV][Settings] Не удалось прочитать {path}: {e.Message}");
            }
            s ??= new GameSettings();
            s._path = path;
            return s;
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(_path)) return;
            var dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(_path, JsonUtility.ToJson(this, true));
        }
    }
}
