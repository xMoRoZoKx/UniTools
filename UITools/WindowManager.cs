using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.UI
{
    public class WindowManager : MonoBehaviour
    {
        public static WindowManager instance;
        public Transform root;
        // [SerializeField] private WindowsSO windowsSO;
        private List<WindowBase> windows;//windowsSO.windows;
        private List<WindowBase> shownWindows = new List<WindowBase>();
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                windows = Resources.LoadAll<WindowBase>("").ToList();
                DontDestroyOnLoad(this);
            }
            else Destroy(gameObject);
        }
        public T Show<T>(Action onClose = null) where T : WindowBase
        {
            var window = shownWindows.Find(w => w is T);
            var windowPrefab = windows.Find(w => w is T);
            if (!windowPrefab)
            {
                Debug.LogError("Window " + typeof(T).ToString() + " not found");
                return null;
            };
            if (window && windowPrefab.isReusableView)
            {
                GetOpenedWindow<T>()?.Close();
                window.gameObject.SetActive(true);
                window.transform.SetSiblingIndex(root.childCount - 1);
                window.OnOpened();
                window.animator.StartAnimation(false, () => window.gameObject.SetActive(true));
                return (T)window;
            }
            window = Instantiate(windowPrefab, root);
            window.OnOpened();
            window.onClose.AddListener(() => onClose?.Invoke());
            window.animator.StartAnimation(false, () => window.gameObject.SetActive(true));
            window.gameObject.SetActive(true);
            shownWindows.Add(window);
            return (T)window;
        }
        public void Close(WindowBase closeWindow)
        {
            var win = shownWindows.Find(w => w == closeWindow);
            if (!win) return;
            win.animator.StartAnimation(true, () =>
            {
                if (win.isReusableView) win.gameObject.SetActive(false);
                else
                {
                    shownWindows.Remove(win);
                    Destroy(win.gameObject);
                }
            });
            win.onClose.Invoke();
            win.onClose.RemoveAllListeners();
            win.active = false;
        }
        public void CloseTop() => Close(shownWindows.FindLast(w => w.active == true && w.closeButton != null));
        public void CloseAll() => shownWindows.ForEach(w => Close(w));
        public T GetOpenedWindow<T>() where T : WindowBase
        {
            T win = (T)shownWindows.Find(w => w is T);
            // if (win == null) win = Show<T>();
            return win;
        }
        void OnGUI()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                CloseTop();
            }
        }
    }
}