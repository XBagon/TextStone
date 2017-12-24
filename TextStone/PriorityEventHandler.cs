using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;

namespace TextStone
{
    public class PriorityEventHandler<TEventArgs> where TEventArgs : EventArgs
    {
        private readonly SimplePriorityQueue<KeyValuePair<Object,Action<Object,TEventArgs>>> subscribers = new SimplePriorityQueue<KeyValuePair<Object,Action<Object,TEventArgs>>>();

        public void Subscribe(Object subscriber, sbyte priority, Action<Object,TEventArgs> a)
        {
            subscribers.Enqueue(new KeyValuePair<Object, Action<Object, TEventArgs>>(subscriber,a), priority);
        }

        public void Unsubscribe(Object subscriber, Action<Object,TEventArgs> a)
        {
            subscribers.Remove(subscribers.First(x => x.Key.Equals(subscriber) && x.Value.Equals(a)));
        }

        public void Call(TEventArgs args)
        {
            foreach (var subscriber in subscribers)
            {
                subscriber.Value(subscriber.Key,args);
            }
        }

    }
}
