using Tools.PlayerPrefs;

namespace Tools.Reactive
{
    public class AutoSaverList<T> : ReactiveList<T>
    {
        public string key { get; private set; }
        public AutoSaverList(string key, string layer = PlayerPrefsPro.BASE_LAYER)
        {
            this.key = key;
            this.ConnectToSaver(key, layer);
        }
        public void Save(string layer = PlayerPrefsPro.BASE_LAYER) => this.Save(key, layer);
    }
}
