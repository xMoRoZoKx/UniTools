using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tools;
using UITools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UITools
{
    public class Presenter<Data, View> where View : MonoBehaviour
    {
        private List<View> views = new List<View>();
        public List<View> Present(List<Data> list, View prefab, RectTransform container, Action<View, Data> onShow)
        {
            views.ForEach(v => v.SetActive(false));
            for (int i = 0; i < list.Count; i++)
            {
                if (views.Count <= i)
                    views.Add(UnityEngine.Object.Instantiate(prefab, container));
                views[i].SetActive(true);
                onShow?.Invoke(views[i], list[i]);
            }
            return views.GetRange(0, list.Count);
        }
    }
    public static class UITools
    {
        public static float GetWidth(this RectTransform rt, float canvasScaledFactor)
        {
            var w = (rt.anchorMax.x - rt.anchorMin.x) * Screen.width + rt.sizeDelta.x * canvasScaledFactor;
            return w;
        }
       
        public static float GetHeight(this RectTransform rt, float canvasScaledFactor)
        {
            var h = (rt.anchorMax.y - rt.anchorMin.y) * Screen.height + rt.sizeDelta.y * canvasScaledFactor;
            return h;
        }
    }
}
namespace Tools
{
    public static class RandomTools
    {
        public static void InvokWithChance(Action action, int chance)
        {
            if (chance == 0) return;
            if (UnityEngine.Random.Range(0, 100) <= chance) action?.Invoke();
        }
    }


    public static class ListTools
    {
        public static T GetRandom<T>(this List<T> list)
        {
            if (list.Count == 0) return default;
            return list[UnityEngine.Random.Range(0, list.Count())];
        }
        public static List<T> GetRandoms<T>(this List<T> list, int count)
        {
            if (list.Count == 0) return default;
            if (count > list.Count) count = list.Count;
            return list.ToList().Shuffle().GetRange(0, count);
        }
        public static List<T> Shuffle<T>(this List<T> list)
        {
            if (list.Count == 0) return default;
            for (int i = 0; i < list.Count; i++)
            {
                int j = UnityEngine.Random.Range(i, list.Count);
                if (j != i)
                {
                    var temp = list[j];
                    list[j] = list[i];
                    list[i] = temp;
                }
            }
            return list;
        }
        public static List<T> Resize<T>(this List<T> list, int size)
        {
            for (int i = 0; i < list.Count + size; i++)
            {
                list.Add(default);
            }
            if (list.Count > size) list = list.GetRange(0, size);
            return list;
        }
        public static bool AddIfNotContains<T>(this List<T> list, T element)
        {
            if (!list.Contains(element))
            {
                list.Add(element);
                return true;
            }
            return false;
        }
        public static Presenter<Data, View> Present<Data, View>(this List<Data> list, View prefab, RectTransform container, Action<View, Data> onShow) where View : MonoBehaviour
        {
            var presenter = new Presenter<Data, View>();
            presenter.Present(list, prefab, container, onShow);
            return presenter;
        }
    }
    public static class SoundTools
    {
        //TODO simple, for fast codding
        public static AudioSource PlayAudio(this Component component, AudioClip clip, float volume = 1, bool loop = false)
        {
            if (clip == null || component == null) return null;
            var c = component.GetOrAddCommponent<AudioSource>();
            c.enabled = true;
            c.volume = volume;
            c.clip = clip;
            c.loop = loop;
            c.Play();
            return c;
        }
    }
    public static class EnumTools
    {
        public static T[] GetValues<T>() where T : Enum => (T[])Enum.GetValues(typeof(T));
        public static T ToEnum<T>(this string str) where T : Enum
        {
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                if (e.ToString() == str) return (T)e;
            }
            return default;
        }
        public static bool TryConvertToEnum<T>(this string str) where T : Enum
        {
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                if (e.ToString() == str) return true;
            }
            return false;
        }
        public static T GetRandomEnum<T>() where T : Enum
        {
            var enums = Enum.GetValues(typeof(T));
            return (T)enums.GetValue(UnityEngine.Random.Range(0, enums.Length));
        }
    }
    public static class GeometryTools
    {
        public static Vector3 Direction(this Vector3 from, Vector3 to) => (to - from).normalized;
        public static Vector3 WithZ(this Vector3 from, float z) => new Vector3(from.x, from.y, z);
        public static Vector3 WithX(this Vector3 from, float x) => new Vector3(x, from.y, from.z);
        public static Vector3 WithY(this Vector3 from, float y) => new Vector3(from.x, y, from.z);
        public static Vector2 Direction(this Vector2 from, Vector2 to) => (to - from).normalized;
        public static Vector2 WithX(this Vector2 from, float x) => new Vector2(x, from.y);
        public static Vector2 WithY(this Vector2 from, float y) => new Vector2(from.x, y);
        public static Vector3 ToVector3(this Vector2 vector) => new Vector3(vector.x, vector.y, 0);
        public static Vector2 ToVector2(this Vector3 vector) => new Vector2(vector.x, vector.y);
    }
}
public static class MonobehaviorTools
{
    public static void SetActive(this Component component, bool value)
    {
        component?.gameObject?.SetActive(value);
    }
    public static T GetOrAddCommponent<T>(this Component component) where T : Component
    {
        var c = component.gameObject.GetComponent<T>();
        if (c == null) c = component.gameObject.AddComponent<T>();
        return c;
    }
    public static void OnClick(this Button button, Action onClick, bool clearOther = true)
    {
        if (clearOther) button?.onClick.RemoveAllListeners();
        button?.onClick.AddListener(() => onClick?.Invoke());
    }
    public static void AddEvent(this EventTrigger trigger, EventTriggerType type, Action<BaseEventData> callBack)
    {
        var trig = trigger.triggers.Find(t => t.eventID == type);
        if (trig == null)
        {
            trig = new EventTrigger.Entry() { eventID = type, callback = new EventTrigger.TriggerEvent() };
            trigger.triggers.Add(trig);
        }
        trig.callback.AddListener((eventData) => callBack?.Invoke(eventData));
    }
    public static void ClearAllEvents(this EventTrigger trigger)
    {
        trigger.triggers.ForEach(t => t.callback.RemoveAllListeners());
    }
    public static void ClearEvents(this EventTrigger trigger, EventTriggerType type)
    {
        trigger.triggers.Find(t => t.eventID == type)?.callback.RemoveAllListeners();
    }
    public static void Move(this Transform transform, float x, float y, float z)
    {
        transform.position += new Vector3(x, y, z);
    }
    public static void Teleportation(this Transform transform, Vector3 position)
    {
        transform.position += position - transform.position;
    }
    public static void Teleportation(this Transform transform, Vector2 position)
    {
        transform.position += (Vector3)(position - new Vector2(transform.position.x, transform.position.y));
    }
}
