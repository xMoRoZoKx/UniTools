using UnityEngine;
namespace Tools
{
    public class Singleton<T> : ISingleton<T> where T : Singleton<T>, new()
    {
        public static T Instance => ISingleton<T>.Instance;
    }

    public interface ISingleton<T> where T : ISingleton<T>, new()
    {
        private static T _instance;
        public static T Instance => _instance ??= new T();
    }
    public class SingletonBehavior<T> : MonoBehaviour where T : SingletonBehavior<T>, new()
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if(_instance == null) 
                {
                    var prefab = Resources.LoadAll<T>("");
                    if(prefab.Length == 0) 
                    {
                        Debug.LogError("Prefab not exist in Resources folder");
                        return null;
                    }
                    _instance = UnityEngine.Object.Instantiate(prefab[0]);
                    _instance.OnCreateInstance();
                }
                return _instance;
            }
        }
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = (T)this;
                OnCreateInstance();
            }
            else Destroy(this);
        }
        protected virtual void OnCreateInstance() { }
    }
}
