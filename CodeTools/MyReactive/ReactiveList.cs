using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Game.CodeTools
{
    [System.Serializable]
    public enum ReactiveCollectionEventType
    {
        Remove,
        Add,
        Replace
    }
    public class ReactiveList<T> : List<T>, IReactiveCollection<T>
    {
        // private List<T> _list = new List<T>();
        private EventController<(T, ReactiveCollectionEventType)> _eventsForEach = new EventController<(T, ReactiveCollectionEventType)>();
        List<(Action<List<T>>, string)> actionsAndKeys = new List<(Action<List<T>>, string)>();

        public new T this[int index]
        {
            get => this[index];
            set
            {
                if (value.GetHashCode() != base[index].GetHashCode())
                {
                    _eventsForEach.Invoke((value, ReactiveCollectionEventType.Replace));
                    base[index] = value;
                }
            }
        }
        public new void Add(T item)
        {
            _eventsForEach.Invoke((item, ReactiveCollectionEventType.Add));
            base.Add(item);
        }
        public new void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                _eventsForEach.Invoke((base[0], ReactiveCollectionEventType.Remove));
                RemoveAt(0);
            }
        }

        public new void Insert(int index, T item)
        {
            _eventsForEach.Invoke((item, ReactiveCollectionEventType.Replace));
            base.Insert(index, item);
        }

        public new bool Remove(T item)
        {
            if (base.Remove(item))
            {
                _eventsForEach.Invoke((item, ReactiveCollectionEventType.Remove));
                return true;
            }
            return false;
        }

        public new void RemoveAt(int index)
        {
            _eventsForEach.Invoke((base[index], ReactiveCollectionEventType.Remove));
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
    public interface IReactiveCollection<T> : IReactive<List<T>>, ICollection<T>, IEnumerable<T>, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>
    {
        public void SubscribeForEach(Action<T, ReactiveCollectionEventType> onChangeElement);
    }
}