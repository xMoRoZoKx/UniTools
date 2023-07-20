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
    public T CreateObject<T>(T prefab, Transform container = null, System.Action<T> onShown = null) where T : MonoBehaviour
    {
        if(prefabs.Count == 0)
        {
            Debug.Log("prefabs not seted");
        }
        var existView = (T)pool.Find(o => o is T && !o.gameObject.activeInHierarchy);
        if (existView != null)
        {
            existView.SetActive(true);
            existView.transform.position = container.position;
            onShown?.Invoke(existView);
            return existView;
        }
        var createdObject = Object.Instantiate(prefab, container);

        createdObject.SetActive(true);

        pool.Add(createdObject);

        onShown?.Invoke(createdObject);

        return createdObject;
    }
}
