using System;
using UnityEngine;
using Yav.Core;

namespace Yav.Data
{
    /// <summary>Запись игрока (аккаунта) в файловой БД players.json.</summary>
    [Serializable]
    public class PlayerRecord
    {
        public string Id = Guid.NewGuid().ToString("N");
        public string Nickname = "";
        public long CreatedUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public int TotalPlaytimeSec = 0;
    }

    /// <summary>
    /// Запись персонажа в файловой БД characters.json.
    /// Это "server-authoritative" состояние: здесь лежат только те данные,
    /// которые сервер (позже — реальный сетевой сервер) считает истинными.
    /// Никаких полей для визуала (анимации, эффекты) тут быть не должно.
    /// </summary>
    [Serializable]
    public class CharacterRecord
    {
        public string Id = Guid.NewGuid().ToString("N");
        public string OwnerPlayerId = "";
        public string Nickname = "";
        public CharacterClassId ClassId = CharacterClassId.Bogatyr;

        public int Level = 1;
        public int Experience = 0;

        // Текущее состояние (то, что в мультиплеере валидирует сервер)
        public int Health;
        public int Energy;

        // Позиция/сцена для возврата в мир (дальше добавим инвентарь)
        public string LastScene = "";
        public Vector3 LastPosition;

        public long CreatedUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public static CharacterRecord Create(string ownerId, CharacterClassId classId, string nickname)
        {
            var def = CharacterClassDef.Get(classId);
            return new CharacterRecord
            {
                OwnerPlayerId = ownerId,
                Nickname = nickname,
                ClassId = classId,
                Health = def.BaseHealth,
                Energy = def.BaseEnergy
            };
        }
    }
}
