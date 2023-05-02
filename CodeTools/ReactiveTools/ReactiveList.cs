using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tools.Reactive
{
    [System.Serializable]
    public enum CollectionEvent
    {
        Remove,
        Add,
        Replace
    }
    public class ReactiveList<T> : List<T>, IReactiveCollection<T>, IReactive<List<T>>
    {
        private EventStream<(T, CollectionEvent)> _eventsForEach = new EventStream<(T, CollectionEvent)>();
        EventStream<List<T>> eventStream = new EventStream<List<T>>();

        public new T this[int index]
        {
            get => base[index];
            set
            {
                if (value.GetHashCode() != base[index].GetHashCode())
                {
                    base[index] = value;
                    InvokeElementEvents(value, CollectionEvent.Add);
                    InvokeListEvents();
                }
            }
        }
        private void InvokeListEvents()
        {
            // actionsAndKeys.ForEach(actAndKey => actAndKey.Item1.Invoke(this));
            eventStream.Invoke(this);
        }
        private void InvokeElementEvents(T value, CollectionEvent type)
        {
            _eventsForEach.Invoke((value, type));
        }
        public new void Add(T item)
        {
            base.Add(item);
            InvokeElementEvents(item, CollectionEvent.Add);
            InvokeListEvents();
        }
        public new void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                RemoveAt(0);
                InvokeElementEvents(base[0], CollectionEvent.Remove);
                InvokeListEvents();
            }
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            InvokeElementEvents(base[index], CollectionEvent.Replace);
            InvokeListEvents();
        }

        public new bool Remove(T item)
        {
            if (base.Remove(item))
            {
                InvokeElementEvents(item, CollectionEvent.Remove);
                InvokeListEvents();
                return true;
            }
            return false;
        }

        public new void RemoveAt(int index)
        {
            InvokeElementEvents(base[index], CollectionEvent.Remove);
            InvokeListEvents();
            base.RemoveAt(index);
        }


        public IDisposable SubscribeForEach(Action<T, CollectionEvent> onChangeElement) => _eventsForEach.Subscribe(value => onChangeElement.Invoke(value.Item1, value.Item2));
        public List<T> GetValue() => this;

        public void SetValue(List<T> value)
        {
            this.Clear();
            value?.ForEach(v => Add(v));
        }

        public IDisposable SubscribeAndInvoke(Action<List<T>> onChangedEvent)
        {
            onChangedEvent.Invoke(this);
            return Subscribe(onChangedEvent);
        }
        public void UnsubscribeAll() => eventStream.DisonnectAll();
        public IDisposable Subscribe(Action<List<T>> onChangedEvent) => eventStream.Subscribe(onChangedEvent); //SubscribeWithKey(onChangedEvent, onChangedEvent.GetHashCode().ToString());
        public IDisposable Subscribe(Action onChangedEvent) => Subscribe(val => onChangedEvent?.Invoke());
    }
    public interface IReactiveCollection<T> : ICollection<T>, IEnumerable<T>, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>
    {
        public IDisposable SubscribeForEach(Action<T, CollectionEvent> onChangeElement);
    }
    public class AutoSaverList<T> : ReactiveList<T>
    {
        public AutoSaverList(string key)
        {
            this.ConnectToSaver(key);
        }
    }
}
