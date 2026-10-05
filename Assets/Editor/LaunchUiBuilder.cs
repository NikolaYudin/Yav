using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Yav.Core;
using Yav.Data;
using Yav.Launch;

namespace Yav.EditorTools
{
    /// <summary>
    /// Меню: YAV -> Build Launch UI.
    /// Собирает в активной сцене стартовый Canvas с окном запуска и окном выбора
    /// персонажа, связывает все ссылки и GameBootstrapper — чтобы можно было
    /// сразу нажать Play и проверить флоу без ручной сборки в инспекторе.
    /// </summary>
    public static class LaunchUiBuilder
    {
        [MenuItem("YAV/Build Launch UI (scene setup)")]
        public static void Build()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.name != "00_Launch")
                Debug.LogWarning("[YAV][Editor] Активная сцена не 00_Launch — всё равно строю UI здесь.");

            // --- EventSystem + Camera, если нет ---
            if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            if (Camera.main == null)
            {
                var cam = new GameObject("Main Camera");
                cam.tag = "MainCamera";
                cam.AddComponent<Camera>();
            }

            // --- Root + Bootstrapper ---
            var root = new GameObject("GameRoot");
            var boot = root.AddComponent<GameBootstrapper>();

            // --- Canvas ---
            var canvasGO = new GameObject("LaunchCanvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            canvasGO.AddComponent<GraphicRaycaster>();

            var launcherPanel = MakePanel(canvasGO.transform, "LaunchWindow", new Color(0.08f, 0.07f, 0.06f));
            var selectPanel   = MakePanel(canvasGO.transform, "CharacterSelectWindow", new Color(0.10f, 0.09f, 0.07f));

            var launcher = launcherPanel.gameObject.AddComponent<LaunchWindow>();
            var selector = selectPanel.gameObject.AddComponent<CharacterSelectWindow>();

            // === Кнопки окна запуска ===
            var playBtn     = MakeButton(launcherPanel.transform, "PlayButton", "Играть",
                                         new Vector2(0, 40), new Vector2(320, 64), new Color(0.35f, 0.25f, 0.12f));
            var settingsBtn = MakeButton(launcherPanel.transform, "SettingsButton", "Настройки",
                                         new Vector2(0, -40), new Vector2(320, 48), new Color(0.22f, 0.20f, 0.16f));
            var quitBtn     = MakeButton(launcherPanel.transform, "QuitButton", "Выход",
                                         new Vector2(0, -100), new Vector2(320, 48), new Color(0.30f, 0.12f, 0.10f));

            MakeText(launcherPanel.transform, "Title", "ЯВЬ\nЗабытые Былины",
                     new Vector2(0, 220), new Vector2(900, 200), 64, FontStyle.BoldAndItalic);

            SetSo(boot, "launchWindow", launcher);
            SetSo(boot, "characterSelectWindow", selector);
            SetSo(launcher, "playButton", playBtn);
            SetSo(launcher, "settingsButton", settingsBtn);
            SetSo(launcher, "quitButton", quitBtn);

            // === Окно выбора: три карточки ===
            float[] xs = { -360, 0, 360 };
            CharacterClassId[] ids = { CharacterClassId.Bogatyr, CharacterClassId.Strelok, CharacterClassId.Volkhv };
            ClassCard[] cards = new ClassCard[3];

            for (int i = 0; i < 3; i++)
            {
                var def = CharacterClassDef.Get(ids[i]);
                var cardRect = MakePanel(selectPanel.transform, $"Card_{def.NameRu}", new Color(0.16f, 0.14f, 0.11f));
                Fit(cardRect, new Vector2(xs[i], 60), new Vector2(330, 420));

                var icon = MakeImage(cardRect.transform, "Icon", new Vector2(0, 150), new Vector2(96, 96));
                var title = MakeText(cardRect.transform, "Title", def.NameRu, new Vector2(0, 70), new Vector2(300, 50), 34, FontStyle.Bold);
                var stats = MakeText(cardRect.transform, "Stats", "", new Vector2(0, -60), new Vector2(300, 220), 16, FontStyle.Normal);
                var frame = MakeImage(cardRect.transform, "Frame", Vector2.zero, new Vector2(330, 420));
                frame.color = new Color(0.9f, 0.75f, 0.3f);
                var selBtn = MakeButton(cardRect.transform, "SelectButton", "Выбрать",
                                        new Vector2(0, -170), new Vector2(220, 50), new Color(0.30f, 0.22f, 0.10f));

                var card = cardRect.gameObject.AddComponent<ClassCard>();
                SetSo(card, "icon", icon);
                SetSo(card, "titleText", title);
                SetSo(card, "statsText", stats);
                SetSo(card, "selectButton", selBtn);
                SetSo(card, "selectionFrame", frame);
                frame.enabled = false;
                cards[i] = card;
            }

            SetSo(selector, "bogatyrCard", cards[0]);
            SetSo(selector, "strelokCard", cards[1]);
            SetSo(selector, "volkhvCard", cards[2]);

            var inputGO = MakeInputField(selectPanel.transform, "NicknameInput",
                                          new Vector2(0, -200), new Vector2(360, 48), "Имя героя...");
            var confirm = MakeButton(selectPanel.transform, "ConfirmButton", "В путь!",
                                      new Vector2(120, -270), new Vector2(220, 56), new Color(0.20f, 0.35f, 0.15f));
            var back    = MakeButton(selectPanel.transform, "BackButton", "Назад",
                                      new Vector2(-120, -270), new Vector2(220, 56), new Color(0.25f, 0.20f, 0.18f));
            var status  = MakeText(selectPanel.transform, "Status", "", new Vector2(0, -330), new Vector2(900, 40), 20, FontStyle.Italic);
            MakeText(selectPanel.transform, "Header", "Выбери героя былины",
                      new Vector2(0, 260), new Vector2(900, 60), 40, FontStyle.Bold);

            SetSo(selector, "nicknameInput", inputGO.GetComponent<InputField>());
            SetSo(selector, "confirmButton", confirm);
            SetSo(selector, "backButton", back);
            SetSo(selector, "statusText", status);

            // Панель выбора скрыта до нажатия "Играть"
            selectPanel.gameObject.SetActive(false);

            Debug.Log("[YAV][Editor] Стартовый UI собран. Нажми Play — игра начнётся с окна запуска.");
        }

