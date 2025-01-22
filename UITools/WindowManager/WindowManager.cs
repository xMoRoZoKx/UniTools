using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace UniTools
{
    [RequireComponent(typeof(Canvas))]
    public class WindowManager : MonoBehaviour
    {
        private static WindowManager _instance;
        public static WindowManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var managers = FindObjectsOfType<WindowManager>(true).ToList();

                    if (managers.Count() == 0)
                    {
                        Debug.Log("Spawn default manager");

                        _instance = Instantiate(Resources.LoadAll<WindowManager>("")[0]);

                        return null;
                    }
                    else _instance = managers[0];

                    _instance.SetActive(true);
                    _instance.SetNewInstance(_instance);
                    // if (FindObjectOfType<EventSystem>()) // TODO new obj // _instance.GetComponent<EventSystem>().enabled = false;
                }
                return _instance;
            }
        }
        [SerializeField] private Transform root, nonClickBG;
        [SerializeField] private bool useDontDestroyOnLoad = true;
        public Canvas canvas { get; private set; }
        private List<WindowBase> prefabs;
        private List<WindowBase> freeWindows = new List<WindowBase>();
        private List<WindowBase> shownWindows = new List<WindowBase>();
        private void Awake()
        {
            if (_instance == null) SetNewInstance(this);
            if (_instance != this) Destroy(gameObject);
        }
        private void SetNewInstance(WindowManager instance)
        {
            if (_instance != null && _instance != instance)
            {
                Destroy(_instance.gameObject);
                return;
            }

            _instance = instance;
            instance.canvas = instance.GetComponent<Canvas>();

            try
            {
                instance.prefabs = Resources.LoadAll<WindowBase>("").ToList();
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }

            instance.freeWindows.AddRange(root.GetComponentsInChildren<WindowBase>(true));

            nonClickBG.SetActive(false);

            if (useDontDestroyOnLoad) DontDestroyOnLoad(this);

            // instance.prefabs.ForEach(p => Instantiate(p, root));
        }
        public T Show<T>(Action<T> onShown = null) where T : WindowBase
        {
            return Show(prefabName: null, onShown);
        }
        public T Show<T>(string prefabName, Action<T> onShown = null) where T : WindowBase
        {
            var freeWindows = this.freeWindows.FindAll(w => w is T);
            var shownWindows = this.shownWindows.FindAll(w => w is T);
            var windowPrefab = prefabs.Find(w => (String.IsNullOrEmpty(prefabName) || prefabName == w.gameObject.name) && w is T);


            if (!windowPrefab && freeWindows.Count == 0)
            {
                if (string.IsNullOrEmpty(prefabName))
                    Debug.LogError($"Window {typeof(T)} not found");
                else
                    Debug.LogError($"Window by type: {typeof(T)}, with name: {prefabName} not found");

                return null;
            };

            if (!windowPrefab.canShow) return null;
            
            if (freeWindows.Count > 0 && (windowPrefab == null || windowPrefab.isReusableView))
            {
                return ShowExistView((T)freeWindows.Find(w => typeof(T) == w.GetType()), onShown);
            }

            return CreateView((T)windowPrefab, onShown);
        }
        private T ShowExistView<T>(T window, Action<T> onShown) where T : WindowBase
        {
            Show(window, onShown);

            freeWindows.Remove(window);

            return window;
        }
        private T CreateView<T>(T windowPrefab, Action<T> onShown) where T : WindowBase
        {

            var window = Instantiate(windowPrefab, root);

            window.gameObject.name = windowPrefab.gameObject.name;

            Show(window, onShown);

            return window;
        }
        private void Show<T>(T window, Action<T> onShown) where T : WindowBase
        {
            window.gameObject.SetActive(true);

            window.OnOpened();
            window.active = true;

            nonClickBG.SetActive(true);

            this.Wait(window.ShowAnimation(), () => nonClickBG.SetActive(false));

            shownWindows.Add(window);


            OrderViews();

            if (IsTopWindow(window)) window.OnTop();
            if (shownWindows.Count > 1) shownWindows[^2].OnBottom();

            onShown?.Invoke(window);
        }
        public bool IsTopWindow<T>(T window) where T : WindowBase => shownWindows.LastOrDefault() == window;
        public bool IsOpend<T>(T window) where T : WindowBase => shownWindows.Contains(window);
        public bool IsOpend<T>() where T : WindowBase => GetOpenedWindow<T>() != null;
        private void OrderViews()
        {
            shownWindows.Sort((sw1, sw2) => sw1.orderBy > sw2.orderBy ? 1 : -1);
            for (int i = 0; i < shownWindows.Count; i++)
            {
                shownWindows[i].transform.SetSiblingIndex(i);
            }
            shownWindows.ForEach(sw =>
            {
                if (sw.active && sw.needHideThenWindowIsNotTop)
                {
                    Enable(sw, sw == shownWindows.LastOrDefault());
                }
            });
        }
        public void Enable<T>(T window, bool value) where T : WindowBase
        {
            window.SetActive(value);
        }
        public void CloseTop<T>() where T : WindowBase
        {
            Close(GetOpenedWindow<T>());
        }
        public void Close(WindowBase closeWindow)
        {
            Debug.Log("close " + closeWindow?.name);

            var win = shownWindows.FindLast(w => w == closeWindow);
            if (!win) return;

            win.onClose.Invoke();
            win.onCloseAction?.Invoke();
            win.OnClosed();
            win.connections.DisconnectAll();
            win.active = false;

            nonClickBG.SetActive(true);

            shownWindows.Remove(win);
            OrderViews();
            shownWindows.LastOrDefault()?.OnTop();

            this.Wait(win.CloseAnimation(), () =>
            {
                nonClickBG.SetActive(false);
                freeWindows.Add(win);


                if (win.isReusableView) win.gameObject.SetActive(false);
                else
                {
                    freeWindows.Remove(win);
                    Destroy(win.gameObject);
                }
            });
        }
        public void CloseTop() => Close(shownWindows.LastOrDefault());
        public void CloseAll()
        {
            for (int i = shownWindows.Count - 1; i >= 0; i--)
            {
                shownWindows[i]?.Close();
            }
        }
        public T GetOpenedWindow<T>() where T : WindowBase
        {
            var win = shownWindows.Find(w => w is T);

            if (win == null) return null;

            return (T)win;
        }
        public T GetWindowPrefab<T>() where T : WindowBase
        {
            var win = prefabs.Find(w => w is T);

            if (win == null) return null;

            return (T)win;
        }
        void OnGUI()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                // CloseTop();
                Close(shownWindows.FindLast(w => w.active == true && w.closeButton != null));
            }
        }
        private void OnApplicationQuit()
        {

        }
    }
}