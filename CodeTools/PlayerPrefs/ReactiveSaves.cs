using System;
using System.Collections;
using System.Collections.Generic;
using UniTools.PlayerPrefs;
using UnityEngine;

namespace UniTools.Reactive
{
    public class Data
    {
        public string key;
        public object obj;
        public Type type;
    }

    public static class ReactiveSaves
    {
        private static EventStream<Data> _onDataUpdate = new EventStream<Data>();
        public static IDisposable OnDataUpdate(Action<Data> onDataUpdate)
        {
            return _onDataUpdate.Subscribe(onDataUpdate);
        }
        public static void ConnectToSaver<T>(this IReactive<T> reactive, string saveKey, SaveLayer layer = SaveLayer.Default)
        {
            reactive.GetSave(saveKey, layer);
            reactive.SubscribeAndInvoke(value => reactive.Save(saveKey, layer));
        }
        public static void Save<T>(this IReactive<T> reactive, string saveKey, SaveLayer layer = SaveLayer.Default)
        {
            var val = reactive.GetValue();
            _onDataUpdate.Invoke(new Data()
            {
                key = saveKey,
                obj = reactive.Value,
                type = typeof(T)
            });
            PlayerPrefsPro.Set(saveKey, reactive.GetValue(), layer);
        }
        public static IReactive<T> GetSave<T>(this IReactive<T> reactive, string saveKey, SaveLayer layer = SaveLayer.Default)
        {
            if (reactive == null) return reactive;
            reactive.SetValue(PlayerPrefsPro.HasKey(saveKey, layer) ? PlayerPrefsPro.Get<T>(saveKey, layer) : reactive.GetValue());
            return reactive;
        }
    }
}