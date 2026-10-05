using System;

namespace Yav.Core
{
    /// <summary>
    /// Типобезопасные события игры (замена строковым SendMessage-рассылкам).
    /// Используются для связи UI <-> логика без прямых ссылок.
    /// </summary>
    /// <summary>Событие без параметров.</summary>
    public class GameEvent
    {
        private event Action Handlers = null;

        public void Subscribe(Action handler) => Handlers += handler;
        public void Unsubscribe(Action handler) => Handlers -= handler;
        public void Raise() => Handlers?.Invoke();
        public void Clear() => Handlers = null;
    }

    /// <summary>Событие с одним параметром, например GameEvent&lt;CharacterClassId&gt;.</summary>
    public class GameEvent<T>
    {
        private event Action<T> Handlers = null;

        public void Subscribe(Action<T> handler) => Handlers += handler;
        public void Unsubscribe(Action<T> handler) => Handlers -= handler;
        public void Raise(T arg) => Handlers?.Invoke(arg);
        public void Clear() => Handlers = null;
    }
}
