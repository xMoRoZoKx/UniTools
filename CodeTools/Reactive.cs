using System;
using System.Collections;
using System.Collections.Generic;
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
        List<Action<T>> actionWithValue = new List<Action<T>>();
        List<Action> actionWithoutValue = new List<Action>();
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
                    actionWithoutValue.ForEach(action => action?.Invoke());
                    actionWithValue.ForEach(action => action?.Invoke(value));
                }
            }
        }
        public Action<T> OnChanged(Action<T> onChangedEvent)
        {
            actionWithValue.Add(onChangedEvent);
            return onChangedEvent;
        }
        public Action OnChanged(Action onChangedEvent)
        {
            actionWithoutValue.Add(onChangedEvent);
            return onChangedEvent;
        }
        public void DisconnectAll()
        {
            actionWithoutValue.Clear();
            actionWithValue.Clear();
        }
    }

    public static class ReactiveUtils
    {
        //SAVES UTILS
        public static void ConnectToSaverByKey<T>(this Reactive<T> reactive, string saveKey) where T : new()
        {
            reactive.value = reactive.GetSave(nameof(saveKey)).value;
            reactive.OnChanged(() => reactive.Save(nameof(saveKey)));
        }
        public static void Save<T>(this Reactive<T> reactive, string saveKey) where T : new()
        {
            PlayerPrefsPro.Set(saveKey, reactive.value);
        }
        public static Reactive<T> GetSave<T>(this Reactive<T> reactive, string saveKey) where T : new()
        { 
            if(reactive == null) return reactive;
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
            if(fromJson != null) reactive.value = fromJson.Value;
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