using System;
using System.Collections.Generic;
using UniTools.PlayerPrefs;

namespace UniTools.Reactive
{
    [System.Serializable]
    public class AutoSaverList<T> : ReactiveList<T>, IAutoSaver<T>
    {
        public string key { get; private set; }
        public string layer { get; private set; }
        public AutoSaverList(string key, string layer = PlayerPrefsPro.BASE_LAYER)
        {
            this.key = key;
            this.layer = layer;

            this.ConnectToSaver(key, layer);
        }

        public void Save() => this.Save(key, layer);

        public IDisposable OnDataUpdate(Action<string, List<T>> onUpdate)
        {
            return SubscribeAndInvoke(value => onUpdate?.Invoke(key, value));
        }
    }
}
