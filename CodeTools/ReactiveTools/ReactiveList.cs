using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniTools.Reactive
{
    [System.Serializable]
    public enum CollectionEvent
    {
        Remove,
        Add,
        Replace
    }

    [System.Serializable]
    public class ReactiveList<T> : List<T>, IReactiveList<T>
    {
        [NonSerialized] EventStream<(T, CollectionEvent)> _eventsForEach;
        EventStream<(T, CollectionEvent)> eventsForEach => _eventsForEach ??= new EventStream<(T, CollectionEvent)>();
        [NonSerialized] EventStream<List<T>> _eventStream;
        EventStream<List<T>> eventStream => _eventStream ??= new EventStream<List<T>>();
        public int lastSetedHash = 0;
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
                    lastSetedHash = this.GetHashCode();
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
            eventsForEach.Invoke((value, type));
        }
        public new void Add(T item)
        {
            base.Add(item);
            InvokeElementEvents(item, CollectionEvent.Add);
            InvokeListEvents();
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            foreach (var element in collection)
            {
                Add(element);
            }
        }
        public new void Clear()
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                RemoveAt(0);
            }
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            InvokeElementEvents(base[index], CollectionEvent.Add);
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


        public IDisposable SubscribeForEach(Action<T, CollectionEvent> onChangeElement) => eventsForEach.Subscribe(value => onChangeElement.Invoke(value.Item1, value.Item2));
        public List<T> GetValue() => this.ToList();

        public void SetValue(List<T> value)
        {
            Clear();
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
    public interface IReactiveList<T> : IList<T>, IReadOnlyReactiveList<T>
    {
    }
    
    public interface IReadOnlyReactiveList<T> : IReadOnlyCollection<T>, IReadOnlyList<T>, IReactive<List<T>>
    {
        public IDisposable SubscribeForEach(Action<T, CollectionEvent> onChangeElement);
    }
}
