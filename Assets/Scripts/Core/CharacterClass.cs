using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yav.Core
{
    /// <summary>Идентификаторы стартовых классов.</summary>
    public enum CharacterClassId
    {
        Bogatyr = 0,  // Богатырь — танк/ближний бой
        Strelok = 1,  // Стрелок — дальний физ. урон
        Volkhv  = 2   // Волхв — магия/поддержка
    }

    /// <summary>
    /// Чистые данные класса (без MonoBehaviour!) — пример разделения логики и визуала.
    /// Эти же параметры будут валидироваться на сервере, когда появится сеть.
    /// </summary>
    [Serializable]
    public class CharacterClassDef
    {
        public CharacterClassId Id;
        public string NameRu;        // Отображаемое имя
        public string DescriptionRu; // Краткое описание для окна выбора
        public string ColorHex;      // Цвет иконки/карточки в UI

        // Базовые характеристики (гриндилка: HP/урон/ресурс)
        public int BaseHealth;
        public int BaseDamage;
        public float AttackSpeed;    // ударов в секунду
        public float MoveSpeed;      // м/с
        public int BaseEnergy;       // мана/выносливость
        public int CarryWeightBonus; // бонус к переносимому весу (добыча ресурсов)

        public static readonly IReadOnlyDictionary<CharacterClassId, CharacterClassDef> All =
            new Dictionary<CharacterClassId, CharacterClassDef>
            {
                [CharacterClassId.Bogatyr] = new CharacterClassDef
                {
                    Id = CharacterClassId.Bogatyr,
                    NameRu = "Богатырь",
                    DescriptionRu = "Крестьянский сын, поднявший дубовый палицу. Крепкий телом, " +
                                    "идёт вперёд — рубит нежить в ближнем бою и носит больше всех лута.",
                    ColorHex = "#8B3A2F",
                    BaseHealth = 150,
                    BaseDamage = 18,
                    AttackSpeed = 0.9f,
                    MoveSpeed = 4.2f,
                    BaseEnergy = 60,
                    CarryWeightBonus = 50
                },
                [CharacterClassId.Strelok] = new CharacterClassDef
                {
                    Id = CharacterClassId.Strelok,
                    NameRu = "Стрелок",
                    DescriptionRu = "Охотник-следопыт из глухих лесов. Бьёт издалека точной стрелой, " +
                                    "быстро ходит и быстро гибнет вблизи.",
                    ColorHex = "#3F6B3A",
                    BaseHealth = 100,
                    BaseDamage = 24,
                    AttackSpeed = 1.4f,
                    MoveSpeed = 5.2f,
                    BaseEnergy = 80,
                    CarryWeightBonus = 20
                },
                [CharacterClassId.Volkhv] = new CharacterClassDef
                {
                    Id = CharacterClassId.Volkhv,
                    NameRu = "Волхв",
                    DescriptionRu = "Носитель древних заговоров. Творит огонь и мороки, но телом немощен. " +
                                    "В группе — поддержка и контроль толпы.",
                    ColorHex = "#4A4A7A",
                    BaseHealth = 80,
                    BaseDamage = 30,
                    AttackSpeed = 0.7f,
                    MoveSpeed = 4.0f,
                    BaseEnergy = 160,
                    CarryWeightBonus = 0
                }
            };

        public static CharacterClassDef Get(CharacterClassId id) => All[id];
    }
}
