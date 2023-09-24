using System;
using System.Collections.Generic;
using System.Linq;
using Tools.PlayerPrefs;
using Unity.VisualScripting;
using UnityEngine;

namespace Tools.Reactive
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
        protected T _value;
        protected int lastSetedHash;
        EventStream<(T, T)> eventStream = new EventStream<(T, T)>();
        public virtual T value
        {
            get
            {
                return _value;
            }
            set
            {
                if ((value != null && _value == null) || (value != null && !value.Equals(_value)))//.GetHashCode() != _value?.GetHashCode())
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

        public bool HasChanges()
        {
            return lastSetedHash != _value.GetHashCode();
        }
    }
    public interface IReactive<T>
    {
        public T GetValue();
        public void SetValue(T value);
        public IDisposable SubscribeAndInvoke(Action<T> onChangedEvent);
        public IDisposable Subscribe(Action<T> onChangedEvent);
        public IDisposable Subscribe(Action onChangedEvent) => Subscribe(val => onChangedEvent?.Invoke());
        public void UnsubscribeAll();
        public bool HasChanges();
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
        //SAVES UTILS
        // private static async void SaveEachSeconds<T>(this IReactive<T> reactive, string key, float timeOut = 1)// IN TESTING
        // {
        //     while (reactive != null)
        //     {
        //         if(reactive.HasChanges()) reactive.Save(key);
        //         await TaskTools.WaitForSeconds(timeOut);
        //     }
        // }
        public static void ConnectToSaver<T>(this IReactive<T> reactive, string saveKey, string layer = PlayerPrefsPro.BASE_LAYER)
        {
            reactive.GetSave(saveKey, layer);
            reactive.SubscribeAndInvoke(value => reactive.Save(saveKey, layer));
            // reactive.SaveEachSeconds(saveKey); // IN TESTING
        }
        public static void Save<T>(this IReactive<T> reactive, string saveKey, string layer = PlayerPrefsPro.BASE_LAYER)
        {
            var val = reactive.GetValue();
            PlayerPrefsPro.Set(saveKey, reactive.GetValue(), layer);
        }
        public static IReactive<T> GetSave<T>(this IReactive<T> reactive, string saveKey, string layer = PlayerPrefsPro.BASE_LAYER)
        {
            if (reactive == null) return reactive;
            reactive.SetValue(PlayerPrefsPro.Get<T>(saveKey, layer));
            return reactive;
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