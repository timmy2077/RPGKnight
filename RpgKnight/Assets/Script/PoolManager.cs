using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;   // 单例

    // 存储每种预制体对应的对象池
    private Dictionary<GameObject, ObjectPool> pools = new Dictionary<GameObject, ObjectPool>();

    void Awake()
    {
        Instance = this;
    }

    // 创建新的对象池（可配置初始大小和最大大小）
    public void CreatePool(GameObject prefab, int initialSize, int maxSize)
    {
        if (!pools.ContainsKey(prefab))
        {
            GameObject poolGO = new GameObject(prefab.name + "Pool");
            poolGO.transform.SetParent(transform);
            ObjectPool pool = poolGO.AddComponent<ObjectPool>();
            pool.prefab = prefab;
            pool.initialSize = initialSize;
            pool.maxSize = maxSize;
            pool.Initialize();   // 调用初始化方法（需在ObjectPool中添加）
            pools.Add(prefab, pool);
        }
    }

    // 从池中获取对象
    public GameObject GetObject(GameObject prefab)
    {
        if (pools.ContainsKey(prefab))
        {
            return pools[prefab].Get();
        }
        else
        {
            Debug.LogWarning("对象池不存在，请先创建池：" + prefab.name);
            return null;
        }
    }

    // 回收对象（如果对象属于某个池）
    public void ReturnObject(GameObject obj, GameObject prefab)
    {
        if (pools.ContainsKey(prefab))
        {
            pools[prefab].Return(obj);
        }
        else
        {
            // 如果没有池，直接销毁
            Destroy(obj);
        }
    }
}