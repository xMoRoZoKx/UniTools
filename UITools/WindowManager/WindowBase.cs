using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UniTools
{
    public class WindowBase : ConnectableMonoBehaviour
    {
        [field: SerializeField, Header("Optional")] public Button closeButton { get; private set; }
        [field: SerializeField, Space] public bool isReusableView { get; private set; } = true;
        [field: SerializeField] public bool needHideThenWindowIsNotTop { get; private set; } = false;
        [field: SerializeField] public int orderBy { get; private set; } = 0;
        public UnityEvent onClose { get; private set; } = new UnityEvent();
        public Action onCloseAction;
        [HideInInspector] public bool active = false;
        public virtual bool canShow => true; 
        protected virtual void Awake()
        {
            closeButton?.OnClickWithSound(Close);
        }
        public void Close()
        {
            WindowManager.Instance.Close(this);
        }
        public virtual float ShowAnimation()
        {
            return 0;
        }
        public virtual float CloseAnimation()
        {
            return 0;
        }

        public virtual void OnOpened() { }
        public virtual void OnClosed() { }
        public virtual void OnTop() { }
        public virtual void OnBottom() { }
    }
}