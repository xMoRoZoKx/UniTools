using System;
using System.Collections.Generic;
using UniTools.PlayerPrefs;

namespace UniTools.Reactive
{
    [System.Serializable]
    public class AutoSaverList<T> : ReactiveList<T>, IAutoSaver
    {
        public string key { get; private set; }
        public SaveLayer layer { get; private set; }
        public AutoSaverList(string key, SaveLayer layer = SaveLayer.Default)
        {
            this.key = key;
            this.layer = layer;

            this.ConnectToSaver(key, layer);
        }

        public void Save()
        {
            this.Save(key, layer);
            InvokeEvents();
        }

        public IDisposable OnDataUpdate(Action<string, List<T>> onUpdate)
        {
            return SubscribeAndInvoke(value => onUpdate?.Invoke(key, value));
        }
    }
}
