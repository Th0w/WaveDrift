using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[Serializable]
public class SpawnData
{
    public GameObject prefab;
    public float timer;
    public int quantity;
    public float distFromSpawn;
}

public class Spawner : MonoBehaviour {
    private PoolManager poolManager;

    public List<SpawnData> spawnDatas;

    private void Awake()
    {
        poolManager = FindObjectOfType<PoolManager>();

        spawnDatas.ForEach(spawnData =>
        {
            Pool p = poolManager[spawnData.prefab];
            int i, max = spawnData.quantity;
            for(i = 0; i < max; ++i)
            {
                Vector3 position = transform.position + Quaternion.Euler(0, 360.0f / max * i, 0) * transform.forward * spawnData.distFromSpawn;
                Observable.Timer(TimeSpan.FromSeconds(spawnData.timer))
                    .Subscribe(_ => p.Spawn(position));
            }
        });
    }
}
