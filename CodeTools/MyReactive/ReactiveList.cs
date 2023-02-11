using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
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
        private List<T> _list = new List<T>();
        private EventController<(T, ReactiveCollectionEventType)> _events = new EventController<(T, ReactiveCollectionEventType)>();

        public new T this[int index]
        {
            get => _list[index];
            set
            {
                if (value.GetHashCode() != _list[index].GetHashCode())
                {
                    _events.Invoke((value, ReactiveCollectionEventType.Replace));
                    _list[index] = value;
                }
            }
        }

        public new int Count => _list.Count;

        public bool IsReadOnly => false;
        public bool IsSynchronized => false;

        public bool IsFixedSize => false;

        public void Subscribe(Action<T, ReactiveCollectionEventType> onChangeElement)
        {
            _events.Subscribe(value => onChangeElement.Invoke(value.Item1, value.Item2));
        }
        public new void Add(T item)
        {
            _events.Invoke((item, ReactiveCollectionEventType.Add));
            _list.Add(item);
        }
        public new void Clear()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                _events.Invoke((_list[0], ReactiveCollectionEventType.Remove));
                RemoveAt(0);
            }
        }

        public new bool Contains(T item) => _list.Contains(item);

        public new void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public new IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        public new int IndexOf(T item) => _list.IndexOf(item);

        public new void Insert(int index, T item)
        {
            _events.Invoke((item, ReactiveCollectionEventType.Replace));
            _list.Insert(index, item);
        }

        public new bool Remove(T item)
        {
            if (_list.Remove(item))
            {
                _events.Invoke((item, ReactiveCollectionEventType.Remove));
                return true;
            }
            return false;
        }

        public new void RemoveAt(int index)
        {
            _events.Invoke((_list[index], ReactiveCollectionEventType.Remove));
            _list.RemoveAt(index);
        }


        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
    public interface IReactiveCollection<T> : ICollection<T>, IEnumerable<T>, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>
    {
        public void Subscribe(Action<T, ReactiveCollectionEventType> onChangeElement);
    }
}