using System;
using System.Diagnostics;
using UniTools.PlayerPrefs;

namespace UniTools.Reactive
{
    [System.Serializable]
    public class AutoSaver<T> : Reactive<T>, IAutoSaver<T>
    {
        public string key { get; private set; }
        public string layer { get; private set; }
        public AutoSaver(string key, string layer = PlayerPrefsPro.BASE_LAYER)
        {
            this.key = key;
            this.ConnectToSaver(key, layer);
        }
        public AutoSaver(string key, T defaultValue, string layer = PlayerPrefsPro.BASE_LAYER)
        {
            if (!PlayerPrefsPro.HasKey(key, layer)) value = defaultValue;

            this.key = key;
            this.ConnectToSaver(key, layer);
        }
        public void Save() => this.Save(key, layer);
        public IDisposable OnDataUpdate(Action<string, T> onUpdate)
        {
            return SubscribeAndInvoke(value => onUpdate?.Invoke(key, value));
        }
    }
}