using System;
using System.Diagnostics;
using UniTools.PlayerPrefs;

namespace UniTools.Reactive
{
    [System.Serializable]
    public class AutoSaver<T> : Reactive<T>, IAutoSaver
    {
        public string key { get; private set; }
        public SaveLayer layer { get; private set; }
        public AutoSaver(string key, SaveLayer layer = SaveLayer.Default)
        {
            this.key = key;
            this.ConnectToSaver(key, layer);
        }
        public AutoSaver(string key, T defaultValue, SaveLayer layer = SaveLayer.Default)
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