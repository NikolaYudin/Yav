using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Yav.Data;
using Yav.Launch;
using Yav.Save;

namespace Yav.Core
{
    /// <summary>
    /// Точка входа игры. Вешается на GameObject "GameRoot" в сцене 00_Launch.
    /// Порядок: загрузить сохранения -> показать окно запуска -> выбор класса -> вход в мир.
    /// ВАЖНО: здесь только оркестрация, никакая игровая логика не живёт в UI.
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("Пути к файловым БД (относительно persistentDataPath)")]
        [SerializeField] private string playersDbPath = "db/players.json";
        [SerializeField] private string charactersDbPath = "db/characters.json";
        [SerializeField] private string settingsDbPath = "db/settings.json";

        [Header("Ссылки на UI-сцены/панели (заполняются в инспекторе)")]
        [SerializeField] private LaunchWindow launchWindow;
        [SerializeField] private CharacterSelectWindow characterSelectWindow;

        public static GameBootstrapper Instance { get; private set; }

        public readonly GameEvent<CharacterClassId> OnCharacterChosen = new GameEvent<CharacterClassId>();

        // Временная "шина" до появления сети: вместо прямых событий используем
        // пары "последние значения", чтобы не зависеть от порядка подписки в инспекторе.
        public CharacterClassId PendingClassId { get; private set; }
        public string PendingNickname { get; private set; }
        public event System.Action OnEnterWorldRequested;

        public FilePlayerRepository Players { get; private set; }
        public FileCharacterRepository Characters { get; private set; }
        public GameSettings Settings { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // --- Инициализация файловых БД ---
            string root = Application.persistentDataPath;
            Players = new FilePlayerRepository(Path.Combine(root, playersDbPath));
            Characters = new FileCharacterRepository(Path.Combine(root, charactersDbPath));
            Settings = GameSettings.Load(Path.Combine(root, settingsDbPath));

            Debug.Log($"[YAV] persistentDataPath = {root}");
        }

        private void Start()
        {
            // --- Окно запуска ---
            launchWindow.OnPlayClicked.Subscribe(ShowCharacterSelect);
            launchWindow.OnQuitClicked.Subscribe(() => Application.Quit());
            launchWindow.Show();

            // --- Выбор персонажа ---
            characterSelectWindow.OnConfirm.Subscribe(HandleCharacterConfirmed);
            characterSelectWindow.OnBack.Subscribe(() =>
            {
                characterSelectWindow.Hide();
                launchWindow.Show();
            });
        }

        private void ShowCharacterSelect()
        {
            launchWindow.Hide();

            var existing = Characters.GetAll();
            characterSelectWindow.Open(existing, Settings.LastUsedClass);
        }

        private void HandleCharacterConfirmed(CharacterClassId classId, string nickname)
        {
            // 1. Игрок (аккаунт) — пока один локальный профиль
            var player = Players.GetOrCreateDefault(nickname);

            // 2. Персонаж — server-authoritative данные лежат в репозитории,
            //    клиент лишь отправляет "запрос на создание".
            var character = Characters.Create(player.Id, classId, nickname);

            Settings.LastUsedClass = classId;
            Settings.LastNickname = nickname;
            Settings.Save();

            Debug.Log($"[YAV] Выбран класс: {classId}, персонаж: {character.Id}");

            characterSelectWindow.Hide();

            // Запоминаем выбор и сигналим тем, кто готов войти в мир
            PendingClassId = classId;
            PendingNickname = nickname;
            OnCharacterChosen.Raise(classId);
            OnEnterWorldRequested?.Invoke();

            // TODO (следующий шаг): SceneManager.LoadScene("01_World") / загрузка мира
        }
    }
}
