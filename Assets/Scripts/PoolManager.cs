using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class PoolData
{
    [SerializeField]
    private int quantity = -1;
    [SerializeField]
    private GameObject prefab = null;

    public int Quantity { get { return quantity; } }
    public GameObject Prefab { get { return prefab; } }

    public bool IsValid { get { return quantity != -1 && prefab != null; } }
}

public class PoolManager : MonoBehaviour {
    [SerializeField]
    public List<PoolData> poolsToSpawn;
    
    private void Awake()
    {
        poolsToSpawn.Where(pool => pool.IsValid)
            .ForEach(data => Debug.LogFormat("Prefab: {0}, Quantity: {1}", data.Prefab.name, data.Quantity));
    }    
}
