using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool instance;
    public GameObject prefab;
    public List<GameObject> pooledObjects;
    public int countToPool = 50;

    void Awake()
    {
        if (instance == null) { instance = this; }

        pooledObjects = new List<GameObject>();
        for (int i=0;i<countToPool; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }
    public GameObject GetPooledObject()
    {
        for (int i=0; i<pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy) return pooledObjects[i];
        }
        return null;
    }
}
