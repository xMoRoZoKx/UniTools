using System;
using System.Collections;
using UnityEngine;

public static class AsyncOperationTools
{
    public static void OnComplete(this AsyncOperation asyncLoad, Action action, string operationName = "AsyncOperation")
    {
        CoroutineRunner runner = new GameObject(operationName).AddComponent<CoroutineRunner>();
        UnityEngine.Object.DontDestroyOnLoad(runner);
        var cor = runner.StartCoroutine(LoadAsync(asyncLoad, () =>
        {
            UnityEngine.Object.DestroyImmediate(runner.gameObject);
            action();
        }));
    }
    static IEnumerator LoadAsync(AsyncOperation asyncLoad, Action onComplete)
    {
        if(asyncLoad == null) yield break;
        asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
        onComplete();
    }
}
