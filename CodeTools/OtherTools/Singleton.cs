using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.CodeTools
{
    public class Singleton<T> : ISingleton<T> where T : Singleton<T>, new()
    {
        public static T Instance => ISingleton<T>.Instance;
    }
    
    public interface ISingleton<T> where T : ISingleton<T>, new()
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
    }
    public class SingletonBehavior<T> : MonoBehaviour where T : SingletonBehavior<T>, new()
    {
        private static T _instance;
        public static T Instance => _instance;
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = (T)this;
                OnCreateInstance();
            }
            else Destroy(this);
        }
        protected virtual void OnCreateInstance(){}
    }
}
