using System.Collections.Generic;
using System.Linq;
using Tools;
using UnityEngine;
// using UnityEngine.EventSystems;

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
                    if (managers.Count() == 0)
                    {
                        Debug.LogError("Window manager not found");
                        return null;
                    }
                    _instance = Object.Instantiate(managers[0]);
                }
                return _instance;
            }
        }
        [SerializeField] private Transform root;
        [SerializeField] private bool useDontDestroyOnLoad = true;
        public Canvas canvas { get; private set; }
        private List<WindowBase> prefabs;
        private List<WindowBase> freeWindows = new List<WindowBase>();
        private List<WindowBase> shownWindows = new List<WindowBase>();
        private void Awake()
        {
            SetNewInstance(this);
            // if (FindObjectOfType<EventSystem>()) GetComponent<EventSystem>().enabled = false;
            if (useDontDestroyOnLoad) DontDestroyOnLoad(this);
        }
        private void SetNewInstance(WindowManager instance)
        {
            if (_instance != null && _instance != instance) Destroy(_instance.gameObject);
            _instance = instance;
            instance.canvas = instance.GetComponent<Canvas>();
            instance.prefabs = Resources.LoadAll<WindowBase>("").ToList();
        }
        public T Show<T>() where T : WindowBase
        {
            var freeWindows = this.freeWindows.FindAll(w => w is T);
            var shownWindows = this.shownWindows.FindAll(w => w is T);
            var windowPrefab = prefabs.Find(w => w is T);
            if (!windowPrefab)
            {
                Debug.LogError("Window " + typeof(T).ToString() + " not found");
                return null;
            };
            if (freeWindows.Count > 0 && windowPrefab.isReusableView)
            {
                return ShowExistView((T)freeWindows[0]);
            }
            return CreateView((T)windowPrefab);
        }
        private T ShowExistView<T>(T window) where T : WindowBase
        {
            // GetOpenedWindow<T>()?.Close();
            window.gameObject.SetActive(true);
            window.transform.SetSiblingIndex(root.childCount - 1);
            window.OnOpened();
            window.ShowAnimation();
            window.active = true;
            shownWindows.Add(window);
            freeWindows.Remove(window);
            return (T)window;
        }
        private T CreateView<T>(T windowPrefab) where T : WindowBase
        {
            var window = Instantiate(windowPrefab, root);
            window.OnOpened();
            window.ShowAnimation();
            shownWindows.Add(window);
            window.active = true;
            return (T)window;
        }
        public void Close(WindowBase closeWindow)
        {
            Debug.Log("close " + closeWindow?.name);
            var win = shownWindows.FindLast(w => w == closeWindow);
            if (!win) return;
            win.onClose.Invoke();
            win.onClose.RemoveAllListeners();
            win.connections.DisconnectAll();
            win.active = false;
            TaskTools.Wait(win.CloseAnimation(), () =>
            {
                freeWindows.Add(win);
                shownWindows.Remove(win);
                if (win.isReusableView) win.gameObject.SetActive(false);
                else
                {
                    freeWindows.Remove(win);
                    Destroy(win.gameObject);
                }
            });
        }
        public void CloseTop() => Close(shownWindows.FindLast(w => w.active == true && w.closeButton != null));
        public void CloseAll()
        {
            for (int i = shownWindows.Count - 1; i >= 0; i--)
            {
                shownWindows[i]?.Close();
            }
        }
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