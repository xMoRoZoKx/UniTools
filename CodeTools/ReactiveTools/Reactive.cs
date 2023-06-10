using System;
using System.Collections.Generic;
using System.Linq;
using Tools.PlayerPrefs;
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
        EventStream<T> eventStream = new EventStream<T>();
        public T value
        {
            get
            {
                return _value;
            }
            set
            {

                if ((value != null && _value == null) || value?.GetHashCode() != _value?.GetHashCode())
                {
                    eventStream.Invoke(value);
                    _value = value;
                }
            }
        }
        public IDisposable SubscribeAndInvoke(Action<T> onChangedEvent)
        {
            onChangedEvent.Invoke(_value);
            return Subscribe(onChangedEvent);
        }
        public IDisposable Subscribe(Action<T> onChangedEvent) => eventStream.Subscribe(onChangedEvent);
        public IDisposable Buffer(Action<T, T> old_New) => eventStream.Subscribe(newVal =>
        {
            old_New.Invoke(_value, newVal);
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
    public interface IReactive<T>
    {
        public T GetValue();
        public void SetValue(T value);
        public IDisposable SubscribeAndInvoke(Action<T> onChangedEvent);
        public IDisposable Subscribe(Action<T> onChangedEvent);
        public IDisposable Subscribe(Action onChangedEvent) => Subscribe(val => onChangedEvent?.Invoke());
        public void UnsubscribeAll();
    }
    public static class ReactiveUtils
    {
        //SAVES UTILS
        public static void ConnectToSaver<T>(this IReactive<T> reactive, string saveKey, string layer = PlayerPrefsPro.BASE_LAYER)
        {
            reactive.GetSave(saveKey, layer);
            reactive.SubscribeAndInvoke(value => reactive.Save(saveKey, layer));
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