using UnityEngine;
using UnityEngine.UI;
using Yav.Core;

namespace Yav.Launch
{
    /// <summary>
    /// Окно запуска игры: логотип, фоновая музыка, кнопки "Играть"/"Настройки"/"Выход".
    /// UI не знает ничего про БД и классы — только дёргает события.
    /// Сборка панели: либо вручную в инспекторе, либо через меню
    /// GameObject -> Create Legacy UI... (или кнопкой "Build Panel" в редакторе).
    /// </summary>
    public class LaunchWindow : MonoBehaviour
    {
        [Header("Элементы (заполнить в инспекторе)")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private AudioSource musicSource;

        public readonly GameEvent OnPlayClicked = new GameEvent();
        public readonly GameEvent OnQuitClicked = new GameEvent();
        public readonly GameEvent OnSettingsClicked = new GameEvent();

        private void Awake()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

            playButton?.onClick.AddListener(() => OnPlayClicked.Raise());
            quitButton?.onClick.AddListener(() => OnQuitClicked.Raise());
            settingsButton?.onClick.AddListener(() => OnSettingsClicked.Raise());
        }

        public void Show()
        {
            gameObject.SetActive(true);
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            if (musicSource != null) musicSource.Play();
        }

        public void Hide()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            if (musicSource != null) musicSource.Stop();
            gameObject.SetActive(false);
        }
    }
}
