using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yav.Core;
using Yav.Data;

namespace Yav.Launch
{
    /// <summary>
    /// Окно выбора персонажа: три карточки классов + поле имени + кнопки.
    /// Логика выбора (какой класс, какое имя) возвращается в GameBootstrapper
    /// через события — окно ничего не пишет в БД само.
    /// </summary>
    public class CharacterSelectWindow : MonoBehaviour
    {
        [Header("Карточки классов")]
        [SerializeField] private ClassCard bogatyrCard;
        [SerializeField] private ClassCard strelokCard;
        [SerializeField] private ClassCard volkhvCard;

        [Header("Управление")]
        [SerializeField] private InputField nicknameInput;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Text statusText;
        [SerializeField] private CanvasGroup canvasGroup;

        public readonly GameEvent<(CharacterClassId classId, string nickname)> OnConfirm =
            new GameEvent<(CharacterClassId, string)>();
        public readonly GameEvent OnBack = new GameEvent();

        private CharacterClassId _selected = CharacterClassId.Bogatyr;

        private void Awake()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

            bogatyrCard?.Setup(CharacterClassDef.Get(CharacterClassId.Bogatyr), SelectClass);
            strelokCard?.Setup(CharacterClassDef.Get(CharacterClassId.Strelok), SelectClass);
            volkhvCard?.Setup(CharacterClassDef.Get(CharacterClassId.Volkhv), SelectClass);

            confirmButton?.onClick.AddListener(Confirm);
            backButton?.onClick.AddListener(() => OnBack.Raise());
        }

        /// <summary>Открывает окно: подсвечивает последний использованный класс.</summary>
        public void Open(IReadOnlyList<CharacterRecord> existingCharacters, CharacterClassId lastUsed)
        {
            gameObject.SetActive(true);
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            SelectClass(lastUsed);

            if (existingCharacters != null && existingCharacters.Count > 0)
            {
                // Пока показываем количество уже созданных персонажей.
                // Позже здесь появится список "продолжить игру" вместо создания нового.
                SetStatus($"В летописи уже записано героев: {existingCharacters.Count}. Новый подвиг начнётся с чистого листа.");
            }
            else
            {
                SetStatus("Выбери героя, скажи имя его — и ступай в Явь.");
            }
        }

        public void Hide()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        }

        private void SelectClass(CharacterClassId id)
        {
            _selected = id;
            bogatyrCard?.SetSelected(id == CharacterClassId.Bogatyr);
            strelokCard?.SetSelected(id == CharacterClassId.Strelok);
            volkhvCard?.SetSelected(id == CharacterClassId.Volkhv);
        }

        private void Confirm()
        {
            var name = nicknameInput != null ? nicknameInput.text.Trim() : "";
            if (string.IsNullOrEmpty(name))
            {
                SetStatus("Как тебя величать? Впиши имя богатырское!");
                return;
            }
            if (name.Length > 20)
            {
                SetStatus("Имя слишком длинное — не уместится в былине (макс. 20).");
                return;
            }

            OnConfirm.Raise((_selected, name));
        }

        private void SetStatus(string message)
        {
            if (statusText != null) statusText.text = message;
            Debug.Log($"[YAV][Select] {message}");
        }
    }
}
