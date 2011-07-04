using System;
using System.Collections.Generic;

namespace EvilTemple.Runtime
{

    public delegate void MessageHandler<in T>(T message) where T : IMessage;

    public interface IMessage
    {
    }
    
    public static class EventBus
    {

        private static readonly IDictionary<Type, object> Handlers;

        private static readonly IDictionary<Type, IMessage> MessageSingletons;

        static EventBus()
        {
            Handlers = new Dictionary<Type, object>();
            MessageSingletons = new Dictionary<Type, IMessage>();
        }

        public static void Register<T>(MessageHandler<T> h) where T : IMessage
        {
            GetHandlers<T>().Add(h);
        }

        public static void Unregister<T>(MessageHandler<T> h) where T : IMessage
        {
            GetHandlers<T>().Remove(h);
        }

        public static void Send<T>(T m) where T : IMessage
        {
            object handlerBaseList;
            if (!Handlers.TryGetValue(typeof(T), out handlerBaseList)) return;

            var handlerList = (IList<MessageHandler<T>>)handlerBaseList;

            var handlerCopy = new MessageHandler<T>[handlerList.Count];
            handlerList.CopyTo(handlerCopy, 0);

            /*
             * We operate on a copy of the handler list here, since 
             * the actual handlers may register new handlers on the 
             * same type of message.
             */
            foreach (var handler in handlerCopy)
                handler(m);
        }

        private static IList<MessageHandler<T>> GetHandlers<T>() where T : IMessage
        {
            object handlerList;
            if (Handlers.TryGetValue(typeof(T), out handlerList))
                return (IList<MessageHandler<T>>)handlerList;

            // Lazily create a list
            handlerList = new List<MessageHandler<T>>();
            Handlers.Add(typeof(T), handlerList);
            return (IList<MessageHandler<T>>)handlerList;
        }

        public static void Send<T>() where T : IMessage
        {
            IMessage singletonObj;
            if (!MessageSingletons.TryGetValue(typeof(T), out singletonObj))
            {
                singletonObj = (IMessage)typeof(T).GetConstructor(new Type[0]).Invoke(new object[0]);
                MessageSingletons.Add(typeof(T), singletonObj);
            }

            Send((T)singletonObj);
        }

    }
}
