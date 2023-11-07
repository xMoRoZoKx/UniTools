using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class UnityTools
{
    public static void SetActive(this Component component, bool value)
    {
        component?.gameObject?.SetActive(value);
    }
    public static T GetOrAddComponent<T>(this Component component) where T : Component
    {
        var c = component.gameObject.GetComponent<T>();// ?? component.gameObject.AddComponent<T>();
        if (c == null) c = component.gameObject.AddComponent<T>();
        return c;
    }
    public static void OnClick(this Button button, Action onClick, bool clearOther = true)
    {
        if (clearOther) button?.onClick.RemoveAllListeners();
        button?.onClick.AddListener(() => onClick?.Invoke());
    }
    public static IDisposable Subscribe(this Button button, Action onClick)
    {
        return button?.onClick.Subscribe(onClick);
    }
    public static IDisposable Subscribe(this UnityEvent unityEvent, Action action)
    {
        UnityAction unityAction = () => action?.Invoke();
        unityEvent.AddListener(unityAction);
        return new DisposableAction(() => unityEvent.RemoveListener(unityAction));
    }
    
    public static IDisposable Subscribe<T>(this UnityEvent<T> unityEvent, Action<T> action)
    {
        UnityAction<T> unityAction = val => action?.Invoke(val);
        unityEvent.AddListener(unityAction);
        return new DisposableAction(() => unityEvent.RemoveListener(unityAction));
    }
    public static void SetAction(this UnityEvent unityEvent, Action action, bool clearOther = true)
    {
        if (clearOther) unityEvent?.RemoveAllListeners();
        unityEvent?.AddListener(() => action?.Invoke());
    }
    public static void SetAction<T>(this UnityEvent<T> unityEvent, Action<T> action, bool clearOther = true)
    {
        if (clearOther) unityEvent?.RemoveAllListeners();
        unityEvent?.AddListener(t => action?.Invoke(t));
    }
    public static void Move(this Transform transform, float x, float y, float z)
    {
        transform.position += new Vector3(x, y, z);
    }
    public static void MoveTo(this Transform transform, Vector3 targetPosition, float step)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
    }
    public static void Teleportation(this Transform transform, Vector3 position)
    {
        transform.position += position - transform.position;
    }
    public static void LocalRotate(this Transform transform, float xAngle, float yAngle, float zAngle)
    {
        var startRot = transform.localEulerAngles;
        transform.localRotation = Quaternion.Euler(startRot.x + xAngle, startRot.y + yAngle, startRot.z + zAngle);
    }
    public static void Teleportation(this Transform transform, Vector2 position)
    {
        transform.position += (Vector3)(position - new Vector2(transform.position.x, transform.position.y));
    }
    public static Vector3 ScreenToWorldPointPerspective(this Camera camera, Vector2 screenPos)
    {
        Plane plane = new Plane(Vector3.back, Vector3.zero);
        Ray ray = camera.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y));
        if (plane.Raycast(ray, out float enter))
        {
            // Debug.Log(ray.GetPoint(enter));
            return ray.GetPoint(enter);
        }
        return ray.GetPoint(1);
    }
    public static Vector3 ScreenToWorldPointPerspective(this Camera camera, Vector2 screenPos, float distance)
    {
        if (camera == null) return Vector3.zero;
        Plane plane = new Plane(Vector3.back, Vector3.zero);
        Ray ray = camera.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y));
        if (plane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(distance);
        }
        return ray.GetPoint(1);
    }
    public static void ResetShaders<T>(this GameObject prefab, string defaultShaderName = "Standard") where T : Renderer
    {
        Shader newShader;
        foreach (var child in prefab.transform.GetComponentsInChildren<T>())
        {
            newShader = Shader.Find(child.material.shader.name);
            if (newShader == null) newShader = Shader.Find(defaultShaderName);
            child.material.shader = newShader;
        }
    }
    public static void ResetAllShaders(this GameObject prefab)
    {
        ResetShaders<MeshRenderer>(prefab);
        ResetShaders<SpriteRenderer>(prefab);
        ResetShaders<SkinnedMeshRenderer>(prefab);
    }
    public static void SetSprite(this SpriteRenderer renderer, Sprite sprite)
    {
        float rotX = (float)renderer.sprite.texture.width / (float)sprite.texture.width;
        float rotY = (float)renderer.sprite.texture.height / (float)sprite.texture.height;
        float rotPixel = (float)renderer.sprite.pixelsPerUnit / (float)sprite.pixelsPerUnit;
        renderer.sprite = sprite;

        var scale = renderer.transform.localScale / rotPixel;

        renderer.transform.localScale = new Vector3(scale.x * rotX, scale.y * rotY, 0);
    }
    public static string ToTimerFormat(this int time) => ToTimerFormat((float)time);
    public static string ToTimerFormat(this long time) => ToTimerFormat((float)time);
    public static string ToTimerFormat(this double time) => ToTimerFormat((float)time);
    public static string ToTimerFormat(this float time)
    {
        var hour = (int)(time / 60 / 60);
        var min = (int)(time / 60) - (hour * 60);
        var sec = (int)(time % 60);
        return (hour > 0 ? (hour < 10 ? "0" : "") + hour + ":" : "") + (min < 10 ? "0" : "") + min + ":" + (sec < 10 ? "0" : "") + sec;
    }
}
