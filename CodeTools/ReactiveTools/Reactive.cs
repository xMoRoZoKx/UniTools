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
        [NonSerialized] EventStream<(T, T)> _eventStream;
        EventStream<(T, T)> eventStream => _eventStream ??= new EventStream<(T, T)>();
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
                    var oldVal = _value;
                    _value = value;
                    eventStream.Invoke((oldVal, value));
                    lastSetedHash = _value != null ? _value.GetHashCode() : 0;
                }
            }
        }
        public IDisposable SubscribeAndInvoke(Action<T> onChangedEvent)
        {
            onChangedEvent.Invoke(_value);
            return Subscribe(onChangedEvent);
        }
        public IDisposable Subscribe(Action<T> onChangedEvent) => eventStream.Subscribe(val =>
        {
            onChangedEvent(val.Item2);
        });
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
    }
    public interface IReactive<T> : IReadOnlyReactive<T>
    {
        public void SetValue(T value);
    }
    
    public interface IReadOnlyReactive<T>
    {
        public T Value => GetValue();
        public T GetValue();
        public IDisposable SubscribeAndInvoke(Action<T> onChangedEvent);
        public IDisposable Subscribe(Action<T> onChangedEvent);
        public IDisposable Subscribe(Action onChangedEvent) => Subscribe(val => onChangedEvent?.Invoke());
        public void UnsubscribeAll();
    }
    public static class ReactiveUtils
    {
        //CONNECTED
        public static IDisposable SubscribeAndInvoke<T1, T2, T3>(this IReactive<T1> reactive1, IReactive<T2> reactive2, IReactive<T3> reactive3, Action<T1, T2, T3> onChangedEvent)
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
        public static IDisposable SubscribeAndInvoke<T1, T2>(this IReactive<T1> reactive1, IReactive<T2> reactive2, Action<T1, T2> onChangedEvent)
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