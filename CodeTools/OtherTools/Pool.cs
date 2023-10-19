using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[System.Serializable]
public class Pool
{
    [SerializeField] private List<MonoBehaviour> prefabs = new List<MonoBehaviour>();
    private List<MonoBehaviour> pool = new List<MonoBehaviour>();

    public void Init(IEnumerable<MonoBehaviour> prefabs)
    {
        this.prefabs = prefabs.ToList();
    }
    public void FillPool(List<MonoBehaviour> prefabs)
    {
        prefabs.ForEach(p =>
        {
            CreateObject(p).SetActive(false);

        });
    }
    public T CreateObject<T>(T defaultPrefab, Func<T, bool> spawnCondition = null, Transform container = null, System.Action<T> onShown = null) where T : MonoBehaviour
    {
        if (prefabs.Count == 0)
        {
            Debug.Log($"prefabs with type {typeof(T)} not seted");
        }
        var existView = (T)pool.Find(o => o is T && !o.gameObject.activeInHierarchy && (spawnCondition == null || spawnCondition.Invoke((T)o)));
        if (existView != null)
        {
            existView.SetActive(true);
            existView.transform.position = container.position;
            existView.transform.SetParent(null);
            onShown?.Invoke(existView);
            return existView;
        }
        var createdObject = UnityEngine.Object.Instantiate(defaultPrefab, container);

        createdObject.SetActive(true);

        pool.Add(createdObject);

        onShown?.Invoke(createdObject);

        return createdObject;
    }
}
