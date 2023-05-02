using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Tools
{
    public class WindowBase : MonoBehaviour
    {
        public Button closeButton;
        public Connections connections = new Connections();
        public bool isReusableView = true;
        [HideInInspector] public UnityEvent onClose = new UnityEvent();
        [HideInInspector] public bool active = false;
        protected virtual void Awake()
        {
            closeButton?.onClick.AddListener(Close);
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
        public virtual void OnOpened(){}
    }
}