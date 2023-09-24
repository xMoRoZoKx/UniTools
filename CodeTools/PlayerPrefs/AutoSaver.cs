using System.Diagnostics;
using Tools.PlayerPrefs;

namespace Tools.Reactive
{
    public class AutoSaver<T> : Reactive<T>
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
    }
}