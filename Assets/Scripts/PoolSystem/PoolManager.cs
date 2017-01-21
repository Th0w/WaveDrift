﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UniRx;

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

    public PoolData() { }

    public PoolData(string name, int quantity, GameObject prefab)
    {
        this.name = name;
        this.prefab = prefab;
        this.quantity = quantity;
    }
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

    private void Test()
    {
        Observable.Interval(TimeSpan.FromSeconds(2.0))
            .Where(_ => pools[0].empty == false)
            .Subscribe(_ =>
            {
                pools[0].Spawn(Vector3.zero);
            })
            .AddTo(this);
    }
    
    private Pool CreatePool(PoolData data)
    {
        Pool p = new GameObject().AddComponent<Pool>();
        p.Init(data.Quantity, data.Prefab.GetComponent<Poolable>());
        p.name = string.Format("Pool_{0}_{1}", data.Prefab.name, data.Name);
        p.transform.SetParent(transform);
        return p;
    }

    public Pool this[GameObject prefab]
    {
        get
        {
            return pools
                .Where(pool => pool.Prefab.gameObject == prefab)
                .FirstOrDefault()
                ?? CreatePool(new PoolData("Error", 10, prefab));
            
        }
    }
}