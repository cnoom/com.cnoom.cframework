using System;

namespace CnoomFrameWork.Base.Events
{
    public class EventManager
    {
        private static EventHandler _handler = new();
        private static RefEventHandlers _refHandler = new();

        public static void Subscribe<T>(Action<T> handler, int priority = 0, bool once = false)
        {
            _handler.Subscribe(handler, priority, once);
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            _handler.Unsubscribe(handler);
        }

        public static void Publish<T>(T e)
        {
            _handler.Publish(e);
        }

        public static void SubscribeRef<T>(RefEventHandlers.RefEvent<T> handler, int priority = 0,
            bool once = false)
            where T : struct
        {
            _refHandler.Subscribe(handler, priority, once);
        }

        public static void UnSubscribeRef<T>(RefEventHandlers.RefEvent<T> handler) where T : struct
        {
            _refHandler.UnSubscribe(handler);
        }

        public static void RefPublish<T>(ref T e) where T : struct
        {
            _refHandler.Publish(ref e);
        }

        public static void AddFilter<T>(Func<T, Delegate, bool> filter)
        {
            _handler.AddFilter(filter);
        }

        public static void AddRefFilter<T>(Func<T, Delegate, bool> filter) where T : struct
        {
            _refHandler.AddFilter(filter);
        }

        public static void Register(object subscriber)
        {
            _handler.Register(subscriber);
            _refHandler.Register(subscriber);
        }

        public static void Unregister(object subscriber)
        {
            _handler.Unregister(subscriber);
            _refHandler.Unregister(subscriber);
        }
    }
}