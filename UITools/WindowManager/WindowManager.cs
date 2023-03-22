using System.Collections.Generic;
using System.Linq;
using Tools;
using UnityEngine;

namespace Tools
{
    [RequireComponent(typeof(Canvas))]
    internal class WindowManager : MonoBehaviour
    {
        private static WindowManager _instance;
        public static WindowManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var managers = Resources.LoadAll<WindowManager>("");
                    if(managers.Count() == 0) 
                    {
                        Debug.LogError("Window manager not found");
                        return null;
                    }
                    _instance = Object.Instantiate(managers[0]);
                }
                return _instance;
            }
        }
        public Transform root;
        public Canvas canvas { get; private set; }
        private List<WindowBase> prefabs;
        private List<WindowBase> createdWindows = new List<WindowBase>();
        private List<WindowBase> shownWindows = new List<WindowBase>();
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                canvas = GetComponent<Canvas>();
                prefabs = Resources.LoadAll<WindowBase>("").ToList();
                DontDestroyOnLoad(this);
            }
            else Destroy(gameObject);
        }
        public T Show<T>() where T : WindowBase
        {
            var window = createdWindows.Find(w => w is T);
            var windowPrefab = prefabs.Find(w => w is T);
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
            window.active = true;
            shownWindows.Add(window);
            return (T)window;
        }
        private T CreateView<T>(T windowPrefab) where T : WindowBase
        {
            var window = Instantiate(windowPrefab, root);
            window.OnOpened();
            window.ShowAnimation();
            createdWindows.Add(window);
            shownWindows.Add(window);
            window.active = true;
            return (T)window;
        }
        public void Close(WindowBase closeWindow)
        {
            Debug.Log("close" + closeWindow?.name);
            var win = shownWindows.FindLast(w => w == closeWindow);
            if (!win) return;
            win.onClose.Invoke();
            win.onClose.RemoveAllListeners();
            win.active = false;
            TaskTools.Wait(win.CloseAnimation(), () =>
            {
                shownWindows.Remove(win);
                if (win.isReusableView) win.gameObject.SetActive(false);
                else
                {
                    createdWindows.Remove(win);
                    Destroy(win.gameObject);
                }
            });
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