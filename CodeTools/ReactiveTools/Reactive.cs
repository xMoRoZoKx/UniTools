using System;
using System.Collections.Generic;
using System.Linq;
using UniTools.PlayerPrefs;
using Unity.VisualScripting;
using UnityEngine;

namespace UniTools.Reactive
{
    [System.Serializable]
    public class Reactive<T> : IReactive<T>
    {
        public Reactive()
        {
            _value = default;
        }
        public Reactive(T value)
        {
            _value = value;
        }
        [SerializeField] protected T _value;
        protected int lastSetedHash;
        [NonSerialized] protected EventStream<(T, T)> _eventStream;
        protected EventStream<(T, T)> eventStream => _eventStream ??= new EventStream<(T, T)>();
        private T prevValue;
        public virtual T value
        {
            get
            {
                return _value;
            }
            set
            {
                if ((value != null && _value == null) || (value == null && _value != null) || (value != null && !value.Equals(_value)))//.GetHashCode() != _value?.GetHashCode())
                {
                    prevValue = _value;
                    _value = value;
                    InvokeEvents();
                    lastSetedHash = _value != null ? _value.GetHashCode() : 0;
                }
            }
        }
        public IDisposable SubscribeAndInvoke(Action<T> onChangedEvent)
        {
            onChangedEvent.Invoke(value);
            return Subscribe(onChangedEvent);
        }
        public virtual IDisposable Subscribe(Action<T> onChangedEvent)
        {
            return eventStream.Subscribe(val =>
            {
                onChangedEvent(val.Item2);
            });
        }
        public IDisposable Buffer(Action<T, T> old_New) => eventStream.Subscribe(val =>
        {
            old_New(val.Item1, val.Item2);
        });
        public IDisposable Subscribe(Action onChangedEvent) => Subscribe(val => onChangedEvent?.Invoke());
        public void UnsubscribeAll()
        {
            eventStream.DisonnectAll();
        }

        public T GetValue() => value;

        public void SetValue(T value)
        {
            this.value = value;
        }

        public void InvokeEvents()
        {
            eventStream.Invoke((prevValue, value));
        }
    }
    public interface IReactive<T> : IReadOnlyReactive<T>
    {
        public void SetValue(T value);
    }

    public interface IReadOnlyReactive<T>
    {
        public T Value => GetValue();
        public T GetValue();
        public void InvokeEvents();
        public IDisposable SubscribeAndInvoke(Action<T> onChangedEvent);
        public IDisposable Subscribe(Action<T> onChangedEvent);
        public IDisposable Subscribe(Action onChangedEvent) => Subscribe(val => onChangedEvent?.Invoke());
        public void UnsubscribeAll();
    }
    public static class ReactiveUtils
    {
        //CONNECTED
        public static IDisposable SubscribeAndInvoke<T1, T2, T3>(this IReadOnlyReactive<T1> reactive1, IReadOnlyReactive<T2> reactive2, IReadOnlyReactive<T3> reactive3, Action<T1, T2, T3> onChangedEvent)
        {
            var connections = new Connections();
            connections += reactive1.SubscribeAndInvoke(val => Invoke());
            connections += reactive2.SubscribeAndInvoke(val => Invoke());
            connections += reactive3.SubscribeAndInvoke(val => Invoke());

            void Invoke()
            {
                onChangedEvent?.Invoke(reactive1.GetValue(), reactive2.GetValue(), reactive3.GetValue());
            }

            return connections;
        }
        public static IDisposable SubscribeAndInvoke<T1, T2>(this IReadOnlyReactive<T1> reactive1, IReadOnlyReactive<T2> reactive2, Action<T1, T2> onChangedEvent)
        {
            var connections = new Connections();
            connections += reactive1.SubscribeAndInvoke(val => Invoke());
            connections += reactive2.SubscribeAndInvoke(val => Invoke());

            void Invoke()
            {
                onChangedEvent?.Invoke(reactive1.GetValue(), reactive2.GetValue());
            }

            return connections;
        }

        //TOOLS
        public static ReactiveFunc<T1, T2> Func<T1, T2>(this IReadOnlyReactive<T1> reactive, Func<T1, T2> func)
        {
            var result = new ReactiveFunc<T1, T2>
            {
                updater = reactive,
                func = func
            };

            return result;
        }
        //public static ReactiveFunc<T1, T1> SortReactive<T1>(this IReadOnlyReactiveList<T1> reactive, Comparison<T1> comparison) => reactive.Func(lst =>
        //{
        //    var result = lst.ToList();
        //    result.Sort(comparison);
        //    return result;
        //});
        //JSON UTILS
        public static string ToJson<T>(this IReactive<T> reactive)
        {
            return JsonUtility.ToJson(new ReactiveJsonValue<T>(reactive.GetValue()));
        }
        public static IReactive<T> FromJson<T>(this IReactive<T> reactive, string json)
        {
            if (reactive == null) reactive = new Reactive<T>();
            Debug.LogError(json);
            var fromJson = JsonUtility.FromJson<ReactiveJsonValue<T>>(json);
            if (fromJson != null) reactive.SetValue(fromJson.Value);
            return reactive;
        }
        [System.Serializable]
        private class ReactiveJsonValue<T>
        {
            public ReactiveJsonValue(T val)
            {
                Value = val;
            }
            public T Value;
        }
    }
}