using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.CodeTools
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        protected static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                        ((Singleton<T>)_instance).OnCreateInstance()?.Invoke();
                    }
                    return _instance;
                }
                return _instance;
            }
            private set { }
        }
        public virtual Action OnCreateInstance() => null;
    }
    public class SingletonBehavior<T> : MonoBehaviour where T : SingletonBehavior<T>, new()
    {
        private static T _instance;
        public static T Instance => _instance;
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = (T)this;
                _instance.OnCreateInstance(_instance).Invoke();
                DontDestroyOnLoad(_instance);
            }
            else Destroy(this);
        }
        public virtual Action OnCreateInstance(T instance) => null;
    }
    // public interface ISingleton<T> where T : ISingleton<T>, new()
    // {
    //     private static T _instance;
    //     public static T Instance
    //     {
    //         get
    //         {
    //             if (_instance == null)
    //             {
    //                 if (_instance == null)
    //                 {
    //                     _instance = new T();
    //                     ((ISingleton<T>)_instance).OnCreateInstance(_instance)?.Invoke();
    //                 }
    //                 return _instance;
    //             }
    //             return _instance;
    //         }
    //     }
    //     public virtual Action OnCreateInstance(T instance) => null;
    // }
}
