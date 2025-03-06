using System.Dynamic;
using UnityEngine;
namespace UniTools
{
    public class Singleton<T> : ISingleton<T> where T : Singleton<T>, new()
    {
        public static T Instance => ISingleton<T>.Instance;
        public static T CreateInstance() => ISingleton<T>.CreateInstance();
    }

    public interface ISingleton<T> where T : ISingleton<T>, new()
    {
        private static T _instance;
        public static T Instance => _instance ??= new T();
        public static T CreateInstance() => _instance = _instance == null ? new T() : _instance;
    }
    public class SingletonBehavior<T> : MonoBehaviour where T : SingletonBehavior<T>, new()
    {
        protected static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var prefab = Resources.LoadAll<T>("");
                    if (prefab.Length == 0)
                    {
                        Debug.LogError("Prefab not exist in Resources folder");
                        return null;
                    }
                    _instance = Instantiate(prefab[0]);
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