        // ---------- helpers ----------

        private static RectTransform MakePanel(Transform parent, string name, Color bg)
        {
            var go = new GameObject(name);
            var img = go.AddComponent<Image>();
            img.color = bg;
            var rt = go.AddComponent<RectTransform>();
            rt.SetParent(parent, false);
            Stretch(rt);
            return rt;
        }

        private static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static void Fit(RectTransform rt, Vector2 pos, Vector2 size)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
            rt.anchoredPosition = pos;
        }

        private static Image MakeImage(Transform parent, string name, Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name);
            var img = go.AddComponent<Image>();
            var rt = go.AddComponent<RectTransform>();
            rt.SetParent(parent, false);
            Fit(rt, pos, size);
            return img;
        }

        private static Text MakeText(Transform parent, string name, string content,
                                     Vector2 pos, Vector2 size, int font, FontStyle style)
        {
            var go = new GameObject(name);
            var txt = go.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.text = content;
            txt.fontSize = font;
            txt.fontStyle = style;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = new Color(0.92f, 0.88f, 0.78f);
            txt.horizontalOverflow = HorizontalWrapMode.Wrap;
            txt.verticalOverflow = VerticalWrapMode.Overflow;
            var rt = go.AddComponent<RectTransform>();
            rt.SetParent(parent, false);
            Fit(rt, pos, size);
            return txt;
        }

        private static Button MakeButton(Transform parent, string name, string label,
                                         Vector2 pos, Vector2 size, Color color)
        {
            var go = new GameObject(name);
            var img = go.AddComponent<Image>();
            img.color = color;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            var rt = go.AddComponent<RectTransform>();
            rt.SetParent(parent, false);
            Fit(rt, pos, size);
            MakeText(go.transform, "Label", label, Vector2.zero, size, 26, FontStyle.Bold);
            return btn;
        }

        private static GameObject MakeInputField(Transform parent, string name, Vector2 pos, Vector2 size, string placeholder)
        {
            var go = new GameObject(name);
            var img = go.AddComponent<Image>();
            img.color = new Color(0.2f, 0.18f, 0.14f);
            var rt = go.AddComponent<RectTransform>();
            rt.SetParent(parent, false);
            Fit(rt, pos, size);

            var textGo = new GameObject("Text");
            var txt = textGo.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 22;
            txt.alignment = TextAnchor.MiddleLeft;
            txt.color = Color.white;
            var trt = textGo.AddComponent<RectTransform>();
            trt.SetParent(go.transform, false);
            trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
            trt.offsetMin = new Vector2(10, 4); trt.offsetMax = new Vector2(-10, -4);

            var phGo = new GameObject("Placeholder");
            var pht = phGo.AddComponent<Text>();
            pht.font = txt.font;
            pht.fontSize = 22;
            pht.fontStyle = FontStyle.Italic;
            pht.alignment = TextAnchor.MiddleLeft;
            pht.color = new Color(1, 1, 1, 0.4f);
            pht.text = placeholder;
            var prt = phGo.AddComponent<RectTransform>();
            prt.SetParent(go.transform, false);
            prt.anchorMin = Vector2.zero; prt.anchorMax = Vector2.one;
            prt.offsetMin = new Vector2(10, 4); prt.offsetMax = new Vector2(-10, -4);

            var field = go.AddComponent<InputField>();
            field.textComponent = txt;
            field.placeholder = pht;

            return go;
        }

        private static void SetSo(Object target, string fieldName, object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop == null)
            {
                Debug.LogError($"[YAV][Editor] Поле '{fieldName}' не найдено у {target.name}");
                return;
            }
            prop.objectReferenceValue = value as Object;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
