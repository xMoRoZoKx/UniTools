using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.UI
{
    [RequireComponent(typeof(Canvas))]
    public class WindowManager : MonoBehaviour
    {
        public static WindowManager instance;
        public Transform root;
        public Canvas canvas { get; private set; }
        private List<WindowBase> windows;
        private List<WindowBase> shownWindows = new List<WindowBase>();
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                canvas = GetComponent<Canvas>();
                windows = Resources.LoadAll<WindowBase>("").ToList();
                DontDestroyOnLoad(this);
            }
            else Destroy(gameObject);
        }
        public T Show<T>() where T : WindowBase
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
                return ShowExistView((T)window);
            }
            return CreateView((T)windowPrefab);
        }
        private T ShowExistView<T>(T window) where T : WindowBase
        {
            GetOpenedWindow<T>()?.Close();
            window.gameObject.SetActive(true);
            window.transform.SetSiblingIndex(root.childCount - 1);
            window.OnOpened();
            window.ShowAnimation();
            return (T)window;
        }
        private T CreateView<T>(T windowPrefab) where T : WindowBase
        {
            var window = Instantiate(windowPrefab, root);
            // window.gameObject.SetActive(true);
            window.OnOpened();
            window.ShowAnimation();
            shownWindows.Add(window);
            return (T)window;
        }
        public void Close(WindowBase closeWindow)
        {
            var win = shownWindows.Find(w => w == closeWindow);
            if (!win) return;
            win.CloseAnimation(onCompleted: () =>
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