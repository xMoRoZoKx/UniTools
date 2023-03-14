using System;
using UnityEngine;
using UnityEngine.UI;

public static class MonobehaviorTools
{
    public static void SetActive(this Component component, bool value)
    {
        component?.gameObject?.SetActive(value);
    }
    public static T GetOrAddComponent<T>(this Component component) where T : Component
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
