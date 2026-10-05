using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Yav.Core;

namespace Yav.Data
{
    /// <summary>
    /// Обёртка-лист для JsonUtility (он не сериализует голый List&lt;T&gt;).
    /// </summary>
    [Serializable]
    public class Document<T>
    {
        public List<T> Items = new List<T>();
    }

    /// <summary>
    /// Простейшая файловая БД: один JSON-файл = одна таблица.
    /// Позже меняется на PostgreSQL/Firebase без изменения вызывающего кода,
    /// если обращаться только через интерфейс IRepository&lt;T&gt;.
    /// </summary>
    public interface IRepository<T>
    {
        IReadOnlyList<T> GetAll();
        T GetById(string id);
        void Add(T item);
        void Update(T item);
        void Remove(string id);
        void Save();
    }

    public class FileRepository<T> : IRepository<T> where T : class
    {
        private readonly string _path;
        private readonly Func<T, string> _idSelector;
        private Document<T> _doc;

        public FileRepository(string path, Func<T, string> idSelector)
        {
            _path = path;
            _idSelector = idSelector;
            Load();
        }

        private void Load()
        {
            try
            {
                if (File.Exists(_path))
                {
                    var json = File.ReadAllText(_path);
                    _doc = JsonUtility.FromJson<Document<T>>(json) ?? new Document<T>();
                }
                else
                {
                    _doc = new Document<T>();
                    Save(); // создаём пустой файл, чтобы структура БД была видна
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[YAV][DB] Не удалось загрузить {_path}: {e.Message}. Начинаем с чистой таблицы.");
                _doc = new Document<T>();
            }
        }

        public IReadOnlyList<T> GetAll() => _doc.Items;

        public T GetById(string id)
        {
            foreach (var item in _doc.Items)
                if (_idSelector(item) == id) return item;
            return null;
        }

        public void Add(T item)
        {
            _doc.Items.Add(item);
            Save();
        }

        public void Update(T item)
        {
            var id = _idSelector(item);
            for (int i = 0; i < _doc.Items.Count; i++)
            {
                if (_idSelector(_doc.Items[i]) == id)
                {
                    _doc.Items[i] = item;
                    Save();
                    return;
                }
            }
            Add(item);
        }

        public void Remove(string id)
        {
            var existing = GetById(id);
            if (existing != null)
            {
                _doc.Items.Remove(existing);
                Save();
            }
        }

        public void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(_path);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(_path, JsonUtility.ToJson(_doc, true));
            }
            catch (Exception e)
            {
                Debug.LogError($"[YAV][DB] Ошибка записи {_path}: {e.Message}");
            }
        }
    }

    /// <summary>Таблица players.json.</summary>
    public class FilePlayerRepository
    {
        private readonly FileRepository<PlayerRecord> _repo;
        public FilePlayerRepository(string path)
        {
            _repo = new FileRepository<PlayerRecord>(path, r => r.Id);
        }

        /// <summary>Пока игра локальная — один профиль по умолчанию. При появлении авторизации метод уйдёт.</summary>
        public PlayerRecord GetOrCreateDefault(string nickname)
        {
            var items = _repo.GetAll();
            if (items.Count > 0) return items[0];

            var player = new PlayerRecord { Nickname = string.IsNullOrWhiteSpace(nickname) ? "Гость" : nickname };
            _repo.Add(player);
            return player;
        }
    }

    /// <summary>Таблица characters.json.</summary>
    public class FileCharacterRepository
    {
        private readonly FileRepository<CharacterRecord> _repo;
        public FileCharacterRepository(string path)
        {
            _repo = new FileRepository<CharacterRecord>(path, r => r.Id);
        }

        public IReadOnlyList<CharacterRecord> GetAll() => _repo.GetAll();
        public CharacterRecord GetById(string id) => _repo.GetById(id);

        public CharacterRecord Create(string ownerId, CharacterClassId classId, string nickname)
        {
            var rec = CharacterRecord.Create(ownerId, classId, nickname);
            _repo.Add(rec);
            return rec;
        }

        public void SaveState(CharacterRecord rec) => _repo.Update(rec);
        public void Delete(string id) => _repo.Remove(id);
    }
}
