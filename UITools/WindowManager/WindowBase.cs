using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Tools
{
    public class WindowBase : MonoBehaviour
    {
        [field: SerializeField] public Button closeButton { get; private set; }
        public Connections connections = new Connections();
        [field: SerializeField] public bool isReusableView { get; private set; } = true;
        [field: SerializeField] public bool needHideThenWindowIsNotTop { get; private set; } = false;
        [field: SerializeField] public bool isPriorityWindow  { get; private set; } = false;
        public EventStream onClose { get; private set; } = new EventStream();
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
        public virtual void OnOpened() { }
    }
}