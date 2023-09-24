using Tools.PlayerPrefs;

namespace Tools.Reactive
{
    public class AutoSaver<T> : Reactive<T>
    {
        public string key { get; private set; }
        public AutoSaver(string key, string layer = PlayerPrefsPro.BASE_LAYER)
        {
            this.key = key;
            this.ConnectToSaver(key, layer);
        }
        public AutoSaver(string key, T defaultValue, string layer = PlayerPrefsPro.BASE_LAYER)
        {
            this.key = key;
            this.ConnectToSaver(key, layer);
            if (!PlayerPrefsPro.HasKey(key, layer)) value = defaultValue; 
        }
        public void Save(string layer = PlayerPrefsPro.BASE_LAYER) => this.Save(key, layer);
    }
}