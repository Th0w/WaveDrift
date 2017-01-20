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

    [SerializeField]
    private string name = "";

    public int Quantity { get { return quantity; } }
    public GameObject Prefab { get { return prefab; } }
    public string Name { get { return name; } }

    public bool IsValid { get { return quantity != -1 && prefab != null; } }
}

public class PoolManager : MonoBehaviour {
    [SerializeField]
    public List<PoolData> poolsToSpawn;

    private List<Pool> pools;

    private void Awake()
    {
        pools = new List<Pool>();

        poolsToSpawn.Where(pool => pool.IsValid)
            .ForEach(data =>
            {
                pools.Add(CreatePool(data));
            });
    }
    
    private Pool CreatePool(PoolData data)
    {
        Pool p = new GameObject().AddComponent<Pool>();
        p.Init(data.Quantity, data.Prefab.GetComponent<Poolable>());
        p.name = string.Format("Pool_{0}_{1}", data.Prefab.name, data.Name);
        p.transform.SetParent(transform);
        return p;
    }
}
