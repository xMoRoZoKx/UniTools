using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tools.Reactive
{
    [System.Serializable]
    public enum ReactiveCollectionEventType
    {
        Remove,
        Add,
        Replace
    }
    public class ReactiveList<T> : List<T>, IReactiveCollection<T>, IReactive<List<T>>
    {
        // private List<T> _list = new List<T>();
        private EventController<(T, ReactiveCollectionEventType)> _eventsForEach = new EventController<(T, ReactiveCollectionEventType)>();
        List<(Action<List<T>>, string)> actionsAndKeys = new List<(Action<List<T>>, string)>();

        public new T this[int index]
        {
            get => base[index];
            set
            {
                if (value.GetHashCode() != base[index].GetHashCode())
                {
                    base[index] = value;
                    InvokeElementEvents(value, ReactiveCollectionEventType.Add);
                    InvokeListEvents();
                }
            }
        }
        private void InvokeListEvents()
        {
            actionsAndKeys.ForEach(actAndKey => actAndKey.Item1.Invoke(this));
        }
        private void InvokeElementEvents(T value, ReactiveCollectionEventType type)
        {
            _eventsForEach.Invoke((value, type));
        }
        public new void Add(T item)
        {
            base.Add(item);
            InvokeElementEvents(item, ReactiveCollectionEventType.Add);
            InvokeListEvents();
        }
        public new void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                RemoveAt(0);
                InvokeElementEvents(base[0], ReactiveCollectionEventType.Remove);
                InvokeListEvents();
            }
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            InvokeElementEvents(base[index], ReactiveCollectionEventType.Replace);
            InvokeListEvents();
        }

        public new bool Remove(T item)
        {
            if (base.Remove(item))
            {
                InvokeElementEvents(item, ReactiveCollectionEventType.Remove);
                InvokeListEvents();
                return true;
            }
            return false;
        }

        public new void RemoveAt(int index)
        {
            InvokeElementEvents(base[index], ReactiveCollectionEventType.Remove);
            InvokeListEvents();
            base.RemoveAt(index);
        }


        public void SubscribeForEach(Action<T, ReactiveCollectionEventType> onChangeElement)
        {
            _eventsForEach.Subscribe(value => onChangeElement.Invoke(value.Item1, value.Item2));
        }

        public List<T> GetValue() => this;

        public void SetValue(List<T> value)
        {
            this.Clear();
            value?.ForEach(v => Add(v));
        }

        public void SubscribeAndInvoke(Action<List<T>> onChangedEvent)
        {
            onChangedEvent.Invoke(this);
            this.Subscribe(onChangedEvent);
        }
        public void SubscribeWithKey(Action<List<T>> onChangedEvent, string key)
        {
            if (actionsAndKeys.Any(act => act.Item2 == key))
            {
                Debug.LogError("this key exist!");
                return;
            }
            actionsAndKeys.Add((onChangedEvent, key));
        }

        public void Unsubscribe(string key) => actionsAndKeys.RemoveAll(act => act.Item2 == key);
        public void UnsubscribeAll() => actionsAndKeys.Clear();
        public void Subscribe(Action<List<T>> onChangedEvent) => SubscribeWithKey(onChangedEvent, onChangedEvent.GetHashCode().ToString());
        public void Subscribe(Action onChangedEvent) => Subscribe(val => onChangedEvent?.Invoke());
    }
    public interface IReactiveCollection<T> : ICollection<T>, IEnumerable<T>, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>
    {
        public void SubscribeForEach(Action<T, ReactiveCollectionEventType> onChangeElement);
    }
    public class AutoSaverList<T> : ReactiveList<T>
    {
        public AutoSaverList(string key)
        {
            this.ConnectToSaver(key);
        }
    }
}
