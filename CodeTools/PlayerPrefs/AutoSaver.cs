using System;
using System.Diagnostics;
using UniTools.PlayerPrefs;

namespace UniTools.Reactive
{

    public enum SaveLayers
    {
        Default,
        Layer1,
        Layer2
    }
    [System.Serializable]
    public class AutoSaver<T> : Reactive<T>, IAutoSaver
    {
        public string key { get; private set; }
        public string layer { get; private set; }
        public AutoSaver(string key, SaveLayers saveLayer = SaveLayers.Default)
        {
            this.key = key;
            this.ConnectToSaver(key, layer);
        }
        public AutoSaver(string key, T defaultValue, SaveLayers saveLayer = SaveLayers.Default)
        {
            if (!PlayerPrefsPro.HasKey(key, layer)) value = defaultValue;

            this.key = key;
            this.ConnectToSaver(key, layer);
        }
        public void Save()
        {
            this.Save(key, layer);
            InvokeEvents();
        }
        public IDisposable OnDataUpdate(Action<string, T> onUpdate)
        {
            return Subscribe(value => onUpdate?.Invoke(key, value));
        }
    }
}