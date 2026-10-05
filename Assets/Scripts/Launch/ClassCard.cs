using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yav.Core;

namespace Yav.Launch
{
    /// <summary>
    /// Карточка одного класса в окне выбора (Богатырь / Стрелок / Волхв).
    /// Визуал полностью читает данные из CharacterClassDef, сам ничего не решает.
    /// </summary>
    public class ClassCard : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text titleText;
        [SerializeField] private Text statsText;
        [SerializeField] private Button selectButton;
        [SerializeField] private Image selectionFrame;

        public CharacterClassId ClassId { get; private set; }

        public void Setup(CharacterClassDef def, System.Action<CharacterClassId> onSelected)
        {
            ClassId = def.Id;

            if (titleText != null) titleText.text = def.NameRu;
            if (statsText != null)
            {
                statsText.text =
                    $"Здоровье: {def.BaseHealth}\n" +
                    $"Урон: {def.BaseDamage}  Скорость: {def.AttackSpeed:0.0}/с\n" +
                    $"Бег: {def.MoveSpeed:0.0} м/с\n" +
                    $"Энергия: {def.BaseEnergy}  Несомый груз: +{def.CarryWeightBonus}\n\n" +
                    def.DescriptionRu;
            }

            var color = Color.white;
            if (icon != null && ColorUtility.TryParseHtmlString(def.ColorHex, out color))
                icon.color = color;

            selectButton?.onClick.RemoveAllListeners();
            selectButton?.onClick.AddListener(() => onSelected?.Invoke(ClassId));
        }

        public void SetSelected(bool selected)
        {
            if (selectionFrame != null) selectionFrame.enabled = selected;
        }
    }
}
