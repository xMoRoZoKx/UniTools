using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ScaneManagmentTools
{
    public static void OnComplete(this AsyncOperation asyncLoad, Action action)
    {
        CoroutineRunner runner = new GameObject("Loading scene...").AddComponent<CoroutineRunner>();
        UnityEngine.Object.DontDestroyOnLoad(runner);
        var cor = runner.StartCoroutine(LoadAsyncScene(asyncLoad, () =>
        {
            UnityEngine.Object.DestroyImmediate(runner.gameObject);
            action();
        }));
    }
    static IEnumerator LoadAsyncScene(AsyncOperation asyncLoad, Action onComplete)
    {
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
