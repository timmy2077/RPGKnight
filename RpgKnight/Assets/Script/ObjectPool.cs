using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int initialSize = 10;
    public int maxSize = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();

    public void Initialize()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }

    private GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
        return obj;
    }

    public GameObject Get()
    {
        if (pool.Count == 0)
        {
            if (maxSize > 0 && transform.childCount >= maxSize)
            {
                Debug.LogWarning("对象池已达最大容量，无法创建新对象：" + prefab.name);
                return null;
            }
            CreateNewObject();
        }

        GameObject obj = pool.Dequeue();
        obj.SetActive(true);

        Arrow arrow = obj.GetComponent<Arrow>();
        if (arrow != null)
        {
            arrow.OnSpawn(this);
        }

        EnemyMovement enemyMovement = obj.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.OnSpawn();
        }

        Enemy_Health enemyHealth = obj.GetComponent<Enemy_Health>();
        if (enemyHealth != null)
        {
            enemyHealth.OnSpawn();
        }

        return obj;
    }

    public void Return(GameObject obj)
    {
        if (obj == null || !obj) return;
        
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
    }
}