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
        public AutoSaver(string key, T value, string layer = PlayerPrefsPro.BASE_LAYER)
        {
            if (!PlayerPrefsPro.HasKey(key)) _value = value;
            this.key = key;
            this.ConnectToSaver(key, layer);
        }
        public void Save(string layer = PlayerPrefsPro.BASE_LAYER) => this.Save(key, layer);
    }
}