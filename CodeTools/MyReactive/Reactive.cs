using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tools.PlayerPrefs;
using UnityEngine;
namespace Game.CodeTools
{
    [System.Serializable]
    public class Reactive<T>
    {
        public Reactive()
        {
            _value = default;
        }
        public Reactive(T value)
        {
            _value = value;
        }
        T _value;
        List<(Action<T>, string)> actionsAndKeys = new List<(Action<T>, string)>();
        public T value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value.GetHashCode() != _value.GetHashCode())
                {
                    _value = value;
                    actionsAndKeys.ForEach(actionAndKey => actionAndKey.Item1?.Invoke(value));
                }
            }
        }
        public void SubscribeAndInvoke(Action<T> onChangedEvent)
        {
            onChangedEvent.Invoke(_value);
            Subscribe(onChangedEvent);
        }
        public void SubscribeWithKey(Action<T> onChangedEvent, string key)
        {
            if (actionsAndKeys.Any(act => act.Item2 == key))
            {
                Debug.LogError("this key exist!");
                return;
            }
            actionsAndKeys.Add((onChangedEvent, key));
        }
        public void Subscribe(Action<T> onChangedEvent) => SubscribeWithKey(onChangedEvent, onChangedEvent.GetHashCode().ToString());
        public void Subscribe(Action onChangedEvent) => Subscribe(val => onChangedEvent?.Invoke());
        public void Unsubscribe(string key)
        {
            actionsAndKeys.RemoveAll(act => act.Item2 == key);
        }
        public void UnsubscribeAll()
        {
            actionsAndKeys.Clear();
        }
    }

    public static class ReactiveUtils
    {
        //SAVES UTILS
        public static void ConnectToSaver<T>(this Reactive<T> reactive, string saveKey) where T : new()
        {
            reactive.value = reactive.GetSave(saveKey).value;
            reactive.SubscribeWithKey(value => reactive.Save(saveKey), saveKey);
        }
        public static void DisonnectSaver<T>(this Reactive<T> reactive, string saveKey) where T : new()
        {
            reactive.Unsubscribe(saveKey);
        }
        public static void Save<T>(this Reactive<T> reactive, string saveKey) where T : new()
        {
            PlayerPrefsPro.Set(saveKey, reactive.value);
        }
        public static Reactive<T> GetSave<T>(this Reactive<T> reactive, string saveKey) where T : new()
        {
            if (reactive == null) return reactive;
            reactive.value = PlayerPrefsPro.Get<T>(saveKey);
            return reactive;
        }
        //JSON UTILS
        public static string ToJson<T>(this Reactive<T> reactive)
        {
            return JsonUtility.ToJson(new ReactiveJsonValue<T>(reactive.value));
        }
        public static Reactive<T> FromJson<T>(this Reactive<T> reactive, string json)
        {
            if (reactive == null) reactive = new Reactive<T>();
            Debug.LogError(json);
            var fromJson = JsonUtility.FromJson<ReactiveJsonValue<T>>(json);
            if (fromJson != null) reactive.value = fromJson.Value;
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